import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [RouterOutlet,RouterModule,CommonModule],
  templateUrl: './user.component.html',
})
export class UserComponent {

}
