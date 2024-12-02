import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { map, switchMap, tap, withLatestFrom } from 'rxjs/operators';
import { addItem, removeItem, clearCart, loadCartFromStorage } from './cart.actions';
import { selectCartItems } from './cart.selectors';

@Injectable()
export class CartEffects {
  constructor(private actions$: Actions, private store: Store) {}

  saveCartToStorage$ = createEffect(() => this.actions$.pipe(
    ofType(addItem, removeItem, clearCart), // Triggered by these actions
    withLatestFrom(this.store.select(selectCartItems)),
    tap(([action, items]) => {
      localStorage.setItem('cart', JSON.stringify(items));
    })
  ), { dispatch: false });
}
