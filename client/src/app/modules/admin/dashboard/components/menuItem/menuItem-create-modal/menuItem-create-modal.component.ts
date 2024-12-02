import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { MenuItem } from '../../../../../../core/models';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { urlFormValidator } from '../../../../../../shared/validators/url-validator';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { closeCreateMenuItemModal } from '../../../../../../core/state/modal/menuItem/modal.actions';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: '[menuItem-create-modal]',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './menuItem-create-modal.component.html',
})
export class MenuItemCreateModalComponent {
  menuItemForm: FormGroup;
  defaultImageUrl: URL = new URL(
    'https://w0.peakpx.com/wallpaper/97/150/HD-wallpaper-mcdonalds-double-cheese-burger-double-mcdonalds-cheese-burger-thumbnail.jpg',
  );

  constructor(
    private fb: FormBuilder,
    private menuItemsService: MenuItemsService,
    private store: Store,
    private toastr: ToastrService,
  ) {
    this.menuItemForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      price: [1, [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?(\.\d+)?(?<=\d)$/)]],
      imageUrl: ['', [urlFormValidator()]],
      category: ['Burger',Validators.required],
      useDefaultImage: [false],
    });
  }

  ngOnInit() {
    this.onUseDefaultImageChange(); // Setup listener for the checkbox
  }

  // Function to handle form submission
  createMenuItem(): void {
    if (this.menuItemForm.valid) {
      this.menuItemForm.get('imageUrl')?.enable();
      const submissionValues = this.menuItemForm.getRawValue();
      delete submissionValues.useDefaultImage; // removing boolean from submit values
      this.menuItemsService.createMenuItem(submissionValues).subscribe({
        next: (MenuItem: MenuItem) => {
          this.toastr.success('Menu Item created successfully!');
          this.closeModal();
          this.resetForm(); // Reset form to default state after creation
          this.menuItemsService.menuItemCreated(MenuItem); // Notify about the new MenuItem !!! VERY IMPORTANT- REFETCH THE TABLE
        },
        error: (error: any) => this.toastr.error('Error creating menu item!', error),
      });
    } else {
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
    this.store.dispatch(closeCreateMenuItemModal());
  }

  resetForm(): void {
    this.menuItemForm.reset({
      title: '',
      description: '',
      price: 1,
      imageUrl: '',
    });
  }

  onUseDefaultImageChange(): void {
    this.menuItemForm.get('useDefaultImage')?.valueChanges.subscribe((useDefault: boolean) => {
      if (useDefault) {
        this.menuItemForm.get('imageUrl')?.setValue(this.defaultImageUrl.href);
        this.menuItemForm.get('imageUrl')?.disable(); // Optionally disable to prevent edits
      } else {
        this.menuItemForm.get('imageUrl')?.enable(); // Re-enable if the user unchecks
        this.menuItemForm.get('imageUrl')?.setValue('');
      }
    });
  }
}
