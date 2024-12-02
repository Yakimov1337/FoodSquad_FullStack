import { Subscription } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { selectDeleteReviewId } from '../../../../../../core/state/modal/review/modal.selectors';
import { closeDeleteReviewModal } from '../../../../../../core/state/modal/review/modal.actions';
import { ReviewsService } from '../../../../../../services/reviews.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[review-delete-modal]',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-delete-modal.component.html',
})
export class ReviewDeleteModalComponent {
  reviewIdToDelete: number | undefined | null = null;
  private subscription = new Subscription();

  constructor(private store: Store, private reviewsService: ReviewsService, private toastr: ToastrService) {
    this.subscription.add(
      this.store.select(selectDeleteReviewId).subscribe((reviewId) => {
        this.reviewIdToDelete = reviewId;
      }),
    );
  }
  ngOnInit(): void {
    console.log(this.reviewIdToDelete);
  }

  deleteReview(): void {
    if (!this.reviewIdToDelete) {
      this.toastr.error('No review ID to delete provided!');
      return;
    }
    this.reviewsService.deleteReview(this.reviewIdToDelete).subscribe({
      next: () => {
        this.closeModal();
        this.reviewsService.reviewDeleted(this.reviewIdToDelete); //Notify about deletion
        this.toastr.success(`review with ID ${this.reviewIdToDelete} deleted successfully!`);
      },
      error: (error) => this.toastr.error('Error deleting review:', error)
    });
  }

  closeModal(): void {
    this.store.dispatch(closeDeleteReviewModal());
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
