import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './auth/auth';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  isAdmin = false;

  constructor(public authService: AuthService) {}

  ngOnInit(): void {
    this.authService.isLoggedIn.subscribe(loggedIn => {
      if (loggedIn) {
        this.isAdmin = this.authService.isAdmin();
      } else {
        this.isAdmin = false;
      }
    });
  }

  onLogout(): void {
    this.authService.logout();
  }
}
