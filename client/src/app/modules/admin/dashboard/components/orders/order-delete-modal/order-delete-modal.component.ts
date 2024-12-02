import { Subscription } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { selectDeleteOrderId } from '../../../../../../core/state/modal/order/modal.selectors';
import { closeDeleteOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { OrdersService } from '../../../../../../services/orders.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[order-delete-modal]',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-delete-modal.component.html',
})
export class OrderDeleteModalComponent {
  orderIdToDelete: string | undefined | null = null;
  private subscription = new Subscription();

  constructor(private store: Store, private ordersService: OrdersService, private toastr: ToastrService) {
    this.subscription.add(
      this.store.select(selectDeleteOrderId).subscribe((orderId) => {
        this.orderIdToDelete = orderId;
      }),
    );
  }
  ngOnInit(): void {
    console.log(this.orderIdToDelete);
  }

  deleteOrder(): void {
    if (!this.orderIdToDelete) {
      this.toastr.error('No order ID to delete provided!');
      return;
    }
    this.ordersService.deleteOrder(this.orderIdToDelete).subscribe({
      next: () => {
        this.closeModal();
        this.ordersService.orderDeleted(this.orderIdToDelete); //Notify about deletion
        this.toastr.success(`Order with ID ${this.orderIdToDelete} deleted successfully!`);
      },
      error: (error) => this.toastr.error('Error deleting order:', error)
    });
  }

  closeModal(): void {
    this.store.dispatch(closeDeleteOrderModal());
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
