import { Component, OnInit } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { AngularSvgIconModule } from 'angular-svg-icon';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  standalone: true,
  imports: [AngularSvgIconModule, RouterOutlet, RouterModule],
})
export class AuthComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
