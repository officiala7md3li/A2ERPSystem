import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  TuiRootModule,
  TuiDialogModule,
  TuiAlertModule,
  TuiButtonModule,
  TuiDataListModule,
  TuiLoaderModule,
  TuiHostedDropdownModule,
} from '@taiga-ui/core';
import {
  TuiInputModule,
  TuiSelectModule,
  TuiTagModule,
  TuiTextAreaModule,
  TuiErrorModule,
} from '@taiga-ui/kit';

const TAIGA_UI_MODULES = [
  TuiRootModule,
  TuiDialogModule,
  TuiAlertModule,
  TuiButtonModule,
  TuiDataListModule,
  TuiLoaderModule,
  TuiHostedDropdownModule,
  TuiInputModule,
  TuiSelectModule,
  TuiTagModule,
  TuiTextAreaModule,
  TuiErrorModule,
];

@NgModule({
  imports: [
    CommonModule,
    ...TAIGA_UI_MODULES
  ],
  exports: [
    CommonModule,
    ...TAIGA_UI_MODULES
  ]
})
export class SharedUiModule { }