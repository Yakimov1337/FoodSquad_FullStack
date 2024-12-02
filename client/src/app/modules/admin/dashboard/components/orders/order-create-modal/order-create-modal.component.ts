import { map, Observable, of } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule, formatDate } from '@angular/common';
import { MenuItem, Order, User } from '../../../../../../core/models';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormControl, FormArray } from '@angular/forms';
import { OrdersService } from '../../../../../../services/orders.service';
import { closeCreateOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { UserService } from '../../../../../../services/user.service';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { ToastrService } from 'ngx-toastr';

interface MenuItemQuantity {
  menuItemId: number;
  quantity: number;
}

@Component({
  selector: '[order-create-modal]',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './order-create-modal.component.html',
})
export class OrderCreateModalComponent {
  orderForm: FormGroup;
  users$: Observable<User[]> = of([]); // initial value
  menuItems$!: Observable<MenuItem[]>;
  minDate: string;
  isMenuItemsDropdownOpen: boolean = false;

  constructor(
    private fb: FormBuilder,
    private orderService: OrdersService,
    private userService: UserService,
    private menuItemsService: MenuItemsService,
    private store: Store,
    private toastr: ToastrService,
  ) {
    const currentDate = new Date().toISOString().split('T')[0];
    this.minDate = currentDate;
    this.orderForm = this.fb.group({
      userEmail: ['', Validators.required],
      createdOn: [currentDate, [Validators.required]],
      menuItemQuantities: this.fb.array([]),
      paid: [false, Validators.required],
      status: ['PENDING', Validators.required],
    });
  }

  ngOnInit() {
    this.users$ = this.userService.getAllUsers();
    this.menuItems$ = this.menuItemsService.getAllMenuItems(1, 100).pipe(map((response) => response.items));
  }

  createOrder(): void {
    if (this.orderForm.valid) {
      const orderData = this.orderForm.value;

      const menuItemQuantities = (this.orderForm.get('menuItemQuantities') as FormArray).controls.reduce(
        (acc: Record<number, number>, control) => {
          const value = control.value as MenuItemQuantity;
          acc[value.menuItemId] = value.quantity;
          return acc;
        },
        {},
      );
      if (!menuItemQuantities || Object.keys(menuItemQuantities).length === 0) {
        this.toastr.error('Order must contain at least one menu item');
        return;
      }

      orderData.menuItemQuantities = menuItemQuantities;

      orderData.createdOn = formatDate(orderData.createdOn, 'yyyy-MM-ddTHH:mm:ss', 'en-US');

      this.orderService.createOrder(orderData).subscribe({
        next: (order: Order) => {
          this.closeModal();
          this.resetForm();
          this.orderService.orderCreated(order);
          this.toastr.success('Order created successfully!');
        },
        error: (error: any) => this.toastr.error('Failed to create new order!', error),
      });
    } else {
      this.orderForm.markAllAsTouched();
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
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;

    if (inputElement.checked) {
      menuItemsFormArray.push(this.fb.group({ menuItemId, quantity: 1 }));
    } else {
      const index = menuItemsFormArray.controls.findIndex((control) => control.get('menuItemId')?.value === menuItemId);
      if (index !== -1) {
        menuItemsFormArray.removeAt(index);
      }
    }
  }

  increaseQuantity(menuItemId: number): void {
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;
    const control = menuItemsFormArray.controls.find((control) => control.get('menuItemId')?.value === menuItemId);
    if (control) {
      const quantity = control.get('quantity')?.value + 1;
      control.get('quantity')?.setValue(quantity);
    }
  }

  decreaseQuantity(menuItemId: number): void {
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;
    const control = menuItemsFormArray.controls.find((control) => control.get('menuItemId')?.value === menuItemId);
    if (control) {
      const quantity = Math.max(1, control.get('quantity')?.value - 1);
      control.get('quantity')?.setValue(quantity);
    }
  }

  onQuantityChange(menuItemId: number, event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const quantity = inputElement.value ? parseInt(inputElement.value, 10) : 1;
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;

    const control = menuItemsFormArray.controls.find((control) => control.get('menuItemId')?.value === menuItemId);
    if (control) {
      control.get('quantity')?.setValue(quantity);
    }
  }

  isMenuItemSelected(menuItemId: number): boolean {
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;
    return menuItemsFormArray.controls.some((control) => control.get('menuItemId')?.value === menuItemId);
  }

  getQuantity(menuItemId: number): number {
    const menuItemsFormArray = this.orderForm.get('menuItemQuantities') as FormArray;
    const control = menuItemsFormArray.controls.find((control) => control.get('menuItemId')?.value === menuItemId);
    return control ? control.get('quantity')?.value : 1;
  }

  closeModal(): void {
    this.store.dispatch(closeCreateOrderModal());
  }

  resetForm(): void {
    const currentDate = new Date().toISOString().split('T')[0];
    this.orderForm.reset({
      userEmail: '',
      createdOn: currentDate,
      menuItemQuantities: new FormArray([]),
      paid: false,
      status: 'PENDING',
    });
  }
}
