import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { 
  TuiButtonModule,
  TuiDialogModule,
  TuiLoaderModule,
  TuiDataListModule,
  TuiHostedDropdownModule
} from '@taiga-ui/core';
import {
  TuiInputModule,
  TuiTagModule,
  TuiSelectModule,
  TuiTextAreaModule
} from '@taiga-ui/kit';

import { routes } from './localization-routing';
import { LocalizationComponent } from './localization.component';
import { LocalizationService } from './services/localization.service';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
    HttpClientModule,
    
    // Taiga UI Modules
    TuiButtonModule,
    TuiDialogModule,
    TuiLoaderModule,
    TuiDataListModule,
    TuiHostedDropdownModule,
    TuiInputModule,
    TuiTagModule,
    TuiSelectModule,
    TuiTextAreaModule
  ],
  declarations: [
    LocalizationComponent
  ],
  providers: [
    LocalizationService
  ]
})
export class LocalizationModule {}