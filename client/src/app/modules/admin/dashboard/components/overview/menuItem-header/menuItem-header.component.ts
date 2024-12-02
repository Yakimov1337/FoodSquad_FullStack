import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-menuItem-header',
    templateUrl: './menuItem-header.component.html',
    standalone: true,
    imports: [RouterLink]
})
export class MenuItemHeaderComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
