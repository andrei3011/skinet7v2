import { Component, OnInit } from '@angular/core';
import { Order } from 'src/app/shared/models/order';
import { OrdersService } from '../orders.service';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {
  order?: Order;

  constructor(private ordersService: OrdersService, private activatedRoute: ActivatedRoute, private bcService: BreadcrumbService) {
    this.bcService.set('@orderDetails', ' ');
  }

  ngOnInit(): void {
    this.getOrder();
  }

  getOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;

    this.ordersService.getOrder(+id).subscribe({
      next: order => {
        this.order = order
        this.bcService.set('@orderDetails', `Order# ${order.id} - ${order.status}`);
      }
    });
  }
}
