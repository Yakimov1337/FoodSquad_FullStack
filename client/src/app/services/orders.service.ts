import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { BaseService } from './base.service';
import { Order, OrderSubmission } from '../core/models';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrdersService extends BaseService {
  private readonly baseUrl = `${environment.apiUrl}/orders`;

  private orderCreatedSource = new BehaviorSubject<Order | null>(null);
  private orderUpdatedSource = new BehaviorSubject<Order | null>(null);
  private orderDeletedSource = new BehaviorSubject<string | undefined | null>(null);

  // Observable stream to be consumed by components
  orderCreated$ = this.orderCreatedSource.asObservable();
  orderDeleted$ = this.orderDeletedSource.asObservable();
  orderUpdated$ = this.orderUpdatedSource.asObservable();

  constructor(http: HttpClient, router: Router, toastr: ToastrService) {
    super(http, router, toastr);
  }

  // Emit event when an order is created
  orderCreated(order: Order): void {
    this.orderCreatedSource.next(order);
  }

  // Emit event when an order is updated
  orderUpdated(order: Order): void {
    this.orderUpdatedSource.next(order);
  }

  // Emit event when an order is deleted
  orderDeleted(orderId: string | undefined | null): void {
    this.orderDeletedSource.next(orderId);
  }

  createOrder(orderData: OrderSubmission): Observable<Order> {
    return this.post<Order>(`${this.baseUrl}`, orderData).pipe(
      tap(order => this.orderCreated(order))
    );
  }

  getAllOrders(page: number = 1, limit: number = 10): Observable<Order[]> {
    const params = new HttpParams().set('page', (page - 1).toString()).set('size', limit.toString());
    return this.get<Order[]>(this.baseUrl, params);
  }

  getOrderById(orderId: string): Observable<Order> {
    return this.get<Order>(`${this.baseUrl}/${orderId}`);
  }

  updateOrder(orderId: string, orderData: Partial<Order>): Observable<Order> {
    return this.put<Order>(`${this.baseUrl}/${orderId}`, orderData).pipe(
      tap(order => this.orderUpdated(order))
    );
  }

  deleteOrder(orderId: string): Observable<void> {
    return this.delete<void>(`${this.baseUrl}/${orderId}`).pipe(
      tap(() => this.orderDeleted(orderId))
    );
  }

  getOrdersByUserId(userId: string): Observable<Order[]> {
    return this.get<Order[]>(`${this.baseUrl}/user/${userId}`);
  }

}
