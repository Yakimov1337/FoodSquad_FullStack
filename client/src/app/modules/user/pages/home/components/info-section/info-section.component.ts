import { Component, AfterViewInit, ElementRef, ViewChild, NgZone } from '@angular/core';
import { trigger, transition, style, animate } from '@angular/animations';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-info-section',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './info-section.component.html',
  animations: [
    trigger('slideInLeft', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(-100%)' }),
        animate('1s', style({ opacity: 1, transform: 'translateX(0)' })),
      ]),
    ]),
    trigger('slideInRight', [
      transition('* => true', [
        style({ opacity: 0, transform: 'translateX(100%)' }),
        animate('1s', style({ opacity: 1, transform: 'translateX(0)' })),
      ]),
    ]),
  ],
})
export class InfoSectionComponent implements AfterViewInit {
  @ViewChild('firstImage') firstImage!: ElementRef;
  @ViewChild('secondImage') secondImage!: ElementRef;
  firstVisible = false;
  secondVisible = false;

  constructor(private ngZone: NgZone) {}

  ngAfterViewInit(): void {
    this.observeElement(this.firstImage.nativeElement, () => (this.firstVisible = true));
    this.observeElement(this.secondImage.nativeElement, () => (this.secondVisible = true));
  }

  private observeElement(element: Element, makeVisible: () => void): void {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          this.ngZone.run(makeVisible);
          observer.unobserve(element);
        }
      });
    }, { threshold: 0.5 });

    observer.observe(element);
  }
}
