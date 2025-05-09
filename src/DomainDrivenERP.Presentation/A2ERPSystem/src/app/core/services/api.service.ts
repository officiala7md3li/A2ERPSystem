import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Performs a GET request to the API
   * @param path The API endpoint path
   * @param params Optional query parameters
   * @returns Observable of the response
   */
  get<T>(path: string, params: any = {}): Observable<T> {
    const httpParams = this.buildParams(params);
    return this.http.get<T>(`${this.apiUrl}/${path}`, { params: httpParams });
  }

  /**
   * Performs a POST request to the API
   * @param path The API endpoint path
   * @param body The request body
   * @returns Observable of the response
   */
  post<T>(path: string, body: any = {}): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${path}`, body);
  }

  /**
   * Performs a PUT request to the API
   * @param path The API endpoint path
   * @param body The request body
   * @returns Observable of the response
   */
  put<T>(path: string, body: any = {}): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${path}`, body);
  }

  /**
   * Performs a DELETE request to the API
   * @param path The API endpoint path
   * @param params Optional query parameters
   * @returns Observable of the response
   */
  delete<T>(path: string, params: any = {}): Observable<T> {
    const httpParams = this.buildParams(params);
    return this.http.delete<T>(`${this.apiUrl}/${path}`, { params: httpParams });
  }

  /**
   * Builds HttpParams from an object
   * @param params Object containing parameters
   * @returns HttpParams
   */
  private buildParams(params: any): HttpParams {
    let httpParams = new HttpParams();
    
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.set(key, params[key]);
        }
      });
    }
    
    return httpParams;
  }
}