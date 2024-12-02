import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from './layout.component';
import { HomeComponent } from '../pages/home/home.component';
import { ContactUsComponent } from '../pages/contact-us/contact-us.component';
import { MenuComponent } from '../pages/menu/menu.component';
import { UserProfileComponent } from '../pages/user-profile/user-profile.component';
import { AboutUsComponent } from '../pages/about-us/about-us.component';
import { OrdersComponent } from '../pages/orders/orders.component';
import { SuccessComponent } from '../pages/stripe/success/success.component';
import { CancelledComponent } from '../pages/stripe/cancelled/cancelled.component';
import { ReviewsComponent } from '../pages/reviews/reviews.component';

const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'menu', component: MenuComponent },
      { path: 'reviews', component: ReviewsComponent },
      { path: 'profile', component: UserProfileComponent },
      { path: 'orders', component: OrdersComponent },
      { path: 'about-us', component: AboutUsComponent },
      { path: 'contact-us', component: ContactUsComponent },
      { path: 'payment-success', component: SuccessComponent },
      { path: 'payment-cancel', component: CancelledComponent },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UserLayoutRoutingModule { }
