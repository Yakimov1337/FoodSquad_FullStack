import { Store } from '@ngrx/store';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewsTableComponent } from '../../components/reviews/reviews-table/reviews-table.component';
import { selectIsCreateReviewModalOpen, selectIsDeleteReviewModalOpen, selectIsUpdateReviewModalOpen } from '../../../../../core/state/modal/review/modal.selectors';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { ReviewDeleteModalComponent } from '../../components/reviews/review-delete-modal/review-delete-modal.component';
import { ReviewUpdateModalComponent } from '../../components/reviews/review-update-modal/review-update-modal.component';
import { ReviewCreateModalComponent } from '../../components/reviews/review-create-modal/review-create-modal.component';
@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [
    CommonModule,
    ReviewsTableComponent,
    ReviewUpdateModalComponent,
    ReviewCreateModalComponent,
    ReviewDeleteModalComponent,
  ],
  templateUrl: './reviews.component.html',
  animations: [
    trigger('slideInRight', [
      transition(':enter', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateX(0)', opacity: 1 })),
      ]),
    ]),
    trigger('fadeInOut', [
      state('in', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('0.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class ReviewsComponent {
  showUpdateReviewModal$ = this.store.select(selectIsUpdateReviewModalOpen);
  showCreateReviewModal$ = this.store.select(selectIsCreateReviewModalOpen);
  showDeleteReviewModal$ = this.store.select(selectIsDeleteReviewModalOpen);


  constructor(private store: Store) {}
}
