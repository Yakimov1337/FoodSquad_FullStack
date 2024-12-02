import { map, Observable, of } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule, formatDate } from '@angular/common';
import { MenuItem, Review, User } from '../../../../../../core/models';
import {
  ReactiveFormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
  FormArray,
  ValidatorFn,
  AbstractControl,
} from '@angular/forms';
import { ReviewsService } from '../../../../../../services/reviews.service';
import { closeCreateReviewModal } from '../../../../../../core/state/modal/review/modal.actions';
import { UserService } from '../../../../../../services/user.service';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[review-create-modal]',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './review-create-modal.component.html',
})
export class ReviewCreateModalComponent {
  reviewForm: FormGroup;
  menuItems$!: Observable<MenuItem[]>;

  constructor(
    private fb: FormBuilder,
    private reviewsService: ReviewsService,
    private menuItemsService: MenuItemsService,
    private store: Store,
    private toastr: ToastrService
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
  }

  createReview(): void {
    if (this.reviewForm.valid) {
      const reviewData = this.reviewForm.value;

      this.reviewsService.createReview(reviewData).subscribe({
        next: (review: Review) => {
          this.toastr.success('Review created successfully!');
          this.closeModal();
          this.reviewForm.reset();
        },
        error: (error: any) => this.toastr.error('Failed to create review!', error)
      });
    } else {
      this.reviewForm.markAllAsTouched();
    }
  }

  closeModal(): void {
    this.store.dispatch(closeCreateReviewModal());
  }
}
