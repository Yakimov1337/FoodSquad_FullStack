import { createAction, props } from '@ngrx/store';
import { MenuItem } from '../../../models/index';

//MenuItems related modals
export const openCreateMenuItemModal = createAction('[Modal] Open Create MenuItem Modal');
export const closeCreateMenuItemModal = createAction('[Modal] Close Create MenuItem Modal');

export const openUpdateMenuItemModal = createAction('[Modal] Open Update MenuItem Modal', props<{ menuItem: MenuItem }>());
export const closeUpdateMenuItemModal = createAction('[Modal] Close Update MenuItem Modal');

export const openDeleteMenuItemModal = createAction('[Modal] Open Delete MenuItem Modal', props<{ menuItemId: number | undefined }>());
export const closeDeleteMenuItemModal = createAction('[Modal] Close Delete MenuItem Modal');

export const openUserReviewsModal = createAction('[Modal] Open User Reviews Modal', props<{ itemId: number  }>());
export const closeUserReviewsModal = createAction('[Modal] Close User Reviews Modal');
