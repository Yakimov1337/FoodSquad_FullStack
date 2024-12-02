import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { take } from 'rxjs';
import { User, UserUpdate } from '../../../../core/models';
import { Store, select } from '@ngrx/store';
import { selectCurrentUser } from '../../../../core/state/auth/auth.selectors';
import { UserService } from '../../../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { animate, style, transition, trigger } from '@angular/animations';
import * as AuthActions from '../../../../core/state/auth/auth.actions';
import { urlFormValidator } from '../../../../shared/validators/url-validator';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LoaderComponent, ButtonComponent],
  templateUrl: './user-profile.component.html',
  animations: [
    trigger('slideInDown', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
})
export class UserProfileComponent implements OnInit {
  currentUser: User | null = null;
  userForm: FormGroup;
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  currentUserId: string | undefined;
  isUpdatingInfo: boolean = false;
  isUploadingAvatar: boolean = false;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private userService: UserService,
    private toastr: ToastrService,
  ) {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      phoneNumber: ['', [Validators.pattern(/^\+?(\d[\s-]?){1,11}\d$/)]],
      email: [{ value: '', disabled: true }],
      role: [{ value: '', disabled: true }],
      imageUrl: ['', [urlFormValidator()]],
    });
  }

  ngOnInit(): void {
    this.store.pipe(select(selectCurrentUser), take(1)).subscribe((user: User | null) => {
      if (user) {
        this.currentUser = user;
        this.currentUserId = user.id;
        this.userForm.patchValue({
          name: user.name,
          email: user.email,
          phoneNumber: user.phoneNumber,
          role: user.role,
          imageUrl: user.imageUrl,
        });
        this.previewUrl = user.imageUrl?.toString() || null;
      }
    });

    // Subscribe to user updates
    this.store.pipe(select(selectCurrentUser)).subscribe((user: User | null) => {
      if (user) {
        this.currentUser = user;
        this.userForm.patchValue({
          name: user.name,
          email: user.email,
          phoneNumber: user.phoneNumber,
          role: user.role,
          imageUrl: user.imageUrl,
        });
        this.previewUrl = user.imageUrl?.toString() || null;
      }
    });
  }

  updatePersonalInfo(): void {
    if (this.userForm.valid && this.currentUserId) {
      const formValues = this.userForm.getRawValue();
      const hasChanges =
        this.currentUser &&
        (this.currentUser.name !== formValues.name ||
          this.currentUser.phoneNumber !== formValues.phoneNumber ||
          this.currentUser.imageUrl.toString() !== formValues.imageUrl);

      if (!hasChanges) {
        this.toastr.info('No changes detected. No update required.');
        return;
      }

      formValues.name = formValues.name.trim().replace(/\s+/g, ' ');
      this.userForm.get('name')?.setValue(formValues.name, { emitEvent: false });

      if (!formValues.imageUrl) {
        this.toastr.error('Image URL is required.');
        return;
      }

      // this.isUpdatingInfo = true;

      const updateUserDTO: UserUpdate = {
        name: formValues.name,
        phoneNumber: formValues.phoneNumber,
        role: this.currentUser!.role.toUpperCase(),
        imageUrl: formValues.imageUrl,
      };

      this.store.dispatch(AuthActions.updateUserProfile({ id: this.currentUserId, user: updateUserDTO }));
    } else {
      this.markAllAsTouched();
      this.toastr.error('Please fill out all required fields correctly.');
    }
  }

  markAllAsTouched(): void {
    this.userForm.markAllAsTouched();
  }

  handleFileInput(event: Event): void {
    const element = event.currentTarget as HTMLInputElement;
    this.selectedFile = element.files ? element.files[0] : null;

    if (this.selectedFile) {
      this.previewUrl = URL.createObjectURL(this.selectedFile);
      this.toastr.info('Feature under maintenance');
    }
  }
}
