import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AngularSvgIconModule } from 'angular-svg-icon';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [AngularSvgIconModule, RouterOutlet, RouterModule,TranslateModule],
  templateUrl: './footer.component.html',
})
export class FooterComponent {
}
