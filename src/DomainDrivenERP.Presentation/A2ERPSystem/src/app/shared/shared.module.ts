import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

// Taiga UI Modules
import { TuiTableModule } from '@taiga-ui/addon-table';
import {
  TuiButtonModule,
  TuiDialogModule,
  TuiLoaderModule,
  TuiAlertModule,
  TuiTextfieldModule,
  TuiErrorModule,
  TuiDataListModule,
  TuiDropdownModule,
  TuiHostedDropdownModule,
  TuiPrimitiveTextfieldModule
} from '@taiga-ui/core';
import {
  TuiInputModule,
  TuiTextAreaModule,
  TuiComboBoxModule,
  TuiSelectModule,
  TuiCheckboxModule,
  TuiTagModule,
  TuiFieldErrorModule,
  TuiFilesModule
} from '@taiga-ui/kit';

// Components
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';

// Services
import { DialogService } from './services/dialog.service';

const TAIGA_MODULES = [
  // Core
  TuiButtonModule,
  TuiDialogModule,
  TuiLoaderModule,
  TuiAlertModule,
  TuiTextfieldModule,
  TuiErrorModule,
  TuiDataListModule,
  TuiDropdownModule,
  TuiHostedDropdownModule,
  TuiPrimitiveTextfieldModule,

  // Kit
  TuiInputModule,
  TuiTextAreaModule,
  TuiComboBoxModule,
  TuiSelectModule,
  TuiCheckboxModule,
  TuiTagModule,
  TuiFieldErrorModule,
  TuiFilesModule,

  // Add-ons
  TuiTableModule
];

const COMPONENTS = [
  ConfirmationDialogComponent
];

const SERVICES = [
  DialogService
];

@NgModule({
  declarations: [...COMPONENTS],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ...TAIGA_MODULES
  ],
  exports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ...TAIGA_MODULES,
    ...COMPONENTS
  ],
  providers: [...SERVICES]
})
export class SharedModule {
  static forRoot() {
    return {
      ngModule: SharedModule,
      providers: [...SERVICES]
    };
  }
}