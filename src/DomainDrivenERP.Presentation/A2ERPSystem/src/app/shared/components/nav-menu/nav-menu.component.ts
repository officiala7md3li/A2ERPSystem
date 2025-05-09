import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgFor, NgIf } from '@angular/common';
import { TuiButtonModule, TuiDataListModule, TuiHostedDropdownModule } from '@taiga-ui/core';

interface NavMenuItem {
  label: string;
  icon?: string;
  route?: string;
  children?: NavMenuItem[];
}

@Component({
  selector: 'app-nav-menu',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    RouterLink,
    RouterLinkActive,
    TuiButtonModule,
    TuiDataListModule,
    TuiHostedDropdownModule
  ],
  template: `
    <nav class="nav-menu">
      <div class="brand">
        <a routerLink="/" class="brand-link">
          <span class="brand-name">{{ appName }}</span>
        </a>
      </div>

      <div class="nav-items">
        <ng-container *ngFor="let item of menuItems">
          <ng-container *ngIf="!item.children; else dropdownMenu">
            <a
              [routerLink]="item.route"
              routerLinkActive="active"
              class="nav-item"
              [title]="item.label">
              <i *ngIf="item.icon" [class]="item.icon"></i>
              <span>{{ item.label }}</span>
            </a>
          </ng-container>

          <ng-template #dropdownMenu>
            <tui-hosted-dropdown
              [content]="dropdownContent"
              class="nav-item">
              <button
                tuiButton
                type="button"
                appearance="flat"
                [iconRight]="'tuiIconChevronDown'"
                class="dropdown-button">
                <i *ngIf="item.icon" [class]="item.icon"></i>
                {{ item.label }}
              </button>

              <ng-template #dropdownContent>
                <tui-data-list>
                  <button
                    *ngFor="let child of item.children"
                    tuiOption
                    [routerLink]="child.route"
                    routerLinkActive="active"
                    class="dropdown-item">
                    <i *ngIf="child.icon" [class]="child.icon"></i>
                    <span>{{ child.label }}</span>
                  </button>
                </tui-data-list>
              </ng-template>
            </tui-hosted-dropdown>
          </ng-template>
        </ng-container>
      </div>
    </nav>
  `,
  styles: [`
    @import '../../../shared/styles/base';

    .nav-menu {
      display: flex;
      align-items: center;
      background: var(--tui-base-01);
      padding: $spacing-unit * 2 $spacing-unit * 4;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .brand {
      margin-right: $spacing-unit * 4;

      .brand-link {
        text-decoration: none;
        color: var(--tui-text-01);
      }

      .brand-name {
        font-size: 1.25rem;
        font-weight: 500;
      }
    }

    .nav-items {
      display: flex;
      gap: $spacing-unit * 2;
      align-items: center;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: $spacing-unit;
      padding: $spacing-unit $spacing-unit * 2;
      border-radius: $border-radius;
      text-decoration: none;
      color: var(--tui-text-02);
      transition: all 0.2s;

      &:hover {
        background: var(--tui-base-02);
        color: var(--tui-text-01);
      }

      &.active {
        background: var(--tui-primary);
        color: var(--tui-primary-text);
      }

      i {
        font-size: 1.25rem;
      }
    }

    .dropdown-button {
      width: 100%;
      justify-content: space-between;
    }

    .dropdown-item {
      display: flex;
      align-items: center;
      gap: $spacing-unit;
      width: 100%;
      padding: $spacing-unit $spacing-unit * 2;
      border: none;
      background: none;
      text-align: left;
      cursor: pointer;
      color: var(--tui-text-02);
      transition: all 0.2s;

      &:hover {
        background: var(--tui-base-02);
        color: var(--tui-text-01);
      }

      &.active {
        background: var(--tui-primary-hover);
        color: var(--tui-primary-text);
      }
    }

    @media (max-width: 768px) {
      .nav-menu {
        flex-direction: column;
        align-items: stretch;
        padding: $spacing-unit * 2;
      }

      .brand {
        margin-right: 0;
        margin-bottom: $spacing-unit * 2;
        text-align: center;
      }

      .nav-items {
        flex-direction: column;
      }

      .dropdown-button {
        width: 100%;
      }
    }
  `]
})
export class NavMenuComponent {
  appName = 'Domain Driven ERP';

  menuItems: NavMenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'tuiIconLayoutDashboard',
      route: '/dashboard'
    },
    {
      label: 'Localization',
      icon: 'tuiIconWorld',
      children: [
        {
          label: 'Languages',
          route: '/localization/languages',
          icon: 'tuiIconLanguage'
        },
        {
          label: 'Translations',
          route: '/localization/translations',
          icon: 'tuiIconEdit'
        }
      ]
    },
    {
      label: 'Reports',
      icon: 'tuiIconChartBar',
      route: '/reports'
    },
    {
      label: 'Settings',
      icon: 'tuiIconSettings',
      route: '/settings'
    },
    {
      label: 'Help',
      icon: 'tuiIconHelpCircle',
      route: '/help'
    }
  ];
}