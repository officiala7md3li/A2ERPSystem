import { Component } from '@angular/core';
import { TuiButtonModule } from '@taiga-ui/core';
import { TuiTagModule } from '@taiga-ui/kit';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    TuiButtonModule,
    TuiTagModule
  ],
  template: `
    <div class="feature-container fade-in">
      <header class="feature-header">
        <h2>Reports</h2>
        <div class="actions-group">
          <button 
            tuiButton
            icon="tuiIconDownload"
            appearance="outline">
            Export
          </button>
          <button
            tuiButton
            icon="tuiIconPlus">
            New Report
          </button>
        </div>
      </header>

      <div class="content-section">
        <div class="reports-placeholder">
          <i class="tuiIconChartBar placeholder-icon"></i>
          <h3>No Reports Generated</h3>
          <p>Click "New Report" to create your first report</p>
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

    .reports-placeholder {
      @include flex-center;
      flex-direction: column;
      padding: $spacing-unit * 8;
      text-align: center;
      color: $text-secondary;

      .placeholder-icon {
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

    @media (max-width: 768px) {
      :host {
        margin: $spacing-unit * 2;
      }
    }
  `]
})
export class ReportsComponent {}