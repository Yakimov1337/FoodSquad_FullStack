import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControlOptions, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { MustMatch } from './must-match.validator';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonComponent } from 'src/app/shared/components/button/button.component';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { Store } from '@ngrx/store';
import * as AuthActions from 'src/app/core/state/auth/auth.actions'

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink, AngularSvgIconModule, ButtonComponent],
})
export class SignUpComponent implements OnInit {
  passwordTextType!: boolean;
  signUpForm!: FormGroup;
  isLoading = false;

  get passwordStrength(): boolean[] {
    const length = this.signUpForm.get('password')?.value.length || 0;
    return [1, 2, 3, 4].map((i) => length >= i * 1.5);
  }

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService,
    private store: Store,
  ) {}

  ngOnInit() {
    const formOptions: AbstractControlOptions = {
      validators: MustMatch('password', 'confirmPassword'),
    };

    this.signUpForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      formOptions,
    );
  }

  get form() {
    return this.signUpForm.controls;
  }

  onSubmit() {
    this.signUpForm.markAllAsTouched();
    this.isLoading = true;
    if (this.signUpForm.valid) {
      const { email, password, confirmPassword } = this.signUpForm.value;
      this.authService.createUserAccount({ email, password, confirmPassword }).subscribe({
        next: (userCreatedResponse) => {
          this.toastr.success('Registration successful. Please log in.');
          this.router.navigate(['/auth/sign-in']);
          this.isLoading = false;
        },
        error: (error) => {
          this.toastr.error(error);
          this.isLoading = false;
        },
      });
    } else {
      this.toastr.error('Form is invalid');
      this.isLoading = false;
    }
  }

  togglePasswordTextType() {
    this.passwordTextType = !this.passwordTextType;
  }

  googleAuth(event: Event){
    this.toastr.info('Coming soon!');
    event.preventDefault();
  }
}
