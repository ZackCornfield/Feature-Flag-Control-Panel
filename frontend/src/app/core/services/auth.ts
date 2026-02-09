import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { StorageService } from './storage';
import { Router } from '@angular/router';
import { tap } from 'rxjs/internal/operators/tap';

export interface UserDto {
  id: string;
  email: string;
  createdAt: Date;
  token?: string;
}

export interface UserRequestDto {
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private router: Router,
    private http: HttpClient,
    private storage: StorageService,
  ) {
    this.loadUserFromStorage();
  }

  private readonly API_URL = 'http://localhost:5248/api/auth/';
  currentUser = signal<UserDto | null>(null);
  isAuthenticated = signal<boolean>(false);

  login(credentials: UserRequestDto) {
    return this.http
      .post<UserDto>(this.API_URL + 'login', credentials)
      .pipe(tap((user) => this.handleAuthSuccess(user)));
  }

  register(credentials: UserRequestDto) {
    return this.http
      .post<UserDto>(this.API_URL + 'register', credentials)
      .pipe(tap((user) => this.handleAuthSuccess(user)));
  }

  logout() {
    this.storage.clear();
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['control-panel/login']);
  }

  getToken(): string | null {
    return this.storage.getToken();
  }

  handleAuthSuccess(user: UserDto) {
    this.storage.saveUser(user);
    this.storage.saveToken(user.token!);
    this.currentUser.set(user);
    this.isAuthenticated.set(true);
  }

  private loadUserFromStorage(): void {
    const token = this.storage.getToken();
    const user = this.storage.getUser();

    if (token && user) {
      this.currentUser.set(user);
      this.isAuthenticated.set(true);
    }
  }

  // return TRUE when token is missing or invalid
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) {
      return true; // No token = expired
    }

    const parts = token.split('.');
    if (parts.length !== 3) {
      console.error('Invalid token format');
      return true; // Malformed token = expired
    }

    try {
      const payload = JSON.parse(atob(parts[1]));
      const expiry = payload.exp * 1000;
      return Date.now() > expiry;
    } catch (error) {
      console.error('Error decoding token:', error);
      return true; // Invalid token = expired
    }
  }
}
