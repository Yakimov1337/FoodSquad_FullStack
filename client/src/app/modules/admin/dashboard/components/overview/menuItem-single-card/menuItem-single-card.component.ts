import { Component, Input, OnInit } from '@angular/core';
import { NgStyle, CurrencyPipe } from '@angular/common';
import { MenuItem } from '../../../../../../core/models';
import { RouterLink } from '@angular/router';

@Component({
    selector: '[menuItem-single-card]',
    templateUrl: './menuItem-single-card.component.html',
    standalone: true,
    imports: [NgStyle, CurrencyPipe,RouterLink],
})
export class MenuItemSingleCardComponent implements OnInit {
  @Input() menuItem: MenuItem = <MenuItem>{};

  constructor() {}

  ngOnInit(): void {}
}
