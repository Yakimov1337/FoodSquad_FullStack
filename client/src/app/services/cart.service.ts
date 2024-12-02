import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { addItem, removeItem, clearCart, loadCartFromStorage } from '../core/state/shopping-cart/cart.actions';
import { MenuItem } from '../core/models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  constructor(private store: Store) {}

  addItem(item: MenuItem) {
    this.store.dispatch(addItem({ item }));
  }

  removeItem(itemId: number) {
    this.store.dispatch(removeItem({ itemId }));
  }

  clearCart() {
    this.store.dispatch(clearCart());
  }

  loadCartFromStorage() {
    const items = JSON.parse(localStorage.getItem('cart') || '[]');
    this.store.dispatch(loadCartFromStorage({ items }));
  }
}
