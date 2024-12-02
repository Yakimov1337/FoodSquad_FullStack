import { createReducer, on } from '@ngrx/store';
import * as ModalActions from './modal.actions';
import { Order } from '../../../models';

export interface ModalState {
  createOrder: boolean;
  updateOrder: boolean;
  deleteOrder: boolean;
  deleteOrderId: string | null | undefined;
  orderToUpdate: Order | null;
}

export const initialState: ModalState = {
  createOrder: false,
  updateOrder: false,
  deleteOrder: false,
  deleteOrderId: null,
  orderToUpdate: null,
};

export const orderModalReducer = createReducer(
  initialState,
  on(ModalActions.openCreateOrderModal, state => ({ ...state, createOrder: true })),
  on(ModalActions.closeCreateOrderModal, state => ({ ...state, createOrder: false })),
  on(ModalActions.openUpdateOrderModal, (state, { order }) => ({ ...state, updateOrder: true, orderToUpdate: order })),
  on(ModalActions.closeUpdateOrderModal, state => ({ ...state, updateOrder: false})),
  on(ModalActions.openDeleteOrderModal, (state, { orderId }) => ({ ...state, deleteOrder: true, deleteOrderId: orderId })),
  on(ModalActions.closeDeleteOrderModal, state => ({ ...state, deleteOrder: false}))
);
