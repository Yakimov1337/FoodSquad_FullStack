import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray, FormControl } from '@angular/forms';
import { Store } from '@ngrx/store';
import { map, Observable, of } from 'rxjs';
import { MenuItem, User } from '../../../../../../core/models';
import { selectOrderToUpdate } from '../../../../../../core/state/modal/order/modal.selectors';
import { closeUpdateOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { OrdersService } from '../../../../../../services/orders.service';
import { UserService } from '../../../../../../services/user.service';
import { CommonModule, formatDate } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { MenuItemsService } from '../../../../../../services/menuItems.service';

@Component({
  selector: '[order-update-modal]',
  templateUrl: './order-update-modal.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class OrderUpdateModalComponent implements OnInit {
  orderForm: FormGroup;
  users$: Observable<User[]>;
  menuItems$: Observable<MenuItem[]> = of([]);
  minDate: string;
  isMenuItemsDropdownOpen: boolean = false;
  orderedMenuItemTitles$: Observable<number[]> = of([]);

  private currentOrderId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private ordersService: OrdersService,
    private menuItemsService: MenuItemsService,
    private userService: UserService,
    private toastr: ToastrService,
  ) {
    const currentDate = new Date().toISOString().split('T')[0];
    this.minDate = currentDate;
    this.orderForm = this.fb.group({
      userEmail: ['', Validators.required],
      createdOn: ['', Validators.required],
      status: ['', Validators.required],
      paid: [false, Validators.required],
      menuItemQuantities: this.fb.group({}),
    });
    this.users$ = this.userService.getAllUsers();
  }

  ngOnInit(): void {

      this.menuItems$ = this.menuItemsService.getAllMenuItems(1, 100).pipe(
        map(response => response.items)
      );

    this.store.select(selectOrderToUpdate).subscribe((order) => {
      if (order) {
        this.currentOrderId = order.id;
        this.orderForm.patchValue({
          userEmail: order.userEmail,
          paid: order.paid,
          createdOn: formatDate(order.createdOn, 'yyyy-MM-dd', 'en-US'),
          status: order.status.toUpperCase(),
        });

        // Set selected menu items
        const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
        Object.entries(order.menuItemQuantities).forEach(([id, qty]) => {
          menuItemQuantitiesGroup.addControl(id, new FormControl(qty, Validators.min(1)));
        });
      }
    });
  }

  updateOrder(): void {
    if (this.orderForm.valid && this.currentOrderId) {
      const orderData = this.orderForm.value;

      const menuItemQuantities = this.orderForm.get('menuItemQuantities')?.value;
      if (!menuItemQuantities || Object.keys(menuItemQuantities).length === 0) {
        this.toastr.error('Order must contain at least one menu item');
        return;
      }

      // Convert 'createdOn' to ISO 8601 format (YYYY-MM-DD) (Appwrite problems...)
      orderData.createdOn = formatDate(orderData.createdOn, 'yyyy-MM-ddTHH:mm:ss', 'en-US');
      this.ordersService.updateOrder(this.currentOrderId, orderData).subscribe({
        next: (order) => {
          this.closeModal();
          this.ordersService.orderUpdated(order); //Notify about order update
          this.toastr.success('Order updated successfully!');
        },
        error: (error) => this.toastr.error('Error updating this order:', error),
      });
    } else {
      this.orderForm.markAllAsTouched();
      // If the form is invalid, iterate over the controls and log the errors
      Object.keys(this.orderForm.controls).forEach((key) => {
        const control = this.orderForm.get(key);
        const errors = control?.errors ?? {};
        Object.keys(errors).forEach((keyError) => {
          this.toastr.error(`Form Invalid - control: ${key}, Error: ${keyError}`);
        });
      });
    }
  }

  onMenuItemSelected(event: Event, menuItemId: number): void {
    const inputElement = event.target as HTMLInputElement;
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;

    if (inputElement.checked) {
      menuItemQuantitiesGroup.addControl(menuItemId.toString(), new FormControl(1, Validators.min(1)));
    } else {
      menuItemQuantitiesGroup.removeControl(menuItemId.toString());
    }
  }

  isMenuItemSelected(menuItemId: number): boolean {
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
    return menuItemQuantitiesGroup.controls.hasOwnProperty(menuItemId.toString());
  }

  getQuantity(menuItemId: number): number {
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
    return menuItemQuantitiesGroup.get(menuItemId.toString())?.value || 1;
  }

  onQuantityChange(menuItemId: number, event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
    menuItemQuantitiesGroup.get(menuItemId.toString())?.setValue(parseInt(inputElement.value, 10));
  }

  increaseQuantity(menuItemId: number): void {
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
    const currentQuantity = this.getQuantity(menuItemId);
    menuItemQuantitiesGroup.get(menuItemId.toString())?.setValue(currentQuantity + 1);
  }

  decreaseQuantity(menuItemId: number): void {
    const menuItemQuantitiesGroup = this.orderForm.get('menuItemQuantities') as FormGroup;
    const currentQuantity = this.getQuantity(menuItemId);
    if (currentQuantity > 1) {
      menuItemQuantitiesGroup.get(menuItemId.toString())?.setValue(currentQuantity - 1);
    }
  }

  closeModal(): void {
    this.store.dispatch(closeUpdateOrderModal());
  }
}
