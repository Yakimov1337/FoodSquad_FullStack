import { Subscription } from 'rxjs';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { selectDeleteMenuItemId } from '../../../../../../core/state/modal/menuItem/modal.selectors';
import { closeDeleteMenuItemModal } from '../../../../../../core/state/modal/menuItem/modal.actions';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[menuItem-delete-modal]',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './menuItem-delete-modal.component.html',
})
export class MenuItemDeleteModalComponent {
  menuItemIdToDelete: number | undefined | null = null;
  private subscription = new Subscription();

  constructor(private store: Store, private MenuItemsService: MenuItemsService, private toastr: ToastrService) {
    this.subscription.add(
      this.store.select(selectDeleteMenuItemId).subscribe((MenuItemId) => {
        console.log(MenuItemId)
        this.menuItemIdToDelete = MenuItemId;
      }),
    );
  }

  deleteMenuItem(): void {
    if (!this.menuItemIdToDelete) {
      this.toastr.error('Menu item ID to delete not provided!')
      return;
    }
    this.MenuItemsService.deleteMenuItem(this.menuItemIdToDelete).subscribe({
      next: () => {
        this.MenuItemsService.menuItemDeleted(this.menuItemIdToDelete);
        this.toastr.success(`Menu item with ID ${this.menuItemIdToDelete} deleted successfully!`)
        this.closeModal();
      },
      error: (error) => this.toastr.error('Failed to delete menu item!:', error.message)
    });
  }

  closeModal(): void {
    this.store.dispatch(closeDeleteMenuItemModal());
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
