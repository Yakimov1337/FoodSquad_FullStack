import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import {
  selectIsCreateReviewUserModalOpen,
  selectReviewItemId,
} from '../../../../../core/state/modal/review/modal.selectors';
import { closeCreateReviewUserModal } from '../../../../../core/state/modal/review/modal.actions';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Observable } from 'rxjs';
import { ReviewsService } from '../../../../../services/reviews.service';
import { ToastrService } from 'ngx-toastr';
import { MenuItemsService } from '../../../../../services/menuItems.service';

@Component({
  selector: 'app-submit-review-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './submit-review-modal.component.html',
  animations: [
    trigger('fadeInOut', [
      state('in', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('0.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class SubmitUserReviewModal {
  showReviewModal$ = this.store.select(selectIsCreateReviewUserModalOpen);
  selectedRating: number = 0;
  hoveredRating: number = 0;
  userComment: string = '';
  itemId$: Observable<number | null>;

  constructor(
    private store: Store,
    private toastr: ToastrService,
    private reviewsService: ReviewsService,
    private menuItemsService: MenuItemsService,
  ) {
    this.itemId$ = this.store.select(selectReviewItemId);
  }

  setRating(rating: number): void {
    this.selectedRating = rating;
  }

  hoverRating(rating: number): void {
    this.hoveredRating = rating;
  }

  submitReview(): void {
    if (this.selectedRating > 0 && this.userComment.trim()) {
      this.itemId$.subscribe((itemId) => {
        if (itemId !== null) {
          const reviewData = {
            rating: this.selectedRating,
            comment: this.userComment,
            menuItemId: itemId,
          };

          this.reviewsService.createReview(reviewData).subscribe({
            next: () => {
              this.toastr.success('Review submitted successfully');
              this.menuItemsService.notifyReviewSubmitted();
              this.closeModal();
            },
            error: (error: any) => {
              this.toastr.error('Failed to submit review', error);
            },
          });
        }
      });
    } else {
      this.toastr.error('Please provide a rating and a comment.');
    }
  }

  closeModal(): void {
    this.store.dispatch(closeCreateReviewUserModal());
  }
}
