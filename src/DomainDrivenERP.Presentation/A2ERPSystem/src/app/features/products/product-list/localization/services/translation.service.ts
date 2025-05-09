import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { ConfigService } from '@core/services/config.service';
import { ErrorHandlerService } from '@core/services/error-handler.service';
import {
  TranslationItem,
  TranslationFormValue,
  TranslationUpdate,
  TranslationImportResult,
  TranslationExportOptions
} from '../models/translation.model';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  constructor(
    private http: HttpClient,
    private config: ConfigService,
    private errorHandler: ErrorHandlerService
  ) {}

  getTranslations(languageCode: string): Observable<TranslationFormValue> {
    return this.http.get<Record<string, string>>(
      `${this.config.apiUrl}/api/localization/translations/${languageCode}`
    ).pipe(
      catchError(error => {
        this.errorHandler.handleHttpError(error);
        return of({});
      })
    );
  }

  getTranslationsByModule(languageCode: string, module: string): Observable<TranslationItem[]> {
    const params = new HttpParams().set('module', module);
    
    return this.http.get<Record<string, string>>(
      `${this.config.apiUrl}/api/localization/translations/${languageCode}`,
      { params }
    ).pipe(
      map(translations => this.mapTranslationsToItems(translations)),
      catchError(error => {
        this.errorHandler.handleHttpError(error);
        return of([]);
      })
    );
  }

  updateTranslation(update: TranslationUpdate): Observable<boolean> {
    return this.http.put<void>(
      `${this.config.apiUrl}/api/localization/translations/${update.languageCode}`,
      update
    ).pipe(
      map(() => true),
      catchError(error => {
        this.errorHandler.handleHttpError(error);
        return of(false);
      })
    );
  }

  importTranslations(languageCode: string, file: File): Observable<TranslationImportResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<TranslationImportResult>(
      `${this.config.apiUrl}/api/localization/translations/${languageCode}/import`,
      formData
    ).pipe(
      catchError(error => {
        this.errorHandler.handleHttpError(error);
        return of({
          totalCount: 0,
          updatedCount: 0,
          addedCount: 0,
          errors: [error.message]
        });
      })
    );
  }

  exportTranslations(
    languageCode: string,
    options: TranslationExportOptions = {}
  ): Observable<Blob> {
    const params = new HttpParams()
      .set('includeMetadata', options.includeMetadata?.toString() ?? 'false')
      .set('format', options.format ?? 'json')
      .set('prettyPrint', options.prettyPrint?.toString() ?? 'true');

    return this.http.get(
      `${this.config.apiUrl}/api/localization/translations/${languageCode}/export`,
      {
        params,
        responseType: 'blob'
      }
    ).pipe(
      catchError(error => {
        this.errorHandler.handleHttpError(error);
        throw error;
      })
    );
  }

  private mapTranslationsToItems(translations: Record<string, string>): TranslationItem[] {
    return Object.entries(translations).map(([key, value]) => {
      const parts = key.split('.');
      const group = parts.length > 1 ? parts[0] : undefined;
      const module = parts.length > 2 ? parts[1] : undefined;

      return {
        key,
        value,
        group,
        module,
        isModified: false
      };
    });
  }

  downloadTranslationFile(blob: Blob, languageCode: string): void {
    const date = new Date().toISOString().split('T')[0];
    const fileName = `translations_${languageCode}_${date}.json`;
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }
}