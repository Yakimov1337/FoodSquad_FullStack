import { createAction, props } from '@ngrx/store';
import { Review } from '../../../models/index';

//Reviews related modals
export const openCreateReviewModal = createAction('[Modal] Open Create Review Modal');
export const closeCreateReviewModal = createAction('[Modal] Close Create Review Modal');

export const openUpdateReviewModal = createAction('[Modal] Open Update Review Modal', props<{ review: Review }>());
export const closeUpdateReviewModal = createAction('[Modal] Close Update Review Modal');

export const openDeleteReviewModal = createAction('[Modal] Open Delete Review Modal', props<{ reviewId: number | undefined }>());
export const closeDeleteReviewModal = createAction('[Modal] Close Delete Review Modal');

// for users (not admin dashboard)
export const openCreateReviewUserModal = createAction('[Modal] Open Create Review User Modal',props<{ itemId: number  }>());
export const closeCreateReviewUserModal = createAction('[Modal] Close Create Review User Modal');

export const openUsersReviewModal = createAction('[Modal] Open Users Review Modal',props<{ itemId: number  }>());
export const closeUsersReviewModal = createAction('[Modal] Close Users Review Modal');
