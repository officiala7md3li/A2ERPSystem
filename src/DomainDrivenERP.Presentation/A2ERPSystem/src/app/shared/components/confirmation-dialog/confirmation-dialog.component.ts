import { Component, Inject } from '@angular/core';
import { TuiDialogContext } from '@taiga-ui/core';
import { POLYMORPHEUS_CONTEXT } from '@tinkoff/ng-polymorpheus';

export interface ConfirmationDialogData {
  message: string;
  status?: 'info' | 'warning' | 'error' | 'success';
  confirmText: string;
  cancelText: string;
}

@Component({
  selector: 'app-confirmation-dialog',
  template: `
    <div class="confirmation-dialog">
      <div class="dialog-content" [ngClass]="context.data.status || 'info'">
        <p class="dialog-message">{{ context.data.message }}</p>
      </div>
      <div class="dialog-actions">
        <button
          tuiButton
          type="button"
          appearance="outline"
          size="m"
          (click)="onCancel()">
          {{ context.data.cancelText }}
        </button>
        <button
          tuiButton
          type="button"
          [appearance]="context.data.status === 'error' ? 'accent' : 'primary'"
          size="m"
          (click)="onConfirm()">
          {{ context.data.confirmText }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    .confirmation-dialog {
      padding: $spacing-unit * 4;
      min-width: 300px;
    }

    .dialog-content {
      margin-bottom: $spacing-unit * 4;
      padding: $spacing-unit * 3;
      border-radius: $border-radius;

      &.info {
        background: var(--tui-info-bg);
        color: var(--tui-info-fill);
      }

      &.warning {
        background: var(--tui-warning-bg);
        color: var(--tui-warning-fill);
      }

      &.error {
        background: var(--tui-error-bg);
        color: var(--tui-error-fill);
      }

      &.success {
        background: var(--tui-success-bg);
        color: var(--tui-success-fill);
      }
    }

    .dialog-message {
      margin: 0;
      font-size: 1rem;
      line-height: 1.5;
    }

    .dialog-actions {
      display: flex;
      justify-content: flex-end;
      gap: $spacing-unit * 2;
    }
  `]
})
export class ConfirmationDialogComponent {
  constructor(
    @Inject(POLYMORPHEUS_CONTEXT)
    readonly context: TuiDialogContext<boolean, Required<ConfirmationDialogData>>
  ) {}

  onConfirm(): void {
    this.context.completeWith(true);
  }

  onCancel(): void {
    this.context.completeWith(false);
  }
}