import { Component } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../../../../../core/models';
import { UserService } from '../../../../../../services/user.service';
import { closeUpdateUserModal } from '../../../../../../core/state/modal/user/modal.actions';
import { ToastrService } from 'ngx-toastr';
import { selectUserToUpdate } from '../../../../../../core/state/modal/user/modal.selectors';
import { urlFormValidator } from '../../../../../../shared/validators/url-validator';
import { Observable } from 'rxjs';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';

@Component({
  selector: '[user-update-modal]',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-update-modal.component.html',
})
export class UserUpdateModalComponent {
  userForm: FormGroup;
  currentUser$: Observable<User | null>;
  private currentUserId: string | undefined | null = null;
  get emailControl(): FormControl {
    const email = this.userForm.get('email');
    if (!email) {
      throw new Error('Email FormControl is not found in the form group');
    }
    return email as FormControl;
  }
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private store: Store,
    private toastr: ToastrService,
  ) {
    this.userForm = this.fb.group({
      email: new FormControl({ value: '', disabled: true }),
      name: [''],
      role: ['', Validators.required],
      phoneNumber: ['', [Validators.pattern(/^\+?(\d[\s-]?){1,11}\d$/)]],
      imageUrl: ['', urlFormValidator()],
    });
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
  }

  ngOnInit(): void {
    this.store.select(selectUserToUpdate).subscribe((user) => {
      console.log(user);
      if (user) {
        console.log(user.role)
        this.currentUserId = user.id;
        this.userForm.patchValue({
          email: user.email,
          name: user.name,
          role: user.role,
          phoneNumber: user.phoneNumber,
          imageUrl: user.imageUrl,
        });
      }
    });
  }

  updateUser(): void {
    if (this.userForm.valid && this.currentUserId) {
      const userData = {
        ...this.userForm.getRawValue(),
        role: this.userForm.get('role')?.value.toUpperCase(),
        email: this.userForm.get('email')?.value //  email included
      };
      this.userService.updateUser(this.currentUserId, userData).subscribe({
        next: (user) => {
          this.closeModal();
          this.userService.userUpdated(user); //Notify about user update
          this.toastr.success('User updated successfully!');
        },
        error: (error) => this.toastr.error('Error updating this user:', error),
      });
    } else {
      this.userForm.markAllAsTouched();
      // If the form is invalid, iterate over the controls and log the errors
      Object.keys(this.userForm.controls).forEach((key) => {
        const control = this.userForm.get(key);
        const errors = control?.errors ?? {};
        Object.keys(errors).forEach((keyError) => {
          this.toastr.error(`Form Invalid - control: ${key}, Error: ${keyError}`);
        });
      });
    }
  }

  closeModal(): void {
    this.store.dispatch(closeUpdateUserModal());
  }
}
