import { createSelector } from '@ngrx/store';
import { ModalState } from './modal.reducer';

export const selectReviewModalState = (state: any) => state.reviewModals;

export const selectIsCreateReviewModalOpen = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.createReview
);

export const selectIsCreateReviewUserModalOpen = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.createUserReview
);

export const selectIsUserReviewsModalOpen = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.readReviews
);

export const selectIsUpdateReviewModalOpen = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.updateReview
);

export const selectIsDeleteReviewModalOpen = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.deleteReview
);

export const selectDeleteReviewId = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.deleteReviewId
);

export const selectReviewToUpdate = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.reviewToUpdate
);

export const selectReviewItemId = createSelector(
  selectReviewModalState,
  (state: ModalState) => state.itemId
);
