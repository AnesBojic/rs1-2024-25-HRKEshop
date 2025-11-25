import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'HRKEShop';

  languages = [
    { code: 'bs', label: 'Bosanski' },
    { code: 'en', label: 'English' }
  ];

  // Store previous route as a string
  private previousRoute: string = '/products'; // default fallback

  constructor(private translate: TranslateService, private router: Router) {
    // Set default language
    this.translate.setDefaultLang('bs');
    this.translate.use('bs');
  }

  changeLanguage(lang: string): void {
    this.translate.use(lang);
  }

  openMaps(): void {
    if (this.router.url === '/maps') {
      // Go back to the previously saved route
      this.router.navigate([this.previousRoute]);
    } else {
      // Save current route before navigating to /maps
      this.previousRoute = this.router.url;
      this.router.navigate(['/maps']);
    }
  }
}
