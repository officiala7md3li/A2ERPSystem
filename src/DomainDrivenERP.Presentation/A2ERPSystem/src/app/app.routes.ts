import { Routes } from '@angular/router';
import { ProductListComponent } from './features/products/product-list/product-list.component';
import { DashboardComponent } from './features/products/product-list/dashboard/dashboard.component';
import { LocalizationComponent } from './features/products/product-list/localization/localization.component';
import { HelpComponent } from './features/products/product-list/help/help.component';
import { ReportsComponent } from './features/products/product-list/reports/reports.component';
import { SettingsComponent } from './features/products/product-list/settings/settings.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'localization', component: LocalizationComponent, loadChildren: () => import('./features/products/product-list/localization/localization-routing').then(m => m.routes) },
  { path: 'help', component: HelpComponent },
  { path: 'reports', component: ReportsComponent },
  { path: 'settings', component: SettingsComponent },
  { path: '**', redirectTo: 'dashboard' }
];
