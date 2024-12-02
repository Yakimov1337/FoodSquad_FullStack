import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { User, Role } from '../core/models/user.model';
import { environment } from '../../environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/auth`;
  private readonly tokenUrl = `${environment.apiUrl}/token`;

  constructor(private http: HttpClient, private router: Router) {}

  createUserAccount(user: { email: string; password: string; confirmPassword: string }): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/sign-up`, user).pipe(
      tap(() => {
        this.router.navigate(['/auth/sign-in']);
      }),
      catchError((error) => {
        return throwError(() => error.error.error);
      }),
    );
  }

  signInAccount(email: string, password: string): Observable<User> {
    return this.http.post<{ accessToken: string }>(`${this.baseUrl}/sign-in`, { email, password }, { withCredentials: true }).pipe(
      map((response) => {
        const accessToken = response.accessToken;
        localStorage.setItem('accessToken', accessToken);
        return this.decodeToken(accessToken);
      }),
      catchError((error) => {
        return throwError(() => error.error.error);
      }),
    );
  }

  refreshToken(): Observable<string> {
    return this.http.post<{ accessToken: string }>(`${this.tokenUrl}/refresh-token`, {}, { withCredentials: true }).pipe(
      tap((response) => {
        localStorage.setItem('accessToken', response.accessToken);
      }),
      map((response) => response.accessToken),
      catchError((error) => {
        localStorage.removeItem('accessToken');
        return throwError(() => error);
      }),
    );
  }

  logout(): Observable<any> {
    const accessToken = localStorage.getItem('accessToken');
    return this.http.post(
      `${this.baseUrl}/logout`,
      {},
      {
        headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : {},
        withCredentials: true
      }
    ).pipe(
      tap(() => {
        localStorage.removeItem('accessToken');
        sessionStorage.clear();
      })
    );
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/current-user`).pipe(
      map((user: User) => ({
        ...user,
        // role: this.mapRole(user.role as unknown as string) // Ensure role is mapped correctly
      })),
      catchError((error) => {
        return throwError(() => error.error.error);
      }),
    );
  }

  private decodeToken(token: string): User {
    const decodedToken: any = jwtDecode(token);
    return {
      id: decodedToken.id,
      name: decodedToken.name,
      email: decodedToken.email,
      role: decodedToken.role,
      imageUrl: decodedToken.imageUrl,
      phoneNumber: decodedToken.phoneNumber,
    };
  }

  private mapRole(role: string): Role {
    switch (role) {
      case 'ADMIN':
        return Role.Admin;
      case 'MODERATOR':
        return Role.Moderator;
      case 'NORMAL':
        return Role.Normal;
      case 'SUPER_ADMIN':
        return Role.SuperAdmin;
      default:
        return Role.Normal;
    }
  }
}
