import { NgModule } from '@angular/core';
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

const TAIGA_UI_CORE = [
  TuiRootModule,
  TuiDialogModule,
  TuiAlertModule,
  TuiButtonModule,
  TuiDataListModule,
  TuiLoaderModule,
  TuiHostedDropdownModule,
];

const TAIGA_UI_KIT = [
  TuiInputModule,
  TuiSelectModule,
  TuiTagModule,
  TuiTextAreaModule,
  TuiErrorModule,
];

@NgModule({
  imports: [...TAIGA_UI_CORE, ...TAIGA_UI_KIT],
  exports: [...TAIGA_UI_CORE, ...TAIGA_UI_KIT],
})
export class TaigaUiModule { }