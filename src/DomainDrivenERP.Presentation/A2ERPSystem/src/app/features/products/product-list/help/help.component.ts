import { Component } from '@angular/core';
import { NgFor } from '@angular/common';
import { 
  TuiButtonModule,
  TuiDataListModule
} from '@taiga-ui/core';
import { TuiTagModule } from '@taiga-ui/kit';

interface FaqItem {
  question: string;
  answer: string;
  category: string;
  isExpanded?: boolean;
}

@Component({
  selector: 'app-help',
  standalone: true,
  imports: [
    NgFor,
    TuiButtonModule,
    TuiTagModule,
    TuiDataListModule
  ],
  template: `
    <div class="feature-container fade-in">
      <header class="feature-header">
        <div>
          <h2>Help Center</h2>
          <p class="subtitle">Find answers to common questions and get support</p>
        </div>
        <div class="actions-group">
          <button 
            tuiButton
            icon="tuiIconMessage"
            appearance="outline">
            Contact Support
          </button>
          <button
            tuiButton
            icon="tuiIconVideo">
            Watch Tutorial
          </button>
        </div>
      </header>

      <!-- Quick Help -->
      <div class="quick-help grid-layout">
        <div class="help-card" (click)="openDocs()">
          <i class="tuiIconBook"></i>
          <h3>Documentation</h3>
          <p>Browse detailed guides and documentation</p>
        </div>
        <div class="help-card" (click)="openTutorials()">
          <i class="tuiIconVideo"></i>
          <h3>Video Tutorials</h3>
          <p>Learn through step-by-step video guides</p>
        </div>
        <div class="help-card" (click)="openCommunity()">
          <i class="tuiIconUsers"></i>
          <h3>Community Forum</h3>
          <p>Connect with other users and share knowledge</p>
        </div>
        <div class="help-card" (click)="openSupport()">
          <i class="tuiIconLifebuoy"></i>
          <h3>Support Ticket</h3>
          <p>Get direct help from our support team</p>
        </div>
      </div>

      <!-- FAQ Sections -->
      <section class="content-section">
        <h3 class="section-title">Frequently Asked Questions</h3>
        
        <div class="faq-categories">
          <button
            *ngFor="let category of categories"
            tuiButton
            type="button"
            [appearance]="selectedCategory === category ? 'primary' : 'outline'"
            (click)="selectedCategory = category">
            {{ category }}
          </button>
        </div>

        <div class="faq-list">
          <div
            *ngFor="let item of filteredFaqs; let i = index"
            class="faq-item"
            [class.expanded]="item.isExpanded"
            (click)="toggleFaq(item)">
            <div class="faq-question">
              <span>{{ item.question }}</span>
              <div class="faq-controls">
                <tui-tag
                  [status]="getTagStatus(item.category)"
                  class="category-tag">
                  {{ item.category }}
                </tui-tag>
                <i 
                  [class]="item.isExpanded ? 'tuiIconChevronUp' : 'tuiIconChevronDown'"
                  class="expand-icon">
                </i>
              </div>
            </div>
            <div class="faq-answer" *ngIf="item.isExpanded">
              {{ item.answer }}
            </div>
          </div>
        </div>
      </section>

      <!-- Contact Info -->
      <div class="contact-section">
        <div class="contact-card">
          <i class="tuiIconClock"></i>
          <div class="contact-info">
            <h4>Support Hours</h4>
            <p>Monday - Friday</p>
            <p>9:00 AM - 6:00 PM GMT</p>
          </div>
        </div>
        <div class="contact-card">
          <i class="tuiIconMail"></i>
          <div class="contact-info">
            <h4>Email Support</h4>
            <p>support@example.com</p>
            <p>Response within 24 hours</p>
          </div>
        </div>
        <div class="contact-card">
          <i class="tuiIconPhone"></i>
          <div class="contact-info">
            <h4>Phone Support</h4>
            <p>+1 (555) 123-4567</p>
            <p>Premium support subscribers only</p>
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

    .subtitle {
      color: $text-secondary;
      margin-top: $spacing-unit;
    }

    .quick-help {
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      margin: $spacing-unit * 4 0;
    }

    .help-card {
      background: $background;
      border-radius: $border-radius;
      padding: $spacing-unit * 4;
      text-align: center;
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;
      box-shadow: var(--tui-shadow);

      &:hover {
        transform: translateY(-2px);
        box-shadow: var(--tui-shadow-hover);
      }

      i {
        font-size: 2rem;
        color: $primary-color;
        margin-bottom: $spacing-unit * 2;
      }

      h3 {
        margin: 0 0 $spacing-unit;
        font-size: 1.125rem;
      }

      p {
        margin: 0;
        color: $text-secondary;
        font-size: 0.875rem;
      }
    }

    .section-title {
      font-size: 1.25rem;
      font-weight: 500;
      margin: 0 0 $spacing-unit * 4;
    }

    .faq-categories {
      display: flex;
      gap: $spacing-unit * 2;
      margin-bottom: $spacing-unit * 4;
      flex-wrap: wrap;
    }

    .faq-item {
      margin-bottom: $spacing-unit * 2;
      background: $background;
      border-radius: $border-radius;
      overflow: hidden;
      cursor: pointer;
      transition: box-shadow 0.2s;

      &:hover {
        box-shadow: var(--tui-shadow-hover);
      }

      &.expanded {
        .faq-question {
          background: $background-secondary;
        }
      }
    }

    .faq-question {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: $spacing-unit * 3;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .faq-controls {
      display: flex;
      align-items: center;
      gap: $spacing-unit * 2;
    }

    .expand-icon {
      font-size: 1.25rem;
      color: $text-secondary;
      transition: transform 0.2s;
    }

    .faq-answer {
      padding: $spacing-unit * 3;
      color: $text-secondary;
      border-top: 1px solid $background-secondary;
      animation: slideDown 0.2s ease-out;
    }

    @keyframes slideDown {
      from {
        opacity: 0;
        transform: translateY(-10px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .category-tag {
      font-size: 0.75rem;
    }

    .contact-section {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: $spacing-unit * 4;
      margin-top: $spacing-unit * 6;
    }

    .contact-card {
      display: flex;
      align-items: flex-start;
      gap: $spacing-unit * 3;
      padding: $spacing-unit * 4;
      background: $background;
      border-radius: $border-radius;
      box-shadow: var(--tui-shadow);

      i {
        font-size: 1.5rem;
        color: $primary-color;
      }

      h4 {
        margin: 0 0 $spacing-unit;
        font-size: 1rem;
      }

      p {
        margin: 0;
        color: $text-secondary;
        font-size: 0.875rem;
        line-height: 1.4;
      }
    }

    @media (max-width: 768px) {
      :host {
        margin: $spacing-unit * 2;
      }

      .faq-categories {
        flex-direction: column;
      }

      .contact-section {
        grid-template-columns: 1fr;
      }

      .faq-controls {
        flex-direction: column;
        align-items: flex-end;
        gap: $spacing-unit;
      }
    }
  `]
})
export class HelpComponent {
  categories = ['General', 'Account', 'Features', 'Billing'];
  selectedCategory = 'General';

