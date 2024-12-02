import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { MenuItem, User } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { Store, select } from '@ngrx/store';
import {
  openDeleteMenuItemModal,
  openUpdateMenuItemModal,
  openUserReviewsModal,
} from '../../../../../../core/state/modal/menuItem/modal.actions';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';
import { Observable } from 'rxjs';

@Component({
  selector: '[menuItem-auctions-table-item]',
  templateUrl: './menuItem-auctions-table-item.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, CurrencyPipe, ButtonComponent, CommonModule],
})
export class MenuItemAuctionsTableItemComponent implements OnInit {
  @Input() menuItem: MenuItem = <MenuItem>{};
  @Input() selectedItemIds: number[] = [];
  @Input() onToggleSelection: (id: number) => void = () => {};
  @Input() isSelected: (id: number) => boolean = () => false;

  currentUser$: Observable<User | null>;

  constructor(private store: Store) {
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }

  ngOnInit(): void {}
  handleCheckboxChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.onToggleSelection(this.menuItem.id);
  }

  handleUpdateButtonClick(currentUser: User, menuItem: MenuItem): void {
    // Only Admin can update default items
    // if (currentUser.role === 'Admin' || (currentUser.role === 'Moderator' && !menuItem.defaultItem)) {
    // }
    this.openUpdateModal();
  }

  openUpdateModal() {
    this.store.dispatch(openUpdateMenuItemModal({ menuItem: this.menuItem }));
  }

  openDeleteModal() {
    this.store.dispatch(openDeleteMenuItemModal({ menuItemId: this.menuItem.id }));
  }

  openUserReviewsModal(itemId: number): void {
    this.store.dispatch(openUserReviewsModal({ itemId }));
  }


  truncateDescription(description: string, maxLength: number = 25): string {
    if (description.length > maxLength) {
      return description.substring(0, maxLength) + '...';
    } else {
      return description;
    }
  }
  getTone(rating: number): 'primary' | 'danger' | 'success' | 'warning' | 'info' | 'light' {
    const roundedRating = Math.round(rating);
    switch (roundedRating) {
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
  getCategoryTone(category: string): 'primary' | 'danger' | 'success' | 'warning' | 'info' | 'light' {
    switch (category) {
      case 'PIZZA':
        return 'primary';
      case 'PASTA':
        return 'success';
      case 'BURGER':
        return 'warning';
      case 'DESSERT':
        return 'info';
      case 'SALAD':
        return 'success';
      case 'OTHER':
        return 'light';
      default:
        return 'info';
    }
  }
}
