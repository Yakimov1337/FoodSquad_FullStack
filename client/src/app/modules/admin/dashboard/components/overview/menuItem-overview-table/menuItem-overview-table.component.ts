import { Observable, Subscription, interval, startWith, switchMap } from 'rxjs';
import { Component, Input, OnInit } from '@angular/core';

import { CommonModule, NgFor } from '@angular/common';
import { MenuItemsService } from '../../../../../../services/menuItems.service';
import { MenuItem } from '../../../../../../core/models';
import { LoaderComponent } from '../../../../../../shared/components/loader/loader.component';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { openCreateMenuItemModal } from '../../../../../../core/state/modal/menuItem/modal.actions';
import { Store } from '@ngrx/store';
import { PaginationComponent } from '../../../../../../shared/components/pagination/pagination.component';
import { ToastrService } from 'ngx-toastr';
import { MenuItemOverviewTableItemComponent } from '../menuItem-overview-table-item/menuItem-overview-table-item.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: '[menuItem-overview-table]',
  templateUrl: './menuItem-overview-table.component.html',
  standalone: true,
  imports: [
    NgFor,
    AngularSvgIconModule,
    ButtonComponent,
    MenuItemOverviewTableItemComponent,
    CommonModule,
    LoaderComponent,
    PaginationComponent,
    RouterLink
  ],
})
export class MenuItemOverviewTableComponent implements OnInit {
  @Input() menuItems: MenuItem[] = [];
  @Input() isLoading: boolean = true;
  public timeSinceLastUpdate$!: Observable<number>;
  public lastUpdated: Date = new Date();

  constructor() {}

  ngOnInit(): void {
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
