import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'languages',
        loadComponent: () => import('./components/language-list/language-list.component')
          .then(m => m.LanguageListComponent),
        title: 'Languages'
      },
      {
        path: 'translations',
        loadComponent: () => import('./components/translations/translations.component')
          .then(m => m.TranslationsComponent),
        title: 'Translations'
      },
      {
        path: '',
        redirectTo: 'languages',
        pathMatch: 'full'
      }
    ]
  }
];