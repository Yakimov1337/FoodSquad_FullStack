import { Component } from '@angular/core';
import { OrdersTableComponent } from './components/orders-table/orders-table.component';
import { animate, style, transition, trigger } from '@angular/animations';
@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [  OrdersTableComponent,],
  templateUrl: './orders.component.html',
  animations: [
    trigger('slideInDown', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
})
export class OrdersComponent {

}
