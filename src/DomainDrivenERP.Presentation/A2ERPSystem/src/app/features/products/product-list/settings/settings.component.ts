import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { 
  TuiButtonModule,
  TuiDataListModule,
  TuiHostedDropdownModule
} from '@taiga-ui/core';
import { 
  TuiSelectModule,
  TuiInputModule,
  TuiTagModule
} from '@taiga-ui/kit';

interface Theme {
  id: string;
  name: string;
}

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    FormsModule,
    TuiButtonModule,
    TuiSelectModule,
    TuiInputModule,
    TuiTagModule,
    TuiDataListModule,
    TuiHostedDropdownModule
  ],
  template: `
    <div class="feature-container fade-in">
      <header class="feature-header">
        <h2>Settings</h2>
        <div class="actions-group">
          <button 
            tuiButton
            appearance="outline"
            icon="tuiIconRefresh">
            Reset to Default
          </button>
          <button
            tuiButton
            icon="tuiIconCheck"
            class="save-button">
            Save Changes
          </button>
        </div>
      </header>

      <div class="settings-grid">
        <!-- Appearance -->
        <div class="content-section">
          <h3 class="section-title">
            <i class="tuiIconPalette"></i>
            Appearance
          </h3>
          <div class="form-row">
            <label>Theme</label>
            <tui-select
              [(ngModel)]="selectedTheme"
              [valueContent]="selectedTheme?.name">
              Theme
              <tui-data-list>
                <button
                  *ngFor="let theme of themes"
                  tuiOption
                  [value]="theme">
                  {{ theme.name }}
                </button>
              </tui-data-list>
            </tui-select>
          </div>
          <div class="form-row form-row--inline">
            <label>Dark Mode</label>
            <button
              tuiButton
              [appearance]="darkMode ? 'primary' : 'outline'"
              (click)="darkMode = !darkMode"
              icon="tuiIconMoon">
              {{ darkMode ? 'Enabled' : 'Disabled' }}
            </button>
          </div>
        </div>

        <!-- Preferences -->
        <div class="content-section">
          <h3 class="section-title">
            <i class="tuiIconSettings"></i>
            Preferences
          </h3>
          <div class="form-row">
            <label>Language</label>
            <tui-select
              [(ngModel)]="selectedLanguage"
              [valueContent]="selectedLanguage">
              Language
              <tui-data-list>
                <button
                  *ngFor="let lang of languages"
                  tuiOption
                  [value]="lang">
                  {{ lang }}
                </button>
              </tui-data-list>
            </tui-select>
          </div>
          <div class="form-row form-row--inline">
            <label>Notifications</label>
            <button
              tuiButton
              [appearance]="notifications ? 'primary' : 'outline'"
              (click)="notifications = !notifications"
              icon="tuiIconBell">
              {{ notifications ? 'Enabled' : 'Disabled' }}
            </button>
          </div>
        </div>

        <!-- Account -->
        <div class="content-section">
          <h3 class="section-title">
            <i class="tuiIconUser"></i>
            Account
          </h3>
          <div class="form-row">
            <label>Email</label>
            <tui-input
              [(ngModel)]="email">
              Email address
            </tui-input>
          </div>
          <div class="form-actions">
            <button
              tuiButton
              appearance="outline"
              icon="tuiIconLock">
              Change Password
            </button>
          </div>
        </div>

        <!-- System -->
        <div class="content-section">
          <h3 class="section-title">
            <i class="tuiIconTool"></i>
            System
          </h3>
          <div class="system-info">
            <div class="info-item">
              <span class="info-label">Version</span>
              <span class="info-value">1.0.0</span>
            </div>
            <div class="info-item">
              <span class="info-label">Last Update</span>
              <span class="info-value">March 19, 2025</span>
            </div>
            <div class="info-item">
              <span class="info-label">Storage Used</span>
              <span class="info-value">234 MB</span>
            </div>
          </div>
          <div class="form-actions">
            <button
              tuiButton
              appearance="outline"
              icon="tuiIconTrash"
              class="danger-button">
              Clear Cache
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    :host {
      display: block;
      margin: $spacing-unit * 4;
    }

    .settings-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
      gap: $spacing-unit * 4;
    }

    .section-title {
      display: flex;
      align-items: center;
      gap: $spacing-unit * 2;
      margin: 0 0 $spacing-unit * 4;
      font-size: 1.125rem;
      color: $text-color;

      i {
        font-size: 1.25rem;
        color: $primary-color;
      }
    }

    .form-row {
      margin-bottom: $spacing-unit * 4;

      label {
        display: block;
        margin-bottom: $spacing-unit * 2;
        color: $text-secondary;
        font-size: 0.875rem;
      }
    }

    .form-row--inline {
      display: flex;
      justify-content: space-between;
      align-items: center;

      label {
        margin-bottom: 0;
      }
    }

    .system-info {
      margin-bottom: $spacing-unit * 4;
    }

    .info-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: $spacing-unit * 2 0;
      border-bottom: 1px solid $background-secondary;

      &:last-child {
        border-bottom: none;
      }
    }

    .info-label {
      color: $text-secondary;
      font-size: 0.875rem;
    }

    .info-value {
      font-weight: 500;
    }

    .save-button {
      background: var(--tui-success-fill);
    }

    .danger-button {
      color: var(--tui-error-fill);
      border-color: var(--tui-error-fill);

      &:hover {
        background: var(--tui-error-bg);
      }
    }

    @media (max-width: 768px) {
      :host {
        margin: $spacing-unit * 2;
      }

      .settings-grid {
        grid-template-columns: 1fr;
      }

      .form-row--inline {
        flex-direction: column;
        align-items: stretch;
        gap: $spacing-unit * 2;
      }
    }
  `]
})
export class SettingsComponent {
  themes: Theme[] = [
    { id: 'light', name: 'Light Theme' },
    { id: 'dark', name: 'Dark Theme' },
    { id: 'system', name: 'System Default' }
  ];
  selectedTheme = this.themes[0];
  darkMode = false;
  
  languages = ['English', 'French', 'German', 'Spanish'];
  selectedLanguage = 'English';
  
  notifications = true;
  email = 'admin@example.com';
}