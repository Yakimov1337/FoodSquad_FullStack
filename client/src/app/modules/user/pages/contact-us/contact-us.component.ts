import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { animate, style, transition, trigger } from '@angular/animations';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-contact-us',
  standalone: true,
  imports: [FormsModule,TranslateModule],
  templateUrl: './contact-us.component.html',
  animations: [
    trigger('slideInLeft', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateX(0)', opacity: 1 })),
      ]),
    ]),
    trigger('slideInDown', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
})
export class ContactUsComponent {
  email: string = '';
  subject: string = '';
  message: string = '';

  constructor(private toastr: ToastrService) {}

  onSubmit(): void {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!this.email || !emailRegex.test(this.email)) {
      this.toastr.error('Please enter a valid email address.');
      return;
    }

    // Assuming the form is valid at this point, construct and open the mailto link
    const mailtoLink = `mailto:${this.email}?subject=${encodeURIComponent(this.subject)}&body=${encodeURIComponent(this.message)}`;
    window.location.href = mailtoLink;

    this.toastr.success('Thank you for reaching out!');
    // Reset form fields after submission
    this.email = '';
    this.subject = '';
    this.message = '';
  }
}
