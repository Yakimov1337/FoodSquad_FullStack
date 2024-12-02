import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Role, User, UserUpdate } from '../core/models';
import { BaseService } from './base.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserService extends BaseService {
  private readonly baseUrl = `${environment.apiUrl}/users`;

  private userCreatedSource = new BehaviorSubject<User | null>(null);
  private userUpdatedSource = new BehaviorSubject<User | null>(null);
  private userDeletedSource = new BehaviorSubject<string | undefined | null>(null);

  // Observable stream to be consumed by components
  userCreated$ = this.userCreatedSource.asObservable();
  userUpdated$ = this.userUpdatedSource.asObservable();
  userDeleted$ = this.userDeletedSource.asObservable();

  constructor(http: HttpClient, router: Router, toastr: ToastrService) {
    super(http, router, toastr);
  }

  // Emit event when a user is created
  userCreated(user: User): void {
    this.userCreatedSource.next(user);
  }

  // Emit event when a user is updated
  userUpdated(user: User): void {
    this.userUpdatedSource.next(user);
  }

  // Emit event when a user is deleted
  userDeleted(userId: string | undefined | null): void {
    this.userDeletedSource.next(userId);
  }

  getAllUsers(page: number = 1, limit: number = 10): Observable<User[]> {
    const params = new HttpParams().set('page', (page - 1).toString()).set('limit', limit.toString());
    return this.get<User[]>(this.baseUrl, params).pipe(
      map(users => users.map(this.mapUser.bind(this)))
    );
  }

  createUser(userData: User): Observable<User> {
    return this.post<User>(`${this.baseUrl}`, userData).pipe(
      tap(user => this.userCreated(user))
    );
  }

  getUserById(userId: string): Observable<User> {
    return this.get<User>(`${this.baseUrl}/${userId}`).pipe(
      map(this.mapUser.bind(this))
    );
  }

  updateUser(userId: string, userData: UserUpdate): Observable<User> {
    return this.put<User>(`${this.baseUrl}/${userId}`, userData).pipe(
      map(this.mapUser.bind(this)),
      tap(result => this.userUpdated(result))
    );
  }


  deleteUser(userId: string): Observable<void> {
    return this.delete<void>(`${this.baseUrl}/${userId}`).pipe(
      tap(() => this.userDeleted(userId))
    );
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

  private mapUser(user: User): User {
    return {
      ...user,
      role: this.mapRole(user.role),
    };
  }
}
