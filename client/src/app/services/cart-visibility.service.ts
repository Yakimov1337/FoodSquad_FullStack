// cart-visibility.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartVisibilityService {
  private showCartSubject = new BehaviorSubject<boolean>(false);
  showCart$ = this.showCartSubject.asObservable();

  constructor() {}

  toggleCart(): void {
    this.showCartSubject.next(!this.showCartSubject.value);
  }
}
