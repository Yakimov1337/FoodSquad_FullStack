import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgClass, NgIf } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { AuthService } from '../../../../services/auth.service';
import { Store } from '@ngrx/store';
import * as AuthActions from '../../../../core/state/auth/auth.actions';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, RouterLink, AngularSvgIconModule, NgClass, NgIf, ButtonComponent],
})
export class SignInComponent implements OnInit {
  form!: FormGroup;
  submitted = false;
  passwordTextType!: boolean;
  isLoadingUser = false;
  isLoadingAdmin = false;
  isLoadingModerator = false;
  baseUrl: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private store: Store,
    private toastr: ToastrService,
  ) {}

  ngOnInit(): void {
    this.baseUrl = this.removeApiSegment(environment.apiUrl);
    this.form = this.formBuilder.group({
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  // Admin Login
  loginAsAdmin() {
    this.isLoadingAdmin = true;
    this.login("admin@example.com", "123123", 'admin');
  }

  // Moderator Login
  loginAsModerator() {
    this.isLoadingModerator = true;
    this.login("moderator@example.com", "123123", 'moderator');
  }

  // Normal User Login
  loginAsUser() {
    if (this.form.valid) {
      this.isLoadingUser = true;
      const { email, password } = this.form.value;
      this.login(email, password, 'user');
    } else {
      this.toastr.error('Please fill the form with proper values.');
    }
  }

  // Common login method
  private login(email: string, password: string, type: 'user' | 'admin' | 'moderator') {
    this.authService.signInAccount(email, password).subscribe({
      next: (user) => {
        this.store.dispatch(AuthActions.loginSuccess({ user }));
      },
      error: (error) => {
        this.toastr.error(error);
        (this as any)[`isLoading${type.charAt(0).toUpperCase() + type.slice(1)}`] = false; // Converts to camelCase isLoading admin or user or mod
      },
    });
  }

  get f() {
    return this.form.controls;
  }

  togglePasswordTextType() {
    this.passwordTextType = !this.passwordTextType;
  }

  private removeApiSegment(url: string): string {
    return url.replace(/\/api$/, '');
  }
  swaggerRedirect() {
    // Remove the '/api' segment from apiUrl to get baseUrl (workaround for now)
    const url = `${this.baseUrl}/swagger/index.html`;
    window.open(url, '_blank');
  }

  googleAuth(event: Event) {
    this.toastr.info('Coming soon!');
    event.preventDefault();
  }
}
