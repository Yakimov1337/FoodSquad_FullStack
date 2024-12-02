import { createReducer, on } from '@ngrx/store';
import * as AuthActions from './auth.actions';
import { AuthState } from '../../models';

export const initialState: AuthState = {
  isAuthenticated: false,
  user: null,
  error: null,
  isLoading: false,
  isUpdatingInfo: false
};
// In auth.reducer.ts
export const authReducer = createReducer(
  initialState,
  on(AuthActions.login, (state) => {
    console.log("Reducer handling login", state); // Debugging log
    return { ...state, isLoading: true };
  }),
  on(AuthActions.loginSuccess, (state, { user }) => {
    console.log("Reducer handling loginSuccess", user); // Debugging log
    return { ...state, isAuthenticated: true, user, error: null, isLoading: false };
  }),
  on(AuthActions.restoreSessionSuccess, (state, { user }) => {
    console.log("Reducer handling restore session", user); // Debugging log
    return { ...state, isAuthenticated: true, user, error: null, isLoading: false };
  }),
  on(AuthActions.loginFailure, (state, { error }) => {
    console.log("Reducer handling loginFailure", error); // Debugging log
    return { ...state, error, isLoading: false };
  }),
  on(AuthActions.logout, () => {
    console.log("Reducer handling logout"); // Debugging log
    return initialState;
  }),
  on(AuthActions.updateUserProfile, (state) => ({
    ...state,
    isUpdatingInfo: true,
    error: null,
  })),
  on(AuthActions.updateUserProfileSuccess, (state, { user }) => ({
    ...state,
    user,
    isUpdatingInfo: false,
    error: null,
  })),
  on(AuthActions.updateUserProfileFailure, (state, { error }) => ({
    ...state,
    isUpdatingInfo: false,
    error,
  }))
);
