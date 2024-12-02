/// <reference types="@angular/localize" />
import { APP_INITIALIZER, enableProdMode, importProvidersFrom, isDevMode } from '@angular/core';
import { environment } from './environments/environment';
import { AppComponent } from './app/app.component';
import { AppRoutingModule } from './app/app-routing.module';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { BrowserAnimationsModule, provideAnimations } from '@angular/platform-browser/animations';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { userModalReducer } from './app/core/state/modal/user/modal.reducer';
import { orderModalReducer } from './app/core/state/modal/order/modal.reducer';
import { menuItemsModalReducer } from './app/core/state/modal/menuItem/modal.reducer';
import { provideToastr } from 'ngx-toastr';
import { AuthStateService } from './app/services/auth-state.service';
import { authReducer } from './app/core/state/auth/auth.reducer';
import { AuthEffects } from './app/core/state/auth/auth.effects';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { AuthInterceptor } from './app/core/interceptor/interceptor';
import { cartReducer } from './app/core/state/shopping-cart/cart.reducer';
import { CartEffects } from './app/core/state/shopping-cart/cart.effects';
import { ReviewModalReducer } from './app/core/state/modal/review/modal.reducer';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

if (environment.production) {
  enableProdMode();
  //show this warning only in prod mode
  if (window) {
    selfXSSWarning();
  }
}



export function initializeApp(authStateService: AuthStateService, translate: TranslateService) {
  return (): Promise<void> => {
    return new Promise<void>((resolve) => {
      translate.setDefaultLang('en');
      translate.use('en').subscribe(() => {
        authStateService.initializeAuthState().then(() => resolve());
      });
    });
  };
}
export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http);
}

const initialReducers = {
  userModals: userModalReducer,
  orderModals: orderModalReducer,
  reviewModals: ReviewModalReducer,
  menuItemModals: menuItemsModalReducer,
  auth: authReducer,
  cart: cartReducer,
};

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      BrowserModule,
      AppRoutingModule,
      BrowserAnimationsModule,
      HttpClientModule,
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient],
        },
      })
    ),
    provideAnimations(),
    provideToastr(),
    provideStore({
      userModals: userModalReducer,
      orderModals: orderModalReducer,
      reviewModals: ReviewModalReducer,
      menuItemModals: menuItemsModalReducer,
      auth: authReducer,
      cart: cartReducer,
    }),
    provideEffects([AuthEffects, CartEffects]),
    provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() }),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AuthStateService, TranslateService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
}).catch((err) => console.error(err));


function selfXSSWarning() {
  setTimeout(() => {
    console.log(
      '%c** STOP **',
      'font-weight:bold; font: 2.5em Arial; color: white; background-color: #e11d48; padding-left: 15px; padding-right: 15px; border-radius: 25px; padding-top: 5px; padding-bottom: 5px;',
    );
    console.log(
      `\n%cThis is a browser feature intended for developers. Using this console may allow attackers to impersonate you and steal your information using an attack called Self-XSS. Do not enter or paste code that you do not understand.`,
      'font-weight:bold; font: 2em Arial; color: #e11d48;',
    );
  });
}
