import { createSelector } from '@ngrx/store';
import { ModalState } from './modal.reducer';

export const selectUserModalState = (state: any) => state.userModals;

export const selectIsCreateUserModalOpen = createSelector(
  selectUserModalState,
  (state: ModalState) => state.createUser
);

export const selectIsUpdateUserModalOpen = createSelector(
  selectUserModalState,
  (state: ModalState) => state.updateUser
);

export const selectIsDeleteUserModalOpen = createSelector(
  selectUserModalState,
  (state: ModalState) => state.deleteUser
);

export const selectDeleteUserId = createSelector(
  selectUserModalState,
  (state: ModalState) => state.deleteUserId
);

export const selectUserToUpdate = createSelector(
  selectUserModalState,
  (state: ModalState) => state.userToUpdate
);
