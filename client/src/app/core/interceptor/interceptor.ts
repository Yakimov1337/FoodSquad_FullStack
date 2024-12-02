import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap, finalize } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('HTTP error intercepted:', error);

        if (error.status === 401 && !this.isRefreshing) {
          this.isRefreshing = true;
          return this.authService.refreshToken().pipe(
            switchMap((newAccessToken: string) => {
              console.log('New access token obtained:', newAccessToken);
              localStorage.setItem('accessToken', newAccessToken);
              this.isRefreshing = false; // Reset the flag
              // Retry the failed request with the new token
              request = request.clone({
                setHeaders: {
                  Authorization: `Bearer ${newAccessToken}`,
                },
              });
              return next.handle(request);
            }),
            catchError((err) => {
              console.error('Error refreshing token:', err);
              this.isRefreshing = false; // Reset the flag
              if (err.status === 403 || err.status === 401) {
                this.toastr.info("Session expired. Please log in again.");
              }
              localStorage.removeItem('accessToken');
              sessionStorage.clear();
              this.router.navigate(['/auth/sign-in']);
              return throwError(() => new Error('Session expired. Please log in again.'));
            }),
            finalize(() => {
              this.isRefreshing = false; // Reset the flag in case of any error
            }),
          );
        } else {
          this.isRefreshing = false; // Reset the flag
          return throwError(() => error);
        }
      }),
    );
  }
}
