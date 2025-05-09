import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiService } from './api.service';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
  isSuccess: boolean;
  errors: string[];
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private tokenKey = 'auth_token';

  constructor(private apiService: ApiService) {
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.apiService.post<LoginResponse>('v1/auth/login', credentials).pipe(
      tap(response => {
        if (response.isSuccess && response.token) {
          localStorage.setItem(this.tokenKey, response.token);
          this.currentUserSubject.next(response.user);
        }
      })
    );
  }

  register(userData: RegisterRequest): Observable<any> {
    return this.apiService.post('v1/auth/register', userData);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private loadUserFromStorage(): void {
    const token = this.getToken();
    if (token) {
      try {
        // For a real app, you would decode the JWT token or make an API call
        // to get the current user information
        // For now, we'll just set a placeholder user
        const user: User = {
          id: 'placeholder',
          email: 'user@example.com',
          firstName: 'John',
          lastName: 'Doe',
          roles: ['User']
        };
        this.currentUserSubject.next(user);
      } catch (error) {
        console.error('Error loading user from token', error);
        this.logout();
      }
    }
  }
}