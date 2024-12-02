import { Store } from '@ngrx/store';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenuItemAuctionsTableComponent } from '../../components/menuItem/menuItem-auctions-table/menuItem-auctions-table.component';
import { MenuItemCreateModalComponent } from '../../components/menuItem/menuItem-create-modal/menuItem-create-modal.component';
import { MenuItemDeleteModalComponent } from '../../components/menuItem/menuItem-delete-modal/menuItem-delete-modal.component';
import { MenuItemUpdateModalComponent } from '../../components/menuItem/menuItem-update-modal/menuItem-update-modal.component';
import { UserReviewsModalComponent } from '../../components/menuItem/user-reviews-modal/user-reviews.component';
import { selectIsCreateMenuItemModalOpen, selectIsDeleteMenuItemModalOpen, selectIsUpdateMenuItemModalOpen, selectIsUserReviewsModalOpen } from '../../../../../core/state/modal/menuItem/modal.selectors';
import { animate, state, style, transition, trigger } from '@angular/animations';
@Component({
  selector: 'app-items',
  standalone: true,
  imports: [
    CommonModule,
    MenuItemAuctionsTableComponent,
    MenuItemCreateModalComponent,
    MenuItemDeleteModalComponent,
    MenuItemUpdateModalComponent,
    UserReviewsModalComponent,
  ],
  templateUrl: './items.component.html',
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
export class ItemsComponent {
  showCreateMenuItemModal$ = this.store.select(selectIsCreateMenuItemModalOpen);
  showDeleteMenuItemModal$ = this.store.select(selectIsDeleteMenuItemModalOpen);
  showUpdateMenuItemModal$ = this.store.select(selectIsUpdateMenuItemModalOpen);
  showUserReviewsModal$ = this.store.select(selectIsUserReviewsModalOpen);

  constructor(private store: Store) {}
}
