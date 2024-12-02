import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { map } from 'rxjs/operators';
import { AuthStateService } from '../../services/auth-state.service';

export const isAuthenticatedGuard: CanActivateFn = () => {
  const authService = inject(AuthStateService);
  const router = inject(Router);
  return authService.isAuthenticated().pipe(
    map(isAuthenticated => {
      if (!isAuthenticated) {
        // Redirect non-authenticated users to sign-in
        return router.parseUrl('/auth/sign-in');
      }
      return true;
    })
  );
};

// Functional guard for checking allowed roles
export const isAllowedRoleGuard: CanActivateFn = () => {
  const authService = inject(AuthStateService);
  const router = inject(Router);
  return authService.hasAnyRole(['Admin', 'Moderator']).pipe(
    map(hasRole => {
      if (!hasRole) {
        // Redirect to unauthorized if not having an allowed role
        return router.parseUrl('/unauthorized');
      }
      return true;
    })
  );
};

// isUnauthenticatedGuard: Redirects authenticated users away from auth pages
export const isUnauthenticatedGuard: CanActivateFn = () => {
  const authService = inject(AuthStateService);
  const router = inject(Router);
  return authService.isAuthenticated().pipe(
    map(isAuthenticated => {
      if (!isAuthenticated) {
        return true;
      } else {
        return router.parseUrl('/');
      }
    })
  );
};


