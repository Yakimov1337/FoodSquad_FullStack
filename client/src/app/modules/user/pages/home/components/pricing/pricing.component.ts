import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnInit,
  QueryList,
  Renderer2,
  ViewChildren,
} from '@angular/core';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { StripeService } from '../../../../../../services/stripe.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [AngularSvgIconModule, CommonModule, TranslateModule],
  templateUrl: './pricing.component.html',
  animations: [
    trigger('enterFromLeft', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(-100%)' }),
        animate('1s', style({ opacity: 1, transform: 'translateX(0)' })),
      ]),
    ]),
    trigger('enterFromRight', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(100%)' }),
        animate('1s', style({ opacity: 1, transform: 'translateX(0)' })),
      ]),
    ]),
  ],
})
export class PricingComponent implements AfterViewInit {
  @ViewChildren('planElement') planElements!: QueryList<ElementRef>;
  isVisible: boolean[] = [false, true, false];
  constructor(private stripeService: StripeService, private renderer: Renderer2) {}

  ngAfterViewInit(): void {
    this.planElements.forEach((plan, index) => {
      if (index === 0 || index === 2) {
        // Only observe the 1st and 3rd plans
        const observer = new IntersectionObserver(
          (entries) => {
            entries.forEach((entry) => {
              if (entry.isIntersecting) {
                this.isVisible[index] = true; // Set the flag to true when in view
                observer.unobserve(entry.target); // Stop observing once visible
              }
            });
          },
          { threshold: 0.5 },
        );

        observer.observe(plan.nativeElement);
      }
    });
  }
  plans = [
    {
      key: 'BASIC_BITE',
      id: 'price_1P0kaMHTUTLeEgjn0T3mo9fx',
    },
    {
      key: 'GOURMET_GROWTH',
      id: 'price_1P0kJCHTUTLeEgjnRilGp9Db',
    },
    {
      key: 'EPICUREAN_ENTERPRISE',
      id: 'price_1P0kb5HTUTLeEgjn4zAC25My',
    },
  ];

  subscribe(planId: string) {
    this.stripeService.checkoutSubscription(planId);
  }
}
