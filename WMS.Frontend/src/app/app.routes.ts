import { Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'employees',
    loadComponent: () => import('./employees/employees').then(m => m.EmployeesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'attendance',
    loadComponent: () => import('./attendance/attendance').then(m => m.AttendanceComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'departments',
    loadComponent: () => import('./departments/departments').then(m => m.DepartmentsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'projects',
    loadComponent: () => import('./projects/projects').then(m => m.ProjectsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'leaves',
    loadComponent: () => import('./leaves/leaves').then(m => m.LeavesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'announcements',
    loadComponent: () => import('./announcements/announcements').then(m => m.AnnouncementsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'allocations',
    loadComponent: () => import('./allocations/allocations').then(m => m.AllocationsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'change-password',
    loadComponent: () => import('./auth/change-password/change-password').then(m => m.ChangePasswordComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'clients',
    loadComponent: () => import('./clients/clients').then(m => m.ClientsComponent),
    canActivate: [AuthGuard]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];
