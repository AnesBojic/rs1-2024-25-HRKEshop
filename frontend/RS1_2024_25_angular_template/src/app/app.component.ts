import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrls: ['./app.component.css'] // ispravljeno u množinu
})
export class AppComponent {
  title = 'HRKEShop';

  languages = [
    { code: 'bs', label: 'Bosanski' },
    { code: 'en', label: 'English' }
  ];

  constructor(private translate: TranslateService) {
    // Postavi default jezik
    this.translate.setDefaultLang('bs');
    this.translate.use('bs');
  }

  changeLanguage(lang: string): void {
    this.translate.use(lang);
  }
}
