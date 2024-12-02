import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, QueryList, Renderer2, ViewChildren } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { trigger, state, style, animate, transition, query, stagger } from '@angular/animations';

@Component({
  selector: 'app-food-category',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './food-category.component.html',
  animations: [
    trigger('slideFromLeft', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(-100%)' }),
        animate('1s ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
      ])
    ]),
    trigger('slideFromRight', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(100%)' }),
        animate('1s ease-out', style({ opacity: 1, transform: 'translateX(0)' }))
      ])
    ])
  ],
})
export class FoodCategoryComponent implements AfterViewInit {
  @ViewChildren('foodItem', { read: ElementRef }) foodItems!: QueryList<ElementRef>;
  isVisible: boolean[] = [];
  constructor(private router: Router,private renderer: Renderer2) {}
  navigateToMenuWithCategory(category: string): void {
    this.router.navigate(['/menu'], { queryParams: { category: category.toLowerCase() } });
  }


  ngAfterViewInit(): void {
    this.foodItems.forEach((elRef, i) => {
      const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            // Direct comparison to the nativeElement of each ElementRef
            if (entry.target === elRef.nativeElement) {
              this.isVisible[i] = true;
            }
            observer.unobserve(entry.target);
          }
        });
      }, { threshold: 0.1 });

      observer.observe(elRef.nativeElement);
    });
  }


  shouldSlideFromLeft(index: number): boolean {
    // Adjusted logic for 1st, 2nd, 6th, 7th elements (0-based index)
    return [0, 1, 4, 5].includes(index);
  }

  foods = [
    {
      name: 'Burgers',
      description: 'Beef patties, lettuce, tomatoes, sauces, sesame buns.',
      imageUrl: 'assets/images/burger-home.jpg',
      category: 'Burger',
    },
    {
      name: 'Pizza',
      description: 'Dough, tomato sauce, mozzarella, various toppings.',
      imageUrl: 'assets/images/pizza.jpg',
      category: 'Pizza',
    },
    {
      name: 'Pasta',
      description: 'Al dente pasta, creamy or tomato sauces, meats, veggies.',
      imageUrl: 'assets/images/pasta.jpg',
      category: 'Pasta',
    },
    {
      name: 'Sushi',
      description: 'Sliced fish, seasoned rice, seafood and veggie rolls.',
      imageUrl: 'assets/images/sushi.jpg',
      category: 'Sushi',
    },
    {
      name: 'Salad',
      description: 'Greens, veggies, nuts, fruits, dressings.',
      imageUrl: 'assets/images/salad.jpg',
      category: 'Salad',
    },
    {
      name: 'Tacos',
      description: 'Tortillas with meats, salsa, guacamole, cheese, lime.',
      imageUrl: 'assets/images/tacos.jpg',
      category: 'Tacos',
    },
    {
      name: 'Dessert',
      description: 'Cheesecakes, chocolates, sweet treats.',
      imageUrl: 'assets/images/dessert.jpg',
      category: 'Dessert',
    },
    {
      name: 'Other',
      description: 'Global cuisines, unique dishes, varied tastes.',
      imageUrl: 'assets/images/other.jpg',
      category: 'Other',
    },
  ];
}
