import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import * as AuthActions from '../core/state/auth/auth.actions';
import { AuthService } from './auth.service';
import { selectIsAuthenticated } from '../core/state/auth/auth.selectors';
import { map, Observable, of, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  constructor(private store: Store, private authService: AuthService) {}
  initializeAuthState(): Promise<void> {
    return new Promise((resolve) => {
      // Only fetch current user if there's a token in localStorage
      const accessToken = localStorage.getItem('accessToken');
      if (accessToken) {
        this.authService.getCurrentUser().subscribe({
          next: (user) => {
            this.store.dispatch(AuthActions.restoreSessionSuccess({ user }));
            resolve();
          },
          error: () => {
            localStorage.removeItem('accessToken');
            resolve();
          },
        });
      } else {
        resolve();
      }
    });
  }

  isAuthenticated(): Observable<boolean> {
    const accessToken = localStorage.getItem('accessToken');
    return of(!!accessToken);
  }

  hasAnyRole(expectedRoles: string[]): Observable<boolean> {
    return this.isAuthenticated().pipe(
      switchMap((isAuthenticated) => {
        if (!isAuthenticated) {
          return of(false);
        }
        return this.authService.getCurrentUser().pipe(map((user) => expectedRoles.includes(user.role)));
      }),
    );
  }
}
