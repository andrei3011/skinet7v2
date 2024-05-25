using API.Errors;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace API.Controllers;

public class PaymentsController : BaseApiController
{
    private const string WhSecret = "whsec_b03ef60fd0a9d469fceb6d8ba3906ac3b360c77735a05bfc1d01fa7a2efaeae3";
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;
    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

        if (basket == null) return BadRequest(new ApiResponse(400, "Problem with your basket"));

        return basket;
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(json,
            Request.Headers["Stripe-Signature"], WhSecret);

        PaymentIntent intent;
        Order order;

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogInformation("Payment succeeded: " + intent.Id, intent.Id);
                order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                if (order == null) break;
                _logger.LogInformation("Order updated to payment received: " + order.Id, order.Id);
                break;
            case "payment_intent.payment_failed":
                intent = (PaymentIntent)stripeEvent.Data.Object;
                _logger.LogInformation("Payment succeeded: " + intent.Id, intent.Id);
                order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                if (order == null) break;
                _logger.LogInformation("Order updated to payment received: " + order.Id, order.Id);
                break;
        }

        return new EmptyResult();
    }
}
