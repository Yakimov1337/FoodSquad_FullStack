import { createAction, props } from '@ngrx/store';
import { Order } from '../../../models/index';

//Orders related modals
export const openCreateOrderModal = createAction('[Modal] Open Create Order Modal');
export const closeCreateOrderModal = createAction('[Modal] Close Create Order Modal');

export const openUpdateOrderModal = createAction('[Modal] Open Update Order Modal', props<{ order: Order }>());
export const closeUpdateOrderModal = createAction('[Modal] Close Update Order Modal');

export const openDeleteOrderModal = createAction('[Modal] Open Delete Order Modal', props<{ orderId: string | undefined }>());
export const closeDeleteOrderModal = createAction('[Modal] Close Delete Order Modal');
