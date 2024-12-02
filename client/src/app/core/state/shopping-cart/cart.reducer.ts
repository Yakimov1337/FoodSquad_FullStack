// cart.reducer.ts
import { createReducer, on } from '@ngrx/store';
import { addItem, removeItem, clearCart } from './cart.actions';
import { MenuItem } from '../../models';

export interface CartState {
  items: MenuItem[];
}

// Load initial cart items from localStorage, or default to an empty array
const initialCartItems = JSON.parse(localStorage.getItem('cart') || '[]');

export const initialCartState: CartState = {
  items: initialCartItems,
};

export const cartReducer = createReducer(
  initialCartState,
  on(addItem, (state, { item }) => ({ ...state, items: [...state.items, item] })),
  on(removeItem, (state, { itemId }) => ({ ...state, items: state.items.filter(item => item.id !== itemId) })),
  on(clearCart, state => ({ ...state, items: [] })),
);
