using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly StoreContext _context;
    public BuggyController(StoreContext context)
    {
        _context = context;
    }

    [HttpGet("testauth")]
    [Authorize]
    public ActionResult<string> GetSecretText()
    {
        return "secret stuff";
    }

    [HttpGet("notfound")]
    public ActionResult GetNotFoundRequest()
    {
        return _context.Products.Find(42) == null
            ? NotFound(new ApiResponse(404)) : Ok();
    }


    [HttpGet("servererror")]
    public ActionResult GetServerError()
    {
        var thing = _context.Products.Find(42);

        var thingToReturn = thing.ToString();

        return Ok();
    }


    [HttpGet("badrequest")]
    public ActionResult GetBadRequest()
    {
        return BadRequest(new ApiResponse(400));
    }


    [HttpGet("badrequest/{id}")]
    public ActionResult GetBadRequest(int id)
    {
        return Ok();
    }

    [HttpGet("maths/{id}")]
    public ActionResult GetMath(int id)
    {
        var x = 10 / id;
        return Ok();
    }
}
