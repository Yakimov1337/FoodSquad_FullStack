import { Observable, interval, startWith, switchMap } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Component, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { Review } from '../../../../../../core/models';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';

import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openCreateOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { ToastrService } from 'ngx-toastr';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';
import { ReviewsTableItemComponent } from '../reviews-table-item/reviews-table-item.component';
import { ReviewsService } from '../../../../../../services/reviews.service';

@Component({
  selector: '[reviews-table]',
  templateUrl: './reviews-table.component.html',
  standalone: true,
  imports: [NgFor, ReviewsTableItemComponent, CommonModule, LoaderComponent, AngularSvgIconModule, ButtonComponent],
})
export class ReviewsTableComponent implements OnInit {
  public reviews: Review[] = [];
  public isLoading: boolean = true;
  public timeSinceLastUpdate$!: Observable<number>;
  public lastUpdated: Date = new Date();

  constructor(private reviewsService: ReviewsService, private store: Store, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.store.pipe(
      select(selectCurrentUser), // Use the selector to get the current user object
      switchMap(user => {
        if (user && user.id) { // Ensure the user object and its id are not undefined
          return this.reviewsService.getReviewsByUserId(user.id); // Fetch reviews for the user by ID
        } else {
          
          return [];
        }
      })
    ).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
        this.isLoading = false;
        this.lastUpdated = new Date(); // Update the lastUpdated time to now after fetching reviews
      },
      error: (error) => {
        this.toastr.error('Error fetching reviews', error);
        this.isLoading = false;
      },
    });

    this.initializeTimeSinceLastUpdate();
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
}
