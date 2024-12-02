import { createReducer, on } from '@ngrx/store';
import * as ModalActions from './modal.actions';
import { User } from '../../../models';

export interface ModalState {
  createUser: boolean;
  updateUser: boolean;
  deleteUser: boolean;
  deleteUserId: string | null | undefined;
  userToUpdate: User | null;
}

export const initialState: ModalState = {
  createUser: false,
  updateUser: false,
  deleteUser: false,
  deleteUserId: null,
  userToUpdate: null,
};

export const userModalReducer = createReducer(
  initialState,
  on(ModalActions.openCreateUserModal, state => ({ ...state, createUser: true })),
  on(ModalActions.closeCreateUserModal, state => ({ ...state, createUser: false })),
  on(ModalActions.openUpdateUserModal, (state, { user }) => ({ ...state, updateUser: true, userToUpdate: user })),
  on(ModalActions.closeUpdateUserModal, state => ({ ...state, updateUser: false})),
  on(ModalActions.openDeleteUserModal, (state, { userId }) => ({ ...state, deleteUser: true, deleteUserId: userId })),
  on(ModalActions.closeDeleteUserModal, state => ({ ...state, deleteUser: false}))
);
