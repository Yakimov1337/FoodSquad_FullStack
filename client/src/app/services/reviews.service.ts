import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { BaseService } from './base.service';
import { Review, ReviewSubmission } from '../core/models';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ReviewsService extends BaseService {
  private readonly baseUrl = `${environment.apiUrl}/reviews`;

  private reviewCreatedSource = new BehaviorSubject<Review | null>(null);
  private reviewUpdatedSource = new BehaviorSubject<Review | null>(null);
  private reviewDeletedSource = new BehaviorSubject<number | undefined | null>(null);

  // Observable stream to be consumed by components
  reviewCreated$ = this.reviewCreatedSource.asObservable();
  reviewDeleted$ = this.reviewDeletedSource.asObservable();
  reviewUpdated$ = this.reviewUpdatedSource.asObservable();

  constructor(http: HttpClient, router: Router, toastr: ToastrService) {
    super(http, router, toastr);
  }

  // Emit event when an review is created
  reviewCreated(review: Review): void {
    this.reviewCreatedSource.next(review);
  }

  // Emit event when an review is updated
  reviewUpdated(review: Review): void {
    this.reviewUpdatedSource.next(review);
  }

  // Emit event when an review is deleted
  reviewDeleted(reviewId: number | undefined | null): void {
    this.reviewDeletedSource.next(reviewId);
  }

  createReview(reviewData: ReviewSubmission): Observable<Review> {
    return this.post<Review>(`${this.baseUrl}`, reviewData).pipe(tap((review) => this.reviewCreated(review)));
  }

  getAllReviews(page: number = 1, limit: number = 10): Observable<Review[]> {
    const params = new HttpParams().set('page', (page - 1).toString()).set('size', limit.toString());
    return this.get<Review[]>(this.baseUrl, params);
  }

  // no such an endpoint for now
  // getReviewById(reviewId: number): Observable<Review> {
  //   return this.get<Review>(`${this.baseUrl}/${reviewId}`);
  // }

  updateReview(reviewId: number, reviewData: Partial<Review>): Observable<Review> {
    return this.put<Review>(`${this.baseUrl}/${reviewId}`, reviewData).pipe(
      tap((review) => this.reviewUpdated(review)),
    );
  }

  deleteReview(reviewId: number): Observable<void> {
    return this.delete<void>(`${this.baseUrl}/${reviewId}`).pipe(tap(() => this.reviewDeleted(reviewId)));
  }

  getReviewsByUserId(userId: string): Observable<Review[]> {
    return this.get<Review[]>(`${this.baseUrl}/user/${userId}`);
  }

  getReviewsByMenuItemId(menuItemId: number): Observable<Review[]> {
    return this.get<Review[]>(`${this.baseUrl}/menu-item/${menuItemId}`);
  }
}
