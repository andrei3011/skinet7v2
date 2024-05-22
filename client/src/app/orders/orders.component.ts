import { Component, OnInit } from '@angular/core';
import { OrdersService } from './orders.service';
import { Order } from '../shared/models/order';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];

  constructor(private ordersService: OrdersService) { }

  ngOnInit(): void {
    this.getOrders();
  }

  getOrders() {
    this.ordersService.getOrders().subscribe({ next: orders => this.orders = orders });
  }
}
