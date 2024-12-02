import { createReducer, on } from '@ngrx/store';
import * as ModalActions from './modal.actions';
import { Review } from '../../../models';

export interface ModalState {
  createReview: boolean;
  createUserReview: boolean;
  readReviews: boolean;
  updateReview: boolean;
  deleteReview: boolean;
  deleteReviewId: number | null | undefined;
  reviewToUpdate: Review | null;
  itemId: number | null;
}

export const initialState: ModalState = {
  createReview: false,
  createUserReview: false,
  readReviews: false,
  updateReview: false,
  deleteReview: false,
  deleteReviewId: null,
  reviewToUpdate: null,
  itemId: null,
};

export const ReviewModalReducer = createReducer(
  initialState,
  //ADMIN PANEL CRUD MODALS
  on(ModalActions.openCreateReviewModal, (state) => ({ ...state, createReview: true })),
  on(ModalActions.closeCreateReviewModal, (state) => ({ ...state, createReview: false })),
  on(ModalActions.openUpdateReviewModal, (state, { review }) => ({
    ...state,
    updateReview: true,
    reviewToUpdate: review,
  })),
  on(ModalActions.closeUpdateReviewModal, (state) => ({ ...state, updateReview: false })),
  on(ModalActions.openDeleteReviewModal, (state, { reviewId }) => ({
    ...state,
    deleteReview: true,
    deleteReviewId: reviewId,
  })),
  on(ModalActions.closeDeleteReviewModal, (state) => ({ ...state, deleteReview: false })),
  //USER MODALS IN FOOD MENU
  on(ModalActions.openUsersReviewModal, (state, { itemId }) => ({ ...state, readReviews: true, itemId })),
  on(ModalActions.closeUsersReviewModal, (state) => ({ ...state, readReviews: false })),
  on(ModalActions.openCreateReviewUserModal, (state, { itemId }) => ({ ...state, createUserReview: true, itemId })),
  on(ModalActions.closeCreateReviewUserModal, (state) => ({ ...state, createUserReview: false })),
);
