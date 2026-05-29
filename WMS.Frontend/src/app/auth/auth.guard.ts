import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(): boolean {
    // Check if the user has a token saved in their browser
    if (localStorage.getItem('jwt_token')) {
      return true; // They are logged in, let them pass!
    }
    
    // If there is no token, redirect them to the login page
    this.router.navigate(['/login']);
    return false;
  }
}