import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf, KeyValuePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { 
  TuiButtonModule, 
  TuiDataListModule,
  TuiLoaderModule,
  TuiHostedDropdownModule,
  TuiDialogService
} from '@taiga-ui/core';
import { 
  TuiInputModule,
  TuiTagModule,
  TuiSelectModule,
  TuiTextAreaModule
} from '@taiga-ui/kit';

import { LocalizationService } from '../../services/localization.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { Language } from '../../models/language.model';
import { FileUploadDialogComponent } from '../file-upload-dialog/file-upload-dialog.component';
import { PolymorpheusComponent } from '@tinkoff/ng-polymorpheus';

interface TranslationItem {
  key: string;
  value: string;
  isModified?: boolean;
  isNew?: boolean;
}

@Component({
  selector: 'app-translations',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    FormsModule,
    KeyValuePipe,
    TuiButtonModule,
    TuiInputModule,
    TuiTagModule,
    TuiSelectModule,
    TuiTextAreaModule,
    TuiDataListModule,
    TuiLoaderModule,
    TuiHostedDropdownModule
  ],
  template: `
    <div class="feature-container fade-in">
      <header class="feature-header">
        <div>
          <h2>Translations</h2>
          <p class="subtitle">Manage translations for each language</p>
        </div>
        <div class="actions-group">
          <button 
            tuiButton
            appearance="outline"
            icon="tuiIconDownload"
            (click)="exportTranslations()"
            [disabled]="!selectedLanguage">
            Export
          </button>
          <button
            tuiButton
            appearance="outline"
            icon="tuiIconUpload"
            (click)="importTranslations()"
            [disabled]="!selectedLanguage">
            Import
          </button>
          <button
            tuiButton
            icon="tuiIconPlus"
            (click)="addTranslation()"
            [disabled]="!selectedLanguage">
            Add Translation
          </button>
        </div>
      </header>

      <!-- Language Selection -->
      <div class="content-section">
        <div class="language-selector">
          <tui-select
            [(ngModel)]="selectedLanguage"
            (ngModelChange)="onLanguageChange($event)"
            [style.maxWidth.px]="300">
            Select Language
            <tui-data-list>
              <button
                *ngFor="let lang of availableLanguages"
                tuiOption
                [value]="lang">
                <span class="language-option">
                  <i [class]="lang.flagIcon || 'tuiIconWorld'"></i>
                  {{ lang.name }}
                  <tui-tag
                    *ngIf="lang.isDefault"
                    status="success"
                    size="s">
                    Default
                  </tui-tag>
                </span>
              </button>
            </tui-data-list>
          </tui-select>
          
          <div class="translation-stats" *ngIf="selectedLanguage">
            <tui-tag [status]="getProgressStatus()">
              {{ translationCount }} Keys
            </tui-tag>
            <tui-tag [status]="getProgressStatus()">
              {{ translationProgress }}% Complete
            </tui-tag>
          </div>
        </div>

        <!-- Search and Filter -->
        <div class="search-bar" *ngIf="selectedLanguage">
          <tui-input
            [(ngModel)]="searchQuery"
            (ngModelChange)="filterTranslations()">
            Search translations...
          </tui-input>
        </div>

        <!-- Translations List -->
        <div class="translations-list" *ngIf="selectedLanguage">
          <div 
            *ngFor="let item of filteredTranslations"
            class="translation-item">
            <div class="translation-key">
              <tui-input
                [(ngModel)]="item.key"
                (ngModelChange)="markAsModified(item)"
                [disabled]="!item.isNew">
                Translation key
              </tui-input>
            </div>
            <div class="translation-value">
              <tui-text-area
                [(ngModel)]="item.value"
                (ngModelChange)="markAsModified(item)">
                Translation value
              </tui-text-area>
            </div>
            <div class="translation-actions">
              <button
                tuiButton
                appearance="outline"
                size="s"
                icon="tuiIconTrash"
                class="danger"
                (click)="removeTranslation(item)">
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div class="empty-state" *ngIf="!selectedLanguage">
          <i class="tuiIconLanguage empty-icon"></i>
          <h3>Select a Language</h3>
          <p>Choose a language to manage its translations</p>
        </div>

        <!-- Loading State -->
        <div class="loader-container" *ngIf="loading">
          <tui-loader size="l"></tui-loader>
        </div>

        <!-- Save Changes -->
        <div 
          class="save-changes"
          *ngIf="hasModifications">
          <span>You have unsaved changes</span>
          <div class="save-actions">
            <button
              tuiButton
              appearance="outline"
              size="m"
              (click)="discardChanges()">
              Discard
            </button>
            <button
              tuiButton
              size="m"
              (click)="saveChanges()">
              Save Changes
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @import '../../../../shared/styles/base';

    :host {
      display: block;
      margin: $spacing-unit * 4;
    }

    .subtitle {
      color: $text-secondary;
      margin-top: $spacing-unit;
    }

    .language-selector {
      display: flex;
      align-items: center;
      gap: $spacing-unit * 4;
      margin-bottom: $spacing-unit * 4;
    }

    .language-option {
      display: flex;
      align-items: center;
      gap: $spacing-unit * 2;

      i {
        font-size: 1.25rem;
        color: $primary-color;
      }
    }

    .translation-stats {
      display: flex;
      gap: $spacing-unit * 2;
    }

    .search-bar {
      margin-bottom: $spacing-unit * 4;
    }

    .translations-list {
      display: flex;
      flex-direction: column;
      gap: $spacing-unit * 3;
      margin-bottom: $spacing-unit * 4;
    }

    .translation-item {
      display: grid;
      grid-template-columns: 2fr 3fr auto;
      gap: $spacing-unit * 3;
      padding: $spacing-unit * 3;
      background: $background;
      border-radius: $border-radius;
      box-shadow: var(--tui-shadow);
      transition: transform 0.2s;

      &:hover {
        transform: translateY(-2px);
      }
    }

    .translation-key {
      position: relative;

      &::after {
        content: '';
        position: absolute;
        top: 50%;
        right: -$spacing-unit * 1.5;
        width: 1px;
        height: 24px;
        background: $background-secondary;
        transform: translateY(-50%);
      }
    }

    .translation-value {
      flex: 1;
    }

    .translation-actions {
      display: flex;
      align-items: flex-start;
    }

    .empty-state {
      @include flex-center;
      flex-direction: column;
      padding: $spacing-unit * 8;
      text-align: center;
      color: $text-secondary;

      .empty-icon {
        font-size: 4rem;
        margin-bottom: $spacing-unit * 4;
        opacity: 0.5;
      }

      h3 {
        margin: 0 0 $spacing-unit;
        font-size: 1.25rem;
        color: $text-color;
      }

      p {
        margin: 0;
        font-size: 0.875rem;
      }
    }

    .save-changes {
      position: fixed;
      bottom: $spacing-unit * 4;
      right: $spacing-unit * 4;
      display: flex;
      align-items: center;
      gap: $spacing-unit * 4;
      padding: $spacing-unit * 3 $spacing-unit * 4;
      background: var(--tui-primary);
      color: var(--tui-primary-text);
      border-radius: $border-radius;
      box-shadow: var(--tui-shadow);

      .save-actions {
        display: flex;
        gap: $spacing-unit * 2;
      }
    }

    .danger {
      color: var(--tui-error-fill);
      border-color: var(--tui-error-fill);

      &:hover {
        background: var(--tui-error-bg);
      }
    }

    @media (max-width: 1024px) {
      .translation-item {
        grid-template-columns: 1fr;
        gap: $spacing-unit * 2;
      }

      .translation-key {
        &::after {
          display: none;
        }
      }
    }

    @media (max-width: 768px) {
      :host {
        margin: $spacing-unit * 2;
      }

      .language-selector {
        flex-direction: column;
        align-items: stretch;
        gap: $spacing-unit * 2;
      }

      .save-changes {
        left: $spacing-unit * 2;
        right: $spacing-unit * 2;
        flex-direction: column;
        gap: $spacing-unit * 2;
      }
    }
  `]
})
export class TranslationsComponent implements OnInit {
  availableLanguages: Language[] = [];
  selectedLanguage: Language | null = null;
  translations: TranslationItem[] = [];
  filteredTranslations: TranslationItem[] = [];
  originalTranslations: { [key: string]: string } = {};
  
