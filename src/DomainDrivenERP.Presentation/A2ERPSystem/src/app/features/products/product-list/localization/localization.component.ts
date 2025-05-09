import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TuiButtonModule } from '@taiga-ui/core';
import { ConfigService } from '@core/services/config.service';

@Component({
  selector: 'app-localization',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    TuiButtonModule
  ],
  template: `
    <div class="feature-container fade-in">
      <header class="feature-header">
        <div>
          <h2>Localization Management</h2>
          <p class="subtitle">Manage languages and translations across your application</p>
        </div>
        <div class="actions-group">
          <button 
            tuiButton
            appearance="outline"
            icon="tuiIconUsers"
            [routerLink]="['languages']"
            routerLinkActive="active">
            Languages
          </button>
          <button 
            tuiButton
            appearance="outline"
            icon="tuiIconEdit"
            [routerLink]="['translations']"
            routerLinkActive="active">
            Translations
          </button>
          <div class="language-info" *ngIf="config.defaultLanguage">
            <tui-tag status="info">
              Default: {{ config.defaultLanguage }}
            </tui-tag>
          </div>
        </div>
      </header>

      <main class="feature-content">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    :host {
      display: block;
      margin: $spacing-unit * 4;
    }

    .subtitle {
      color: $text-secondary;
      margin-top: $spacing-unit;
    }

    .feature-content {
      margin-top: $spacing-unit * 4;
    }

    .language-info {
      margin-left: auto;
      padding-left: $spacing-unit * 4;
      border-left: 1px solid $background-secondary;
    }

    .active {
      background: $primary-color !important;
      color: white !important;
    }

    @media (max-width: 768px) {
      :host {
        margin: $spacing-unit * 2;
      }

      .actions-group {
        flex-direction: column;
        width: 100%;
      }

      .language-info {
        margin-left: 0;
        padding-left: 0;
        border-left: none;
        margin-top: $spacing-unit * 2;
        text-align: center;
      }
    }
  `]
})
export class LocalizationComponent {
  constructor(public config: ConfigService) {}
}