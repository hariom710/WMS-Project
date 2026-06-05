import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap, map } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.apiUrl}/Auth`;

  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private usernameSubject = new BehaviorSubject<string>(localStorage.getItem('username') || 'User');
  private roleSubject = new BehaviorSubject<string>(localStorage.getItem('role') || '');
  private roleIdSubject = new BehaviorSubject<string>(localStorage.getItem('role_id') || '');

  constructor(private http: HttpClient, private router: Router) { }

  get isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  get username$(): Observable<string> {
    return this.usernameSubject.asObservable();
  }

  get role$(): Observable<string> {
    return this.roleSubject.asObservable();
  }

  get isAdmin$(): Observable<boolean> {
    return this.roleIdSubject.pipe(map(id => id === '1'));
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('jwt_token');
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials).pipe(
      tap((res: any) => {
        localStorage.setItem('jwt_token', res.token);
        localStorage.setItem('role_id', res.roleId);
        localStorage.setItem('role', res.role);
        localStorage.setItem('username', res.username);
        this.loggedIn.next(true);
        this.usernameSubject.next(res.username);
        this.roleSubject.next(res.role);
        this.roleIdSubject.next(res.roleId);
      })
    );
  }

  logout() {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('role_id');
    localStorage.removeItem('role');
    localStorage.removeItem('username');
    this.loggedIn.next(false);
    this.usernameSubject.next('User');
    this.roleSubject.next('');
    this.roleIdSubject.next('');
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('jwt_token');
  }

  getRole(): string {
    return this.roleSubject.value || localStorage.getItem('role') || '';
  }

  getUsername(): string {
    return this.usernameSubject.value || localStorage.getItem('username') || 'User';
  }

  isAdmin(): boolean {
    const role = this.getRole();
    const roleId = this.roleIdSubject.value || localStorage.getItem('role_id');
    return role === 'Admin' || roleId === '1';
  }

  changePassword(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/change-password`, data);
  }
}
