import { createAction, props } from '@ngrx/store';
import { User, UserUpdate } from '../../models';

export const login = createAction(
  '[Auth] Login',
  props<{ email: string; password: string }>()
);
export const loginSuccess = createAction('[Auth] Login Success', props<{ user: User }>());
export const loginFailure = createAction('[Auth] Login Failure', props<{ error: string }>());
export const logout = createAction('[Auth] Logout');
export const logoutSuccess = createAction('[Auth] Logout Success');
export const logoutFailure = createAction('[Auth] Logout Failure', props<{ error: string }>());
// Add a new action for session restoration success
export const restoreSessionSuccess = createAction('[Auth] Restore Session Success', props<{ user: User }>());

//Profile update
export const updateUserProfile = createAction(
  '[Auth] Update User Profile',
  props<{ id: string, user: UserUpdate }>()
);
export const updateUserProfileSuccess = createAction(
  '[Auth] Update User Profile Success',
  props<{ user: User }>()
);

export const updateUserProfileFailure = createAction(
  '[Auth] Update User Profile Failure',
  props<{ error: string }>()
);

export const updateCurrentUser = createAction(
  '[Auth] Update Current User',
  props<{ user: User }>()
);

