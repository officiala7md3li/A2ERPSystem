import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgFor } from '@angular/common';
import { 
  TuiButtonModule,
  TuiDataListModule,
  TuiLoaderModule,
  TuiHostedDropdownModule
} from '@taiga-ui/core';
import { TuiTagModule } from '@taiga-ui/kit';
import { ConfigService } from '@core/services/config.service';

interface QuickAction {
  label: string;
  icon: string;
  route: string;
  color?: string;
}

interface ActivityItem {
  type: string;
  title: string;
  description: string;
  timestamp: Date;
  user: string;
  userAvatar?: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    NgFor,
    RouterLink,
    TuiButtonModule,
    TuiTagModule,
    TuiHostedDropdownModule,
    TuiLoaderModule,
    TuiDataListModule
  ],
  template: `
    <div class="dashboard-container fade-in">
      <!-- Welcome Section -->
      <header class="feature-header">
        <div>
          <h2>Welcome to Domain Driven ERP</h2>
          <p class="subtitle">Manage your enterprise resources efficiently</p>
        </div>
        <div class="actions-group">
          <button 
            tuiButton
            icon="tuiIconPlus"
            [routerLink]="['/new-transaction']">
            New Transaction
          </button>
        </div>
      </header>

      <!-- Quick Actions -->
      <section class="quick-actions grid-layout">
        <div
          *ngFor="let action of quickActions"
          class="quick-action-card"
          [routerLink]="[action.route]">
          <div class="quick-action-content">
            <i [class]="action.icon" [style.color]="action.color"></i>
            <h3>{{ action.label }}</h3>
          </div>
        </div>
      </section>

      <!-- Dashboard Grid -->
      <div class="dashboard-grid">
        <!-- Recent Activity -->
        <div class="panel activity-panel">
          <h3 class="panel-title">Recent Activity</h3>
          <div class="activity-scroll">
            <div class="activity-list">
              <div *ngFor="let item of recentActivity" class="activity-item">
                <div class="avatar">
                  {{ item.user.charAt(0) }}
                </div>
                <div class="activity-content">
                  <div class="activity-header">
                    <strong>{{ item.user }}</strong>
                    <tui-tag status="info" size="s">
                      {{ item.type }}
                    </tui-tag>
                  </div>
                  <p class="activity-title">{{ item.title }}</p>
                  <p class="activity-description">{{ item.description }}</p>
                  <small class="activity-time">
                    {{ item.timestamp | date:'medium' }}
                  </small>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Stats Panel -->
        <div class="panel stats-panel">
          <h3 class="panel-title">System Overview</h3>
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-label">Active Users</span>
              <div class="stat-value">
                <i class="tuiIconUsers"></i>
                <strong>2,451</strong>
              </div>
            </div>
            <div class="stat-item">
              <span class="stat-label">Transactions Today</span>
              <div class="stat-value">
                <i class="tuiIconDollarSign"></i>
                <strong>186</strong>
              </div>
            </div>
            <div class="stat-item">
              <span class="stat-label">Languages</span>
              <div class="stat-value">
                <i class="tuiIconWorld"></i>
                <strong>12</strong>
              </div>
            </div>
            <div class="stat-item">
              <span class="stat-label">System Status</span>
              <div class="stat-value">
                <i class="tuiIconCheck" style="color: var(--tui-success-fill)"></i>
                <strong>Healthy</strong>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    .dashboard-container {
      padding: $spacing-unit * 4;
    }

    .subtitle {
      color: $text-secondary;
      margin-top: $spacing-unit;
    }

    .quick-actions {
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      margin: $spacing-unit * 4 0;
    }

    .quick-action-card {
      background: $background;
      border-radius: $border-radius;
      box-shadow: var(--tui-shadow);
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;

      &:hover {
        transform: translateY(-2px);
        box-shadow: var(--tui-shadow-hover);
      }
    }

    .quick-action-content {
      @include flex-center;
      flex-direction: column;
      text-align: center;
      padding: $spacing-unit * 4;

      i {
        font-size: 2rem;
        margin-bottom: $spacing-unit * 2;
      }

      h3 {
        margin: 0;
        font-size: 1rem;
      }
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: $spacing-unit * 4;
      margin-top: $spacing-unit * 4;
    }

    .panel {
      background: $background;
      border-radius: $border-radius;
      padding: $spacing-unit * 4;
      box-shadow: var(--tui-shadow);
    }

    .panel-title {
      font-size: 1.25rem;
      font-weight: 500;
      margin: 0 0 $spacing-unit * 4;
      color: $text-color;
    }

    .activity-panel {
      height: 500px;
      display: flex;
      flex-direction: column;
    }

    .activity-scroll {
      flex: 1;
      overflow-y: auto;
      margin: 0 -$spacing-unit * 4;
      padding: 0 $spacing-unit * 4;

      &::-webkit-scrollbar {
        width: 8px;
      }

      &::-webkit-scrollbar-track {
        background: $background-secondary;
        border-radius: $border-radius;
      }

      &::-webkit-scrollbar-thumb {
        background: var(--tui-secondary);
        border-radius: $border-radius;
      }
    }

    .activity-list {
      padding: $spacing-unit * 2 0;
    }

    .activity-item {
      display: flex;
      gap: $spacing-unit * 2;
      padding: $spacing-unit * 2;
      border-bottom: 1px solid $background-secondary;
      transition: background-color 0.2s;

      &:last-child {
        border-bottom: none;
      }

      &:hover {
        background: $background-secondary;
      }
    }

    .avatar {
      width: 32px;
      height: 32px;
      border-radius: 50%;
      background: $primary-color;
      color: white;
      @include flex-center;
      font-weight: 500;
      font-size: 14px;
    }

    .activity-content {
      flex: 1;
    }

    .activity-header {
      @include flex-between;
      margin-bottom: $spacing-unit;
    }

    .activity-title {
      margin: 0;
      font-weight: 500;
    }

    .activity-description {
      margin: $spacing-unit 0;
      color: $text-secondary;
      font-size: 0.875rem;
    }

    .activity-time {
      color: $text-secondary;
      font-size: 0.75rem;
    }

    .stats-panel {
      height: 500px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: $spacing-unit * 4;
      padding: $spacing-unit * 2;
    }

    .stat-item {
      background: $background-secondary;
      padding: $spacing-unit * 3;
      border-radius: $border-radius;
      display: flex;
      flex-direction: column;
      gap: $spacing-unit;
      transition: transform 0.2s;

      &:hover {
        transform: translateY(-2px);
      }
    }

    .stat-label {
      font-size: 0.875rem;
      color: $text-secondary;
    }

    .stat-value {
      @include flex-center;
      gap: $spacing-unit * 2;

      i {
        font-size: 1.5rem;
        color: $primary-color;
      }

      strong {
        font-size: 1.25rem;
      }
    }

    @media (max-width: 1024px) {
      .dashboard-grid {
        grid-template-columns: 1fr;
      }
    }

    @media (max-width: 768px) {
      .dashboard-container {
        padding: $spacing-unit * 2;
      }

      .stats-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DashboardComponent {
  quickActions: QuickAction[] = [
    {
      label: 'Manage Languages',
      icon: 'tuiIconWorld',
      route: '/localization/languages',
      color: '#7B61FF'
    },
    {
      label: 'View Reports',
      icon: 'tuiIconChartBar',
      route: '/reports',
      color: '#2EC4B6'
    },
    {
      label: 'User Settings',
      icon: 'tuiIconSettings',
      route: '/settings',
      color: '#FF9F1C'
    },
    {
      label: 'Help Center',
      icon: 'tuiIconHelpCircle',
      route: '/help',
      color: '#E71D36'
    }
  ];

  recentActivity: ActivityItem[] = [
    {
      type: 'Update',
      title: 'Updated French translations',
      description: 'Modified 15 translation keys in the French language pack',
      timestamp: new Date(Date.now() - 1000 * 60 * 30),
      user: 'Sarah Chen'
    },
    {
      type: 'Create',
      title: 'Added new language',
      description: 'German language support has been added to the system',
      timestamp: new Date(Date.now() - 1000 * 60 * 60 * 2),
      user: 'Mike Johnson'
    },
    {
      type: 'Delete',
      title: 'Removed obsolete translations',
      description: 'Cleaned up 25 unused translation keys',
      timestamp: new Date(Date.now() - 1000 * 60 * 60 * 24),
      user: 'Alex Wong'
    }
  ];

  constructor(public config: ConfigService) {}
}