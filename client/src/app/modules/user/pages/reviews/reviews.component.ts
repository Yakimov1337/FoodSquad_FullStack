import { Component } from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';
import { ReviewsTableComponent } from './components/reviews-table/reviews-table.component';
@Component({
  selector: 'app-Reviews',
  standalone: true,
  imports: [  ReviewsTableComponent,],
  templateUrl: './reviews.component.html',
  animations: [
    trigger('slideInDown', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
})
export class ReviewsComponent {

}
