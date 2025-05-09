import { Component, inject } from '@angular/core';
import { NgFor } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Language } from '../../models/language.model';
import { LocalizationService } from '../../services/localization.service';
import { TuiButtonModule, TuiDataListModule, TuiHostedDropdownModule } from '@taiga-ui/core';
import { TuiTagModule, TuiInputModule } from '@taiga-ui/kit';
import { DialogService } from '@core/services/dialog.service';

@Component({
  selector: 'app-language-list',
  standalone: true,
  imports: [
    NgFor,
    RouterLink,
    TuiButtonModule,
    TuiDataListModule,
    TuiTagModule,
    TuiInputModule,
    TuiHostedDropdownModule
  ],
  template: `
    <div class="language-list">
      <div class="header">
        <h3>Supported Languages</h3>
        <button 
          tuiButton 
          type="button"
          appearance="primary"
          (click)="openAddLanguageDialog()">
          Add Language
        </button>
      </div>

      <div class="list">
        <div *ngFor="let language of languages" class="language-item">
          <div class="info">
            <h4>{{ language.name }}</h4>
            <tui-tag 
              [value]="language.code" 
              [status]="language.isDefault ? 'success' : 'info'">
            </tui-tag>
          </div>
          <div class="actions">
            <button 
              tuiButton 
              type="button"
              appearance="outline"
              size="s"
              (click)="editLanguage(language)">
              Edit
            </button>
            <button 
              tuiButton 
              type="button"
              appearance="outline"
              status="error"
              size="s"
              [disabled]="language.isDefault"
              (click)="deleteLanguage(language)">
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;

      h3 {
        margin: 0;
      }
    }

    .language-list {
      .list {
        display: grid;
        gap: 1rem;
      }

      .language-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem;
        background: var(--tui-base-02);
        border-radius: var(--tui-radius-m);

        .info {
          display: flex;
          align-items: center;
          gap: 1rem;

          h4 {
            margin: 0;
          }
        }

        .actions {
          display: flex;
          gap: 0.5rem;
        }
      }
    }

    @media (max-width: 768px) {
      .language-item {
        flex-direction: column;
        align-items: flex-start !important;
        gap: 1rem;

        .actions {
          width: 100%;
          justify-content: flex-end;
        }
      }
    }
  `]
})
export class LanguageListComponent {
  private localizationService = inject(LocalizationService);
  private dialogService = inject(DialogService);

  languages: Language[] = [];

  ngOnInit(): void {
    this.loadLanguages();
  }

  private loadLanguages(): void {
    this.localizationService.getLanguages().subscribe(
      languages => this.languages = languages
    );
  }

  openAddLanguageDialog(): void {
    this.dialogService.open('Add Language Dialog').subscribe();
  }

  editLanguage(language: Language): void {
    this.dialogService.open('Edit Language Dialog', { data: language }).subscribe();
  }

  deleteLanguage(language: Language): void {
    this.dialogService.openConfirm(
      `Are you sure you want to delete "${language.name}" language?`,
      { label: 'Delete Language' }
    ).subscribe(confirmed => {
      if (confirmed) {
        // Handle deletion
      }
    });
  }
}