import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Review, User } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { Store, select } from '@ngrx/store';
import { openDeleteReviewModal, openUpdateReviewModal } from '../../../../../../core/state/modal/review/modal.actions';
import { Observable } from 'rxjs';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';

@Component({
  selector: '[reviews-table-item]',
  templateUrl: './reviews-table-item.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, CurrencyPipe, ButtonComponent, CommonModule],
})
export class ReviewsTableItemComponent implements OnInit {
  @Input() review: Review = <Review>{};
  currentUser$: Observable<User | null>;

  constructor(private store: Store) {
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }

  ngOnInit(): void {}

  openUpdateModal() {
    console.log(this.review)
    this.store.dispatch(openUpdateReviewModal({ review: this.review }));
  }

  openDeleteModal() {
    this.store.dispatch(openDeleteReviewModal({ reviewId: this.review.id }));
  }

  getTone(rating: number): 'primary' | 'danger' | 'success' | 'warning' | 'info' | 'light' {
    switch (rating) {
      case 1:
        return 'danger'; // Red color for poor reviews
      case 2:
        return 'warning'; // Orange color for below-average reviews
      case 3:
        return 'info'; // Blue color for average reviews
      case 4:
        return 'success'; // Green color for good reviews
      case 5:
        return 'success'; // Blue color for excellent reviews
      default:
        return 'light'; // Default tone
    }
  }
}
