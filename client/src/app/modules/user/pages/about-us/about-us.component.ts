import { Component } from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-about-us',
  standalone: true,
  imports: [TranslateModule,CommonModule],
  templateUrl: './about-us.component.html',
  animations: [
    trigger('slideInDown', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }),
        animate('1s ease-out', style({ transform: 'translateY(0)', opacity: 1 })),
      ]),
    ]),
  ],
})
export class AboutUsComponent {
  translationData: any = {};

  constructor(private translate: TranslateService) {}

  ngOnInit(): void {
    this.loadTranslations();
    this.translate.onLangChange.subscribe(() => this.loadTranslations());
  }

  private loadTranslations(): void {
    this.translate.get([
      'ABOUT_US.UNIQUENESS.FEATURES',
      'ABOUT_US.GET_INVOLVED.OPPORTUNITIES'
    ]).subscribe(translations => {
      this.translationData = translations;
    });
  }
}