  searchQuery = '';
  loading = false;
  hasModifications = false;

  get translationCount(): number {
    return this.translations.length;
  }

  get translationProgress(): number {
    if (!this.translations.length) return 0;
    const nonEmptyTranslations = this.translations.filter(t => t.value.trim() !== '').length;
    return Math.round((nonEmptyTranslations / this.translations.length) * 100);
  }

  constructor(
    private localizationService: LocalizationService,
    private dialogService: TuiDialogService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadLanguages();
  }

  loadLanguages(): void {
    this.loading = true;
    this.localizationService.getLanguages()
      .subscribe({
        next: (languages) => {
          this.availableLanguages = languages;
          this.loading = false;
        },
        error: (error) => {
          this.loading = false;
          this.notificationService.showError('Failed to load languages: ' + error.message);
        }
      });
  }

  onLanguageChange(language: Language): void {
    this.loading = true;
    this.localizationService.getTranslations(language.code)
      .subscribe({
        next: (translations) => {
          this.originalTranslations = translations;
          this.translations = Object.entries(translations).map(([key, value]) => ({
            key,
            value
          }));
          this.filterTranslations();
          this.loading = false;
        },
        error: (error) => {
          this.loading = false;
          this.notificationService.showError('Failed to load translations: ' + error.message);
        }
      });
  }

