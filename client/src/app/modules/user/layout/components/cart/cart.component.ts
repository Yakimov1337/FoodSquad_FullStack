import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { Observable, map, take } from 'rxjs';
import { MenuItem, OrderSubmission, User } from '../../../../../core/models';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { selectCartItems } from '../../../../../core/state/shopping-cart/cart.selectors';
import { CartVisibilityService } from '../../../../../services/cart-visibility.service';
import { CommonModule } from '@angular/common';
import { OrdersService } from '../../../../../services/orders.service';
import { selectCurrentUser } from '../../../../../core/state/auth/auth.selectors';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { LoaderComponent } from '../../../../../shared/components/loader/loader.component';
import { clearCart, removeItem } from '../../../../../core/state/shopping-cart/cart.actions';

interface CartItem extends MenuItem {
  quantity: number;
}

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  standalone: true,
  imports: [CommonModule, LoaderComponent],
  animations: [
    trigger('slideInOut', [
      state('in', style({ transform: 'translateX(0%)' })),
      state('out', style({ transform: 'translateX(100%)' })),
      transition('in <=> out', animate('400ms ease-in-out')),
    ]),
    trigger('backdropFade', [
      state('visible', style({ opacity: 1 })),
      state('hidden', style({ opacity: 0 })),
      transition('visible <=> hidden', animate('1s ease-in-out')),
    ]),
  ],
})
export class CartComponent implements OnInit {
  currentUser$: Observable<User | null>;
  cartItems: CartItem[] = [];
  totalPrice: number = 0;
  showCart: boolean = false;
  isLoading: boolean = false;

  constructor(
    private store: Store,
    public cartVisibilityService: CartVisibilityService,
    private ordersService: OrdersService,
    private toastr: ToastrService,
    private router: Router
  ) {
    // Workaround for quantity issues, this ensures proper total order value
    // Subscribe to the cart items
    this.store
      .select(selectCartItems)
      .pipe(
        map((items) => {
          // For each item from the store, checking if it's already in local cart
          return items.map((storeItem) => {
            const existingItem = this.cartItems.find((item) => item.id === storeItem.id);
            return {
              ...storeItem,
              // If it exists, keep its current quantity. If not, set quantity to 1.
              quantity: existingItem ? existingItem.quantity : 1,
            };
          }) as CartItem[]; // Cast the result as an array of CartItem
        }),
      )
      .subscribe((items) => {
        this.cartItems = items; // Update local cart items
        this.calculateTotalPrice(); // Recalculate the total price
      });
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }
  ngOnInit(): void {
    this.cartVisibilityService.showCart$.subscribe((visible) => {
      this.showCart = visible;
    });
  }

  increaseQuantity(cartItem: CartItem): void {
    const item = this.cartItems.find((i) => i.id === cartItem.id);
    if (item) {
      item.quantity += 1;
      this.calculateTotalPrice();
    }
  }

  decreaseQuantity(cartItem: CartItem): void {
    if (cartItem.quantity > 1) {
      cartItem.quantity -= 1;
      this.calculateTotalPrice();
    }
  }

  calculateTotalPrice(): void {
    this.totalPrice = this.cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
  }

  onQuantityChange(event: Event, cartItem: CartItem): void {
    const inputElement = event.target as HTMLInputElement;
    const quantity = inputElement.valueAsNumber;
    if (quantity && quantity > 0) {
      cartItem.quantity = quantity;
      this.calculateTotalPrice();
    }
  }

  placeOrder(): void {
    this.currentUser$.pipe(take(1)).subscribe((currentUser) => {
      if (!currentUser || typeof currentUser.id !== 'string') {
        this.toastr.error('User information is missing');
        this.isLoading = false; // Reset loading state in case of an error
        return;
      }

      this.isLoading = true;

      // Constructing order data with IDs instead of full objects
      const orderData: OrderSubmission = {
        userEmail: currentUser.email,
        menuItemQuantities: this.cartItems.reduce((acc, item) => {
          acc[item.id] = item.quantity;
          return acc;
        }, {} as { [menuItemId: number]: number }),
        createdOn: new Date().toISOString(),
        paid: false,
        status: 'PENDING',
      };

      this.ordersService.createOrder(orderData).subscribe({
        next: (order) => {
          this.toastr.success('Order placed successfully!');
          this.store.dispatch(clearCart());
          this.cartVisibilityService.toggleCart();
          this.router.navigate(['/orders']);
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Failed to place an order', error);
          this.toastr.error('Failed to place an order');
          this.isLoading = false;
        },
      });
    });
  }

  removeItem(itemId: number): void {
    this.store.dispatch(removeItem({ itemId }));
  }
}
