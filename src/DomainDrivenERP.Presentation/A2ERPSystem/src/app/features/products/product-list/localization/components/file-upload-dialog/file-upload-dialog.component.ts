import { Component, ChangeDetectionStrategy, Inject } from '@angular/core';
import { POLYMORPHEUS_CONTEXT } from '@tinkoff/ng-polymorpheus';
import { TuiDialogContext } from '@taiga-ui/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TuiButtonModule, TuiErrorModule } from '@taiga-ui/core';

@Component({
  standalone: true,
  imports: [
    NgIf,
    FormsModule,
    TuiButtonModule,
    TuiErrorModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="dialog-content">
      <h3>Import Translations</h3>
      <p>Select a JSON file containing translations</p>
      
      <div class="file-upload">
        <input
          type="file"
          accept=".json"
          (change)="onFileSelected($event)"
          #fileInput>
        
        <button
          tuiButton
          icon="tuiIconUpload"
          appearance="outline"
          class="upload-button"
          (click)="fileInput.click()">
          Choose File
        </button>
        
        <span class="file-name" *ngIf="selectedFile">
          {{ selectedFile.name }}
        </span>
      </div>

      <div class="error-message" *ngIf="error">
        {{ error }}
      </div>

      <div class="dialog-actions">
        <button
          tuiButton
          appearance="outline"
          size="m"
          (click)="cancel()">
          Cancel
        </button>
        <button
          tuiButton
          size="m"
          [disabled]="!selectedFile"
          (click)="import()">
          Import
        </button>
      </div>
    </div>
  `,
  styles: [`
    .dialog-content {
      padding: 1.5rem;
      min-width: 400px;
    }

    h3 {
      margin: 0 0 0.5rem;
    }

    p {
      margin: 0 0 1.5rem;
      color: var(--tui-text-02);
    }

    .file-upload {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 1.5rem;

      input[type="file"] {
        display: none;
      }
    }

    .file-name {
      color: var(--tui-text-02);
      font-size: 0.875rem;
    }

    .error-message {
      color: var(--tui-error-fill);
      font-size: 0.875rem;
      margin-bottom: 1rem;
    }

    .dialog-actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
    }
  `]
})
export class FileUploadDialogComponent {
  selectedFile: File | null = null;
  error: string | null = null;

  constructor(
    @Inject(POLYMORPHEUS_CONTEXT)
    private readonly context: TuiDialogContext<File | null, void>
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (file.type !== 'application/json') {
        this.error = 'Please select a valid JSON file';
        this.selectedFile = null;
        return;
      }

      this.error = null;
      this.selectedFile = file;
    }
  }

  import(): void {
    if (this.selectedFile) {
      this.context.completeWith(this.selectedFile);
    }
  }

  cancel(): void {
    this.context.completeWith(null);
  }
}