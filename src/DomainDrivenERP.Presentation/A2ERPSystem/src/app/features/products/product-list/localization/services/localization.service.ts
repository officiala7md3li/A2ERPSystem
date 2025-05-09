import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Language } from '../models/language.model';

@Injectable({
  providedIn: 'root'
})
export class LocalizationService {
  private readonly apiUrl = 'api/localization';

  constructor(private http: HttpClient) {}

  // Get all available languages
  getLanguages(): Observable<Language[]> {
    return this.http.get<Language[]>(`${this.apiUrl}/languages`);
  }

  // Get translations for a specific language
  getTranslations(languageCode: string): Observable<{ [key: string]: string }> {
    return this.http.get<{ [key: string]: string }>(`${this.apiUrl}/translations/${languageCode}`);
  }

  // Translate a specific key
  translate(key: string, languageCode: string, defaultValue: string = ''): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/translate`, {
      params: {
        key,
        languageCode,
        defaultValue
      }
    });
  }

  // Create a new language (Admin only)
  createLanguage(language: Language): Observable<Language> {
    return this.http.post<Language>(`${this.apiUrl}/languages`, language);
  }

  // Upload translations file
  uploadTranslations(languageCode: string, file: File): Observable<void> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<void>(
      `${this.apiUrl}/languages/${languageCode}/translations`,
      formData
    );
  }

  // Import translations from JSON content
  importTranslations(languageCode: string, jsonContent: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/import`, jsonContent, {
      params: { languageCode },
      headers: { 'Content-Type': 'application/json' }
    });
  }

  // Export translations to JSON
  exportTranslations(languageCode: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/export/${languageCode}`, {});
  }
}