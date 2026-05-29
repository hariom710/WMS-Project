import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { AuthGuard } from './auth/auth.guard'; // Add this import!

// Import your other components...
import { DashboardComponent } from './dashboard/dashboard';
import { EmployeesComponent } from './employees/employees';
import { AttendanceComponent } from './attendance/attendance';
import { DepartmentsComponent } from './departments/departments';
import { ProjectsComponent } from './projects/projects';
import { LeavesComponent } from './leaves/leaves';
import { AnnouncementsComponent } from './announcements/announcements';
import { ChangePasswordComponent } from './auth/change-password/change-password';
import { AllocationsComponent } from './allocations/allocations';
import { ClientsComponent } from './clients/clients';

export const routes: Routes = [
  { path: 'login', component: LoginComponent }, // Unprotected
  
  // ---> ADD canActivate TO PROTECT THESE ROUTES <---
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'employees', component: EmployeesComponent, canActivate: [AuthGuard] },
  { path: 'attendance', component: AttendanceComponent, canActivate: [AuthGuard] },
  { path: 'departments', component: DepartmentsComponent, canActivate: [AuthGuard] },
  { path: 'projects', component: ProjectsComponent, canActivate: [AuthGuard] },
  { path: 'leaves', component: LeavesComponent, canActivate: [AuthGuard] },
  { path: 'announcements', component: AnnouncementsComponent, canActivate: [AuthGuard] },
  { path: 'allocations', component: AllocationsComponent, canActivate: [AuthGuard] },
  { path: 'change-password', component: ChangePasswordComponent, canActivate: [AuthGuard] },
  { path: 'clients', component: ClientsComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];