import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TuiDialogContext } from '@taiga-ui/core';
import { POLYMORPHEUS_CONTEXT } from '@tinkoff/ng-polymorpheus';
import { Language } from '../../models/language.model';

interface LanguageFormData {
  language?: Language;
}

@Component({
  selector: 'app-language-form',
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <div class="form-row">
        <tui-input formControlName="code">
          Language Code
          <input tuiTextfield placeholder="e.g., en" />
        </tui-input>
        <tui-error formControlName="code" [error]="[] | tuiFieldError | async">
          Required field, 2-5 characters
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="name">
          Name
          <input tuiTextfield placeholder="e.g., English" />
        </tui-input>
        <tui-error formControlName="name" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="nativeName">
          Native Name
          <input tuiTextfield placeholder="e.g., English" />
        </tui-input>
        <tui-error formControlName="nativeName" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="flagIcon">
          Flag Icon
          <input tuiTextfield placeholder="e.g., 🇺🇸" />
        </tui-input>
        <tui-error formControlName="flagIcon" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="dateFormat">
          Date Format
          <input tuiTextfield placeholder="e.g., MM/dd/yyyy" />
        </tui-input>
        <tui-error formControlName="dateFormat" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="timeFormat">
          Time Format
          <input tuiTextfield placeholder="e.g., HH:mm" />
        </tui-input>
        <tui-error formControlName="timeFormat" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="numberFormat">
          Number Format
          <input tuiTextfield placeholder="e.g., 1,234.56" />
        </tui-input>
        <tui-error formControlName="numberFormat" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row">
        <tui-input formControlName="currencyFormat">
          Currency Format
          <input tuiTextfield placeholder="e.g., $1,234.56" />
        </tui-input>
        <tui-error formControlName="currencyFormat" [error]="[] | tuiFieldError | async">
          Required field
        </tui-error>
      </div>

      <div class="form-row form-row--inline">
        <tui-checkbox formControlName="isDefault">
          Default Language
        </tui-checkbox>

        <tui-checkbox formControlName="isEnabled">
          Enabled
        </tui-checkbox>

        <tui-checkbox formControlName="isRTL">
          Right to Left (RTL)
        </tui-checkbox>
      </div>

      <div class="form-actions">
        <button
          tuiButton
          type="button"
          appearance="secondary"
          size="m"
          (click)="onCancel()">
          Cancel
        </button>
        <button
          tuiButton
          type="submit"
          appearance="primary"
          size="m"
          [disabled]="!form.valid || form.pristine">
          {{ isEditMode ? 'Update' : 'Create' }}
        </button>
      </div>
    </form>
  `,
  styles: [`
    @import '../../../../shared/styles/base';

    :host {
      display: block;
      padding: $spacing-unit * 4;
    }

    .form-row {
      margin-bottom: $spacing-unit * 4;
    }

    .form-row--inline {
      display: flex;
      gap: $spacing-unit * 4;
      align-items: center;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: $spacing-unit * 2;
      margin-top: $spacing-unit * 6;
      padding-top: $spacing-unit * 4;
      border-top: 1px solid var(--tui-base-03);
    }
  `]
})
export class LanguageFormComponent implements OnInit {
  form!: FormGroup;
  isEditMode: boolean;
  private existingLanguage?: Language;

  constructor(
    @Inject(POLYMORPHEUS_CONTEXT)
    private readonly context: TuiDialogContext<Language, LanguageFormData>,
    private fb: FormBuilder
  ) {
    this.existingLanguage = context.data?.language;
    this.isEditMode = !!this.existingLanguage;
  }

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.form = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(5)]],
      name: ['', [Validators.required]],
      nativeName: ['', [Validators.required]],
      flagIcon: ['', [Validators.required]],
      dateFormat: ['', [Validators.required]],
      timeFormat: ['', [Validators.required]],
      numberFormat: ['', [Validators.required]],
      currencyFormat: ['', [Validators.required]],
      isDefault: [false],
      isEnabled: [true],
      isRTL: [false]
    });

    if (this.existingLanguage) {
      this.form.patchValue(this.existingLanguage);
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      const language: Language = {
        ...(this.isEditMode && this.existingLanguage 
          ? { id: this.existingLanguage.id }
          : { id: crypto.randomUUID() }
        ),
        ...this.form.value
      };
      this.context.completeWith(language);
    }
  }

  onCancel(): void {
    this.context.$implicit.complete();
  }
}