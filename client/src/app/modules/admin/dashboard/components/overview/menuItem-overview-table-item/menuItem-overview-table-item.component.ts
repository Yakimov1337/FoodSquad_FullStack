import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { MenuItem } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { Store } from '@ngrx/store';
import {
  openDeleteMenuItemModal,
  openUpdateMenuItemModal,
} from '../../../../../../core/state/modal/menuItem/modal.actions';

@Component({
  selector: '[menuItem-overview-table-item]',
  templateUrl: './menuItem-overview-table-item.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, CurrencyPipe, ButtonComponent,CommonModule],
})
export class MenuItemOverviewTableItemComponent implements OnInit {
  @Input() menuItem: MenuItem = <MenuItem>{};
  constructor(private store: Store) {}

  ngOnInit(): void {}

  openUpdateModal() {
    this.store.dispatch(openUpdateMenuItemModal({ menuItem: this.menuItem }));
  }

  openDeleteModal() {
    this.store.dispatch(openDeleteMenuItemModal({ menuItemId: this.menuItem.id }));
  }

  truncateDescription(description: string, maxLength: number = 25): string {
    if (description.length > maxLength) {
      return description.substring(0, maxLength) + '...';
    } else {
      return description;
    }
  }
}
