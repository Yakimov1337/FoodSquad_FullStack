import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { MenuItem } from '../../../../../../core/models';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { selectMenuItemToUpdate } from '../../../../../../core/state/modal/menuItem/modal.selectors';
import { closeUpdateMenuItemModal } from '../../../../../../core/state/modal/menuItem/modal.actions';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { urlFormValidator } from '../../../../../../shared/validators/url-validator';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[menuItem-update-modal]',
  templateUrl: './menuItem-update-modal.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class MenuItemUpdateModalComponent implements OnInit {
  menuItemForm: FormGroup;
  // menuItems$: Observable<MenuItem[]>;
  private currentMenuItemId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private menuItemsService: MenuItemsService,
    private toastr: ToastrService,
  ) {
    // Initialize the form with structure
    this.menuItemForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      price: [1, [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?(\.\d+)?(?<=\d)$/)]],
      category: ['Burger',Validators.required],
      imageUrl: ['', [urlFormValidator()]],
    });

    // this.menuItems$ = this.menuItemsService.getAllMenuItems();
  }

  ngOnInit(): void {
    // Subscribe to the current MenuItem to update
    this.store.select(selectMenuItemToUpdate).subscribe((menuItem) => {
      if (menuItem) {
        this.currentMenuItemId = menuItem.id;
        this.menuItemForm.patchValue({
          title: menuItem.title,
          description: menuItem.description,
          price: menuItem.price,
          category: menuItem.category,
          imageUrl: menuItem.imageUrl,
        });
      }
    });
  }

  updateMenuItem(): void {
    if (this.menuItemForm.valid && this.currentMenuItemId) {
      this.menuItemsService.updateMenuItem(this.currentMenuItemId, this.menuItemForm.value).subscribe({
        next: (MenuItem) => {
          this.closeModal();
          this.menuItemsService.menuItemUpdated(MenuItem);
          this.toastr.success('Menu item updated successfully!');
        },
        error: (error) => this.toastr.error('Error updating menu item', error),
      });
    }  else {
      this.menuItemForm.markAllAsTouched();
      // If the form is invalid, iterate over the controls and log the errors
      Object.keys(this.menuItemForm.controls).forEach((key) => {
        const control = this.menuItemForm.get(key);
        const errors = control?.errors ?? {};
        Object.keys(errors).forEach((keyError) => {
          this.toastr.error(`Form Invalid - control: ${key}, Error: ${keyError}`);
        });
      });

    }
  }

  closeModal(): void {
    this.store.dispatch(closeUpdateMenuItemModal());
  }
}
