import { Observable, Subscription, finalize, interval, startWith, switchMap, timer } from 'rxjs';
import { Store } from '@ngrx/store';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { Review } from '../../../../../../core/models';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';
import { ReviewsService } from '../../../../../../services/reviews.service';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openCreateReviewModal } from '../../../../../../core/state/modal/review/modal.actions';
import { ReviewCreateModalComponent } from '../review-create-modal/review-create-modal.component';
import { selectIsCreateReviewModalOpen } from '../../../../../../core/state/modal/review/modal.selectors';
import { ToastrService } from 'ngx-toastr';
import { ReviewsTableItemComponent } from '../reviews-table-item/reviews-table-item.component';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';

@Component({
  selector: '[reviews-table]',
  templateUrl: './reviews-table.component.html',
  standalone: true,
  imports: [
    NgFor,
    ReviewsTableItemComponent,
    ReviewCreateModalComponent,
    CommonModule,
    LoaderComponent,
    AngularSvgIconModule,
    ButtonComponent,
    PaginationComponent,
  ],
})
export class ReviewsTableComponent implements OnInit {
  public reviews: Review[] = [];
  public isLoading: boolean = true;
  public currentPage = 1;
  public totalPages!: number;
  public timeSinceLastUpdate$!: Observable<number>;
  public lastUpdated: Date = new Date();

  private subscriptions: Subscription = new Subscription();

  constructor(private reviewsService: ReviewsService, private store: Store, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.loadReviews(this.currentPage);
    this.initializeSubscriptions();
    this.initializeTimeSinceLastUpdate();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadReviews(page: number, limit: number = 10): void {
    this.isLoading = true;

    // Start fetching reviews
    this.reviewsService.getAllReviews(page, limit).pipe(
      switchMap((reviews) => {
        // Set reviews and calculate total pages
        this.reviews = reviews;
        this.totalPages = Math.ceil(200 / limit); // Update as necessary

        // Return an observable that emits after 1 second
        return timer(300);
      }),
      finalize(() => {
        this.isLoading = false; // This will run after the 1-second timer
      })
    ).subscribe();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadReviews(this.currentPage);
  }

  openCreateModal() {
    console.log('Dispatching openCreateReviewModal action');
    this.store.dispatch(openCreateReviewModal());
  }

  private initializeTimeSinceLastUpdate(): void {
    this.timeSinceLastUpdate$ = interval(60000) // Emit value every 60 seconds
      .pipe(
        startWith(0), // Start immediately upon subscription
        switchMap(() => {
          const now = new Date();
          const difference = now.getTime() - this.lastUpdated.getTime();
          return [Math.floor(difference / 60000)]; // Convert to minutes
        }),
      );
  }

  private initializeSubscriptions(): void {
    const reviewCreatedSub = this.reviewsService.reviewCreated$.subscribe((review) => {
      if (review) {
        this.loadReviews(this.currentPage);
        // this.reviews.unshift(review);
      }
    });
    const reviewDeletedSub = this.reviewsService.reviewDeleted$.subscribe((deletedReviewId) => {
      if (deletedReviewId) {
        this.reviews = this.reviews.filter((review) => review.id !== deletedReviewId);
      }
    });
    const reviewUpdatedSub = this.reviewsService.reviewUpdated$.subscribe((updatedReview) => {
      if (updatedReview) {
        const index = this.reviews.findIndex((review) => review.id === updatedReview.id);
        if (index !== -1) {
          this.reviews[index] = updatedReview; // Update the review in the list
        }
      }
    });
    this.subscriptions.add(reviewCreatedSub);
    this.subscriptions.add(reviewDeletedSub);
    this.subscriptions.add(reviewUpdatedSub);
  }
}
