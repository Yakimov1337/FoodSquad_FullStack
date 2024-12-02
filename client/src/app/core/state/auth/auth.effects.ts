import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { catchError, map, mergeMap, switchMap, tap } from 'rxjs/operators';
import * as AuthActions from './auth.actions';
import { AuthService } from '../../../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { UserService } from '../../../services/user.service';
import { UserUpdate } from '../../models';

@Injectable()
export class AuthEffects {
  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private router: Router,
    private userService: UserService,
    private store: Store,
    private toastr: ToastrService,
  ) {}

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      mergeMap((action) =>
        this.authService.signInAccount(action.email, action.password).pipe(
          map((user) => AuthActions.loginSuccess({ user })),
          catchError((error) => of(AuthActions.loginFailure({ error: error.message }))),
        ),
      ),
    ),
  );

  loginSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginSuccess),
        tap(({ user }) => {
          this.toastr.success(`Successfully logged in!`);
          if (user.role === 'Admin' || user.role === 'Moderator') {
            this.router.navigate(['/admin/dashboard']);
          } else {
            this.router.navigate(['/']);
          }
        }),
      ),
    { dispatch: false },
  );

  loginFailure$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginFailure),
        tap(({ error }) => {
          this.toastr.error('Login failed: ' + error);
        }),
      ),
    { dispatch: false },
  );

  updateUserProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.updateUserProfile),
      switchMap((action) =>
        this.userService.updateUser(action.id, action.user).pipe(
          map((updatedUser) => AuthActions.updateUserProfileSuccess({ user: updatedUser })),
          catchError((error) => of(AuthActions.updateUserProfileFailure({ error: error.message }))),
        ),
      ),
    ),
  );

  updateUserProfileSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.updateUserProfileSuccess),
      tap(({ user }) => {
        this.toastr.success('User information updated successfully');
      }),
    ),
    { dispatch: false }
  );

  updateUserProfileFailure$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.updateUserProfileFailure),
      tap(({ error }) => {
        this.toastr.error('Failed to update user information: ' + error);
      })
    ),
    { dispatch: false }
  );


  restoreSessionSuccess$ = createEffect(() => this.actions$.pipe(ofType(AuthActions.restoreSessionSuccess)), {
    dispatch: false,
  });

  logout$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logout),
        switchMap(() =>
          this.authService.logout().pipe(
            tap(() => {
              this.toastr.success('Successfully logged out!');
              this.router.navigate(['/auth/sign-in']);
            }),
            catchError((error) => {
              this.toastr.error('Logout failed: ' + error.message);
              return of(AuthActions.logoutFailure({ error: error.message }));
            })
          )
        )
      ),
    { dispatch: false }
  );
}