  faqs: FaqItem[] = [
    {
      question: 'How do I get started with the system?',
      answer: 'Begin by setting up your account preferences and exploring the dashboard. Our quick start guide provides step-by-step instructions for initial setup.',
      category: 'General'
    },
    {
      question: 'How do I change my password?',
      answer: 'Go to Settings > Account and click on "Change Password". Follow the prompts to update your password.',
      category: 'Account'
    },
    {
      question: 'What payment methods are accepted?',
      answer: 'We accept all major credit cards, PayPal, and bank transfers for business accounts.',
      category: 'Billing'
    },
    {
      question: 'How do I add a new language?',
      answer: 'Navigate to the Localization section and click "Add Language". Select the desired language and import your translations.',
      category: 'Features'
    }
  ];

  get filteredFaqs(): FaqItem[] {
    return this.faqs.filter(faq => 
      this.selectedCategory === 'All' || faq.category === this.selectedCategory
    );
  }

  toggleFaq(item: FaqItem): void {
    item.isExpanded = !item.isExpanded;
  }

  getTagStatus(category: string): string {
    const statusMap: { [key: string]: string } = {
      'General': 'info',
      'Account': 'success',
      'Features': 'warning',
      'Billing': 'error'
    };
    return statusMap[category] || 'default';
  }

  openDocs(): void {
    window.open('/docs', '_blank');
  }

  openTutorials(): void {
    window.open('/tutorials', '_blank');
  }

  openCommunity(): void {
    window.open('/community', '_blank');
  }

  openSupport(): void {
    window.open('/support', '_blank');
  }
}