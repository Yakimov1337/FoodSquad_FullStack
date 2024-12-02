import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Order, User } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { Store, select } from '@ngrx/store';
import { openDeleteOrderModal, openUpdateOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { Observable } from 'rxjs';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';

@Component({
  selector: '[orders-table-item]',
  templateUrl: './orders-table-item.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, CurrencyPipe, ButtonComponent, CommonModule],
})
export class OrdersTableItemComponent implements OnInit {
  @Input() order: Order = <Order>{};
  currentUser$: Observable<User | null>;

  constructor(private store: Store) {
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }

  ngOnInit(): void {console.log(this.order.id)}

  openUpdateModal() {
    this.store.dispatch(openUpdateOrderModal({ order: this.order }));
  }

  openDeleteModal() {
    this.store.dispatch(openDeleteOrderModal({ orderId: this.order.id }));
  }
}
