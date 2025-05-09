import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TuiButtonModule } from '@taiga-ui/core';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [
    RouterLink,
    TuiButtonModule
  ],
  template: `
    <div class="not-found-container fade-in">
      <div class="not-found-content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you are looking for doesn't exist or has been moved.</p>
        
        <div class="actions">
          <button 
            tuiButton
            icon="tuiIconArrowLeft"
            appearance="outline"
            (click)="goBack()">
            Go Back
          </button>
          <button 
            tuiButton
            icon="tuiIconHome"
            [routerLink]="['/']">
            Return Home
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    .not-found-container {
      @include flex-center;
      min-height: calc(100vh - 200px);
      padding: $spacing-unit * 4;
      text-align: center;
    }

    .not-found-content {
      max-width: 600px;
      padding: $spacing-unit * 8;
      background: $background;
      border-radius: $border-radius-lg;
      box-shadow: var(--tui-shadow);
    }

    h1 {
      font-size: 6rem;
      margin: 0;
      color: $primary-color;
      line-height: 1;
    }

    h2 {
      font-size: 2rem;
      margin: $spacing-unit * 2 0;
      color: $text-color;
    }

    p {
      font-size: 1.125rem;
      color: $text-secondary;
      margin-bottom: $spacing-unit * 6;
    }

    .actions {
      display: flex;
      gap: $spacing-unit * 2;
      justify-content: center;
    }

    @media (max-width: 768px) {
      .not-found-container {
        padding: $spacing-unit * 2;
      }

      .not-found-content {
        padding: $spacing-unit * 4;
      }

      h1 {
        font-size: 4rem;
      }

      h2 {
        font-size: 1.5rem;
      }

      p {
        font-size: 1rem;
      }

      .actions {
        flex-direction: column;
      }
    }
  `]
})
export class NotFoundComponent {
  constructor(private router: Router) {}

  goBack(): void {
    this.router.navigate(['../']);
  }
}