  filterTranslations(): void {
    const query = this.searchQuery.toLowerCase();
    this.filteredTranslations = this.translations.filter(item => 
      item.key.toLowerCase().includes(query) || 
      item.value.toLowerCase().includes(query)
    );
  }

  addTranslation(): void {
    const newItem: TranslationItem = {
      key: '',
      value: '',
      isModified: true,
      isNew: true
    };
    this.translations.unshift(newItem);
    this.filteredTranslations.unshift(newItem);
    this.hasModifications = true;
  }

  removeTranslation(item: TranslationItem): void {
    this.translations = this.translations.filter(t => t !== item);
    this.filterTranslations();
    this.hasModifications = true;
  }

  markAsModified(item: TranslationItem): void {
    item.isModified = true;
    this.hasModifications = true;
  }

  discardChanges(): void {
    this.onLanguageChange(this.selectedLanguage!);
    this.hasModifications = false;
    this.notificationService.showInfo('Changes discarded');
  }

  saveChanges(): void {
    if (!this.selectedLanguage) return;

    const translations: { [key: string]: string } = {};
    this.translations.forEach(item => {
      if (item.key.trim()) {
        translations[item.key] = item.value;
      }
    });

    this.localizationService.importTranslations(this.selectedLanguage.code, JSON.stringify(translations))
      .subscribe({
        next: () => {
          this.hasModifications = false;
          this.onLanguageChange(this.selectedLanguage!);
          this.notificationService.showSuccess('Translations saved successfully');
        },
        error: (error) => {
          this.notificationService.showError('Failed to save translations: ' + error.message);
        }
      });
  }

  exportTranslations(): void {
    if (!this.selectedLanguage) return;

    this.localizationService.exportTranslations(this.selectedLanguage.code)
      .subscribe({
        next: () => {
          this.notificationService.showSuccess('Translations exported successfully');
        },
        error: (error) => {
          this.notificationService.showError('Failed to export translations: ' + error.message);
        }
      });
  }

  importTranslations(): void {
    if (!this.selectedLanguage) return;

    const dialog = this.dialogService.open<File | null>(
      new PolymorpheusComponent(FileUploadDialogComponent),
      { size: 's' }
    );

    dialog.subscribe({
      next: (file: File | null) => {
        if (file) {
          this.loading = true;
          this.localizationService
            .uploadTranslations(this.selectedLanguage!.code, file)
            .subscribe({
              next: () => {
                this.loading = false;
                this.onLanguageChange(this.selectedLanguage!);
                this.notificationService.showSuccess('Translations imported successfully');
              },
              error: (error) => {
                this.loading = false;
                this.notificationService.showError('Failed to import translations: ' + error.message);
              }
            });
        }
      }
    });
  }

  getProgressStatus(): string {
    const progress = this.translationProgress;
    if (progress >= 90) return 'success';
    if (progress >= 50) return 'warning';
    return 'error';
  }
}