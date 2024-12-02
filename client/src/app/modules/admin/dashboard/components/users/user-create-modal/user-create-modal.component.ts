import { Component, OnInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { CommonModule } from '@angular/common';
import { User } from '../../../../../../core/models';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { closeCreateUserModal } from '../../../../../../core/state/modal/user/modal.actions';
import { AuthService } from '../../../../../../services/auth.service';
import { UserService } from '../../../../../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';

@Component({
  selector: '[user-create-modal]',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-create-modal.component.html',
})
export class UserCreateModalComponent {
  userForm: FormGroup;
  currentUser$: Observable<User | null>;

  constructor(
    private fb: FormBuilder,
    private store: Store,
    private authService: AuthService,
    private userService: UserService,
    private toastr: ToastrService,
  ) {
    this.currentUser$ = this.store.pipe(select(selectCurrentUser));
    this.userForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: [''],
      // name: [''],
      // role: ['Normal', Validators.required],
      // phoneNumber: ['', Validators.pattern(/^\+?[0-9]{1,3}?[-\s]?([0-9]{1,4}[-\s]?)*$/)],
      // imageUrl: [''],
    });
  }

  // Function to handle form submission
  createUser(): void {
    if (this.userForm.valid) {
      const userData = {
        ...this.userForm.value,
        confirmPassword: this.userForm.value.password, // Automatically set confirmPassword to match password
      };
      console.log(userData);
      this.userService.createUser(userData).subscribe({
        next: (user: User) => {
          this.closeModal();
          this.resetForm(); // Reset form to default state after creation
          // This will be called only after the user is saved because of switchmap in auth service
          this.userService.userCreated(user); // Notify about the new user !!! VERY IMPORTANT- REFETCH THE TABLE
          this.toastr.success('User created successfully!');
        },
        error: (error: any) => {
          this.toastr.error(error);
        },
      });
    } else {
      this.userForm.markAllAsTouched();
      // If the form is invalid, iterate over the controls and log the errors
      Object.keys(this.userForm.controls).forEach((key) => {
        const control = this.userForm.get(key);
        const errors = control?.errors ?? {}; // Use nullish coalescing operator to default to an empty object if errors are null
        Object.keys(errors).forEach((keyError) => {
          this.toastr.error(`Form Invalid - control: ${key}, Error: ${keyError}`);
        });
      });
    }
  }

  closeModal(): void {
    this.store.dispatch(closeCreateUserModal());
  }

  resetForm(): void {
    this.userForm.reset({
      email: [''],
      password: [''],
      name: [''],
      role: ['Normal'],
      phoneNumber: [''],
      imageUrl: [''],
    });
  }
}
