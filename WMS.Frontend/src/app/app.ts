import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { AuthService } from './auth/auth';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  isAdmin = false;
  sidebarOpen = false;
  currentPageTitle = 'Dashboard';
  username = '';
  userRole = '';

  navItems = [
    { path: '/dashboard', label: 'Dashboard', icon: '📊', section: 'Main' },
    { path: '/departments', label: 'Departments', icon: '🏢', section: 'Organization' },
    { path: '/employees', label: 'Employees', icon: '👥', section: 'Organization' },
    { path: '/attendance', label: 'Attendance', icon: '⏰', section: 'Workforce' },
    { path: '/leaves', label: 'Leaves', icon: '📅', section: 'Workforce' },
    { path: '/projects', label: 'Projects', icon: '📁', section: 'Projects' },
    { path: '/allocations', label: 'Allocations', icon: '🔗', section: 'Projects' },
    { path: '/clients', label: 'Clients', icon: '🤝', section: 'Business' },
    { path: '/announcements', label: 'Notices', icon: '📢', section: 'Communication' },
  ];

  constructor(public authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.isLoggedIn.pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(loggedIn => {
      if (loggedIn) {
        this.isAdmin = this.authService.isAdmin();
        this.username = this.authService.getUsername();
        this.userRole = this.authService.getRole();
      } else {
        this.isAdmin = false;
      }
    });

    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(() => {
      const url = this.router.url;
      const item = this.navItems.find(n => url.startsWith(n.path));
      this.currentPageTitle = item ? item.label : 'Dashboard';
      this.sidebarOpen = false;
    });
  }

  ngOnDestroy(): void {}

  trackByNavPath(_index: number, item: { path: string }): string {
    return item.path;
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar() {
    this.sidebarOpen = false;
  }

  onLogout(): void {
    this.authService.logout();
  }

  getInitials(): string {
    return this.username.substring(0, 2).toUpperCase();
  }
}
