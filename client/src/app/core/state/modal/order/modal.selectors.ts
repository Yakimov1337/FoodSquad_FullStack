import { createSelector } from '@ngrx/store';
import { ModalState } from './modal.reducer';

export const selectOrderModalState = (state: any) => state.orderModals;

export const selectIsCreateOrderModalOpen = createSelector(
  selectOrderModalState,
  (state: ModalState) => state.createOrder
);

export const selectIsUpdateOrderModalOpen = createSelector(
  selectOrderModalState,
  (state: ModalState) => state.updateOrder
);

export const selectIsDeleteOrderModalOpen = createSelector(
  selectOrderModalState,
  (state: ModalState) => state.deleteOrder
);

export const selectDeleteOrderId = createSelector(
  selectOrderModalState,
  (state: ModalState) => state.deleteOrderId
);

export const selectOrderToUpdate = createSelector(
  selectOrderModalState,
  (state: ModalState) => state.orderToUpdate
);
