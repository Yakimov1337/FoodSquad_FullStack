import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { map, Observable } from 'rxjs';
import { Review, MenuItem } from '../../../../../../core/models';
import { selectReviewToUpdate } from '../../../../../../core/state/modal/review/modal.selectors';
import { closeUpdateReviewModal } from '../../../../../../core/state/modal/review/modal.actions';
import { ReviewsService } from '../../../../../../services/reviews.service';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';

@Component({
  selector: '[review-update-modal]',
  templateUrl: './review-update-modal.component.html',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class ReviewUpdateModalComponent implements OnInit {
  reviewForm: FormGroup;
  menuItems$: Observable<MenuItem[]> = new Observable<MenuItem[]>();

  private currentReviewId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private reviewsService: ReviewsService,
    private menuItemsService: MenuItemsService,
    private toastr: ToastrService,
  ) {
    this.reviewForm = this.fb.group({
      comment: ['', [Validators.required, Validators.maxLength(1000)]],
      rating: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
      menuItemId: [null, Validators.required]
    });
  }

  ngOnInit(): void {
      this.menuItems$ = this.menuItemsService.getAllMenuItems(1, 100).pipe(
        map(response => response.items)
      );
    this.store.select(selectReviewToUpdate).subscribe((review) => {
      if (review) {
        this.currentReviewId = review.id;
        this.reviewForm.patchValue({
          comment: review.comment,
          rating: review.rating,
          menuItemId: review.menuItemId
        });
      }
    });
  }

  updateReview(): void {
    if (this.reviewForm.valid && this.currentReviewId) {
      const updatedReviewData = this.reviewForm.value;
      this.reviewsService.updateReview(this.currentReviewId, updatedReviewData).subscribe({
        next: (review: Review) => {
          this.toastr.success('Review updated successfully!');
          this.closeModal();
        },
        error: (error: any) => this.toastr.error('Failed to update review!', error)
      });
    } else {
      this.reviewForm.markAllAsTouched();
    }
  }

  closeModal(): void {
    this.store.dispatch(closeUpdateReviewModal());
  }
}
