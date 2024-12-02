import { createReducer, on } from '@ngrx/store';
import * as ModalActions from './modal.actions';
import { MenuItem } from '../../../models';

export interface ModalState {
  createMenuItem: boolean;
  updateMenuItem: boolean;
  deleteMenuItem: boolean;
  userReviews: boolean;
  deleteMenuItemId: number | null | undefined;
  menuItemToUpdate: MenuItem | null;
  itemId: number | null;
}

export const initialState: ModalState = {
  createMenuItem: false,
  updateMenuItem: false,
  deleteMenuItem: false,
  userReviews: false,
  deleteMenuItemId: null,
  menuItemToUpdate: null,
  itemId: null,
};

export const menuItemsModalReducer = createReducer(
  initialState,
  on(ModalActions.openCreateMenuItemModal, state => ({ ...state, createMenuItem: true })),
  on(ModalActions.closeCreateMenuItemModal, state => ({ ...state, createMenuItem: false })),
  on(ModalActions.openUpdateMenuItemModal, (state, { menuItem }) => ({ ...state, updateMenuItem: true, menuItemToUpdate: menuItem })),
  on(ModalActions.closeUpdateMenuItemModal, state => ({ ...state, updateMenuItem: false})),
  on(ModalActions.openDeleteMenuItemModal, (state, { menuItemId }) => ({ ...state, deleteMenuItem: true, deleteMenuItemId: menuItemId })),
  on(ModalActions.closeDeleteMenuItemModal, state => ({ ...state, deleteMenuItem: false})),
  on(ModalActions.openUserReviewsModal, (state, { itemId }) => ({ ...state, userReviews: true, itemId })),
  on(ModalActions.closeUserReviewsModal, (state) => ({ ...state, userReviews: false })),
);
