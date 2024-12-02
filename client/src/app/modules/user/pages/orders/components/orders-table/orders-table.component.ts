import { Observable, interval, startWith, switchMap } from 'rxjs';
import { Store, select } from '@ngrx/store';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Component, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { Order } from '../../../../../../core/models';
import { OrdersTableItemComponent } from '../orders-table-item/orders-table-item.component';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';
import { OrdersService } from '../../../../../../services/orders.service';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openCreateOrderModal } from '../../../../../../core/state/modal/order/modal.actions';
import { ToastrService } from 'ngx-toastr';
import { selectCurrentUser } from '../../../../../../core/state/auth/auth.selectors';

@Component({
  selector: '[orders-table]',
  templateUrl: './orders-table.component.html',
  standalone: true,
  imports: [NgFor, OrdersTableItemComponent, CommonModule, LoaderComponent, AngularSvgIconModule, ButtonComponent],
})
export class OrdersTableComponent implements OnInit {
  public orders: Order[] = [];
  public isLoading: boolean = true;
  public timeSinceLastUpdate$!: Observable<number>;
  public lastUpdated: Date = new Date();

  constructor(private ordersService: OrdersService, private store: Store, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.store.pipe(
      select(selectCurrentUser), // Use the selector to get the current user object
      switchMap(user => {
        if (user && user.id) { // Ensure the user object and its id are not undefined
          return this.ordersService.getOrdersByUserId(user.id); // Fetch orders for the user by ID
        } else {
          // Optionally handle the case where user or user.id is undefined
          return [];
        }
      })
    ).subscribe({
      next: (orders) => {
        this.orders = orders;
        this.isLoading = false;
        this.lastUpdated = new Date(); // Update the lastUpdated time to now after fetching orders
      },
      error: (error) => {
        this.toastr.error('Error fetching Orders', error);
        this.isLoading = false;
      },
    });

    this.initializeTimeSinceLastUpdate();
  }

  private initializeTimeSinceLastUpdate(): void {
    this.timeSinceLastUpdate$ = interval(60000) // Emit value every 60 seconds
      .pipe(
        startWith(0), // Start immediately upon subscription
        switchMap(() => {
          const now = new Date();
          const difference = now.getTime() - this.lastUpdated.getTime();
          return [Math.floor(difference / 60000)]; // Convert to minutes
        }),
      );
  }
}
