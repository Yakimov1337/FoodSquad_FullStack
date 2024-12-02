import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  selectIsUserReviewsModalOpen,
  selectReviewItemId,
} from '../../../../../core/state/modal/review/modal.selectors';
import { closeUsersReviewModal } from '../../../../../core/state/modal/review/modal.actions';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Subscription } from 'rxjs';
import { ReviewsService } from '../../../../../services/reviews.service';
import { Review } from '../../../../../core/models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-reviews-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './user-reviews-modal.component.html',
  animations: [
    trigger('fadeInOut', [
      state('in', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('0.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class UserReviewsModalComponent implements OnInit, OnDestroy {
  showUserReviewsModal$ = this.store.select(selectIsUserReviewsModalOpen);
  reviews: Review[] = [];
  private subscriptions: Subscription = new Subscription();

  constructor(private store: Store, private reviewsService: ReviewsService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.subscriptions.add(
      this.store.select(selectReviewItemId).subscribe((menuItemId) => {
        if (menuItemId) {
          this.fetchReviews(menuItemId);
        }
      }),
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  fetchReviews(menuItemId: number): void {
    this.reviewsService.getReviewsByMenuItemId(menuItemId).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
      },
      error: (error: any) => {
        this.toastr.error('Failed to fetch reviews', error);
      },
    });
  }

  closeModal(): void {
    this.store.dispatch(closeUsersReviewModal());
  }
}
