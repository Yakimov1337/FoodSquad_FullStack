import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { trigger, transition, style, animate, state } from '@angular/animations';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-food-category',
  standalone: true,
  imports: [CommonModule,TranslateModule],
  templateUrl: './food-category.component.html',
  animations: [
    trigger('slideIn', [
      state('slideInFromLeft', style({ transform: 'translateX(0)', opacity: 1 })), // Define end state for clarity, even if not used for animation
      state('slideInFromRight', style({ transform: 'translateX(0)', opacity: 1 })), // Define end state for clarity
      transition('* => slideInFromLeft', [
        style({ transform: 'translateX(-100%)', opacity: 0 }),
        animate('1s ease-out'),
      ]),
      transition('* => slideInFromRight', [
        style({ transform: 'translateX(100%)', opacity: 0 }),
        animate('1s ease-out'),
      ]),
      state('fadeInOut', style({ opacity: 1 })),
      transition(':enter', [style({ opacity: 0 }), animate('3.5s ease-in', style({ opacity: 1 }))]),
      transition(':leave', [animate('0.5s ease-out', style({ opacity: 0 }))]),
    ]),
  ],
})
export class FoodCategoryComponent implements OnInit {
  selectedCategory = 'burger'; // Default category
  @Output() categorySelected = new EventEmitter<string>();
  categories = [
    { label: 'Pizza', value: 'Pizza', icon: 'assets/images/pizza.png' },
    { label: 'Burger', value: 'Burger', icon: 'assets/images/burger.png' },
    { label: 'Pasta', value: 'Pasta', icon: 'assets/images/pasta.png' },
    { label: 'Sushi', value: 'Sushi', icon: 'assets/images/sushi.png' },
    { label: 'Salad', value: 'Salad', icon: 'assets/images/salad.png' },
    { label: 'Tacos', value: 'Tacos', icon: 'assets/images/tacos.png' },
    { label: 'Dessert', value: 'Dessert', icon: 'assets/images/desert.png' },
    { label: 'Other', value: 'Other', icon: 'assets/images/other.png' },
  ];

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      // Update selectedCategory based on queryParams or default to 'Burger'
      this.selectedCategory = params['category'] || this.selectedCategory;
    });
  }

  selectCategory(category: { label: string; value: string }): void {
    // Navigate and update the queryParams upon selection
    this.router.navigate(['/menu'], { queryParams: { category: category.value.toLowerCase() } });
  }

  determineAnimation(i: number): string {
    return i < 4 ? 'slideInFromLeft' : 'slideInFromRight';
  }
}
