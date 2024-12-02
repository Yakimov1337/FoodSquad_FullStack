import { Component, Input, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { Order } from '../../../../../../core/models';
import { ButtonComponent } from '../../../../../../shared/components/button/button.component';
import { Store } from '@ngrx/store';

@Component({
  selector: '[orders-table-item]',
  templateUrl: './orders-table-item.component.html',
  standalone: true,
  imports: [
    AngularSvgIconModule,
    CurrencyPipe,
    ButtonComponent,
    CommonModule,
  ],
})
export class OrdersTableItemComponent implements OnInit {
  @Input() order: Order = <Order>{};

  constructor(private store: Store) {}

  ngOnInit(): void {}

}
