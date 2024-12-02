import { Component, Input, OnInit } from '@angular/core';
import { NgStyle, CurrencyPipe } from '@angular/common';
import { MenuItem } from '../../../../../../core/models';
import { RouterLink } from '@angular/router';

@Component({
  selector: '[menuItem-dual-card]',
  templateUrl: './menuItem-dual-card.component.html',
  standalone: true,
  imports: [NgStyle, CurrencyPipe, RouterLink],
})
export class MenuItemDualCardComponent  {
  @Input() menuItem: MenuItem = <MenuItem>{};

  constructor() {
  }

  ngOnInit(): void {}
}
