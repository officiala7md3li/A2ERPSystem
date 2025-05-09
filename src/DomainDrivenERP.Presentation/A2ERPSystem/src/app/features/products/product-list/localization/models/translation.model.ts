export interface TranslationItem {
  key: string;
  value: string;
  group?: string;
  module?: string;
  isModified?: boolean;
}

export interface TranslationFormValue {
  [key: string]: string;
}

export interface TranslationUpdate {
  languageCode: string;
  key: string;
  value: string;
  group?: string;
  module?: string;
}

export interface TranslationImportResult {
  totalCount: number;
  updatedCount: number;
  addedCount: number;
  errors?: string[];
}

export interface TranslationExportOptions {
  includeMetadata?: boolean;
  format?: 'json' | 'csv';
  prettyPrint?: boolean;
}