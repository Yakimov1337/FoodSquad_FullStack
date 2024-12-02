import { Injectable } from '@angular/core';
import { loadStripe, Stripe } from '@stripe/stripe-js';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class StripeService {
  private stripePromise: Promise<Stripe | null>;

  constructor() {
    this.stripePromise = loadStripe(environment.stripePublishKey);
  }

  async checkoutSubscription(planId: string) {
    const stripe = await this.stripePromise;

    if (stripe) {
      const { error } = await stripe.redirectToCheckout({
        lineItems: [{ price: planId, quantity: 1 }],
        mode: 'subscription',
        successUrl: window.location.origin + '/payment-success?success=true',
        cancelUrl: window.location.origin + '/payment-cancel?canceled=true',
      });

      if (error) {
        console.error('Stripe checkout error:', error);
      }
    }
  }
}
