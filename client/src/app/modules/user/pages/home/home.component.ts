import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';
import { Store } from '@ngrx/store';
import * as AuthActions from '../../../../core/state/auth/auth.actions';
import { HeroSectionComponent } from './components/hero-section/hero-section.component';
import { FoodCategoryComponent } from './components/food-category/food-category.component';
import { InfoSectionComponent } from './components/info-section/info-section.component';
import { PricingComponent } from './components/pricing/pricing.component';
import { ThemeService } from '../../../../services/theme.service';
import { CommonModule } from '@angular/common';
import { SubscribeComponent } from './components/subscribe/subscribe.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterLink,
    HeroSectionComponent,
    InfoSectionComponent,
    FoodCategoryComponent,
    PricingComponent,
    SubscribeComponent,
    CommonModule,
  ],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  isDarkMode: boolean;
  constructor(
    private router: Router,
    private authService: AuthService,
    private store: Store,
    public themeService: ThemeService,
  ) {
    this.isDarkMode = this.themeService.isDark;
  }
  logout() {
    this.store.dispatch(AuthActions.logout());
  }

  checkThemeMode() {
    this.themeService.theme.update((theme) => {
      const mode = !this.themeService.isDark ? 'dark' : 'light';
      return { ...theme, mode: mode };
    });
  }
}
