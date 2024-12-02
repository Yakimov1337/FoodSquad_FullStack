import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from '../../models';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectIsLoading = createSelector(
  selectAuthState,
  (state: AuthState) => state.isLoading
);

export const selectCurrentUser = createSelector(
  selectAuthState,
  (state: AuthState) => state.user
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state: AuthState) => state.error
);

export const selectIsAuthenticated = createSelector(
  selectCurrentUser,
  (user) => !!user
);

export const selectIsUpdatingInfo = createSelector(
  selectAuthState,
  (state: AuthState) => state.isUpdatingInfo 
);



