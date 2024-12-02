import { Store } from '@ngrx/store';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersTableComponent } from '../../components/orders/orders-table/orders-table.component';
import { selectIsCreateOrderModalOpen, selectIsDeleteOrderModalOpen, selectIsUpdateOrderModalOpen } from '../../../../../core/state/modal/order/modal.selectors';
import { OrderUpdateModalComponent } from '../../components/orders/order-update-modal/order-update-modal.component';
import { OrderCreateModalComponent } from '../../components/orders/order-create-modal/order-create-modal.component';
import { OrderDeleteModalComponent } from '../../components/orders/order-delete-modal/order-delete-modal.component';
import { animate, state, style, transition, trigger } from '@angular/animations';
@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [
    CommonModule,
    OrdersTableComponent,
    OrderUpdateModalComponent,
    OrderCreateModalComponent,
    OrderDeleteModalComponent,
  ],
  templateUrl: './orders.component.html',
  animations: [
    trigger('slideInRight', [
      transition(':enter', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateX(0)', opacity: 1 })),
      ]),
    ]),
    trigger('fadeInOut', [
      state('in', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('0.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class OrdersComponent {
  showUpdateOrderModal$ = this.store.select(selectIsUpdateOrderModalOpen);
  showCreateOrderModal$ = this.store.select(selectIsCreateOrderModalOpen);
  showDeleteOrderModal$ = this.store.select(selectIsDeleteOrderModalOpen);


  constructor(private store: Store) {}
}
