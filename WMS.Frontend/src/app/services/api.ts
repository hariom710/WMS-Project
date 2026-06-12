import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, retry, throwError, map, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';
import { Employee, Department, Role, Attendance, Leave, Project, Allocation, Client, Announcement, DashboardSummary } from '../models';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  paginationInfo?: any;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;
  private departmentsCache$: Observable<Department[]> | null = null;
  private rolesCache$: Observable<Role[]> | null = null;
  private dashboardCache$: Observable<DashboardSummary> | null = null;

  constructor(private http: HttpClient) { }

  invalidateDepartmentCache() { this.departmentsCache$ = null; }
  invalidateRoleCache() { this.rolesCache$ = null; }
  invalidateDashboardCache() { this.dashboardCache$ = null; }

  private handleError(error: any) {
    console.error('API Error:', error);
    return throwError(() => error);
  }

  private extractData<T>(response: any): T {
    if (response && response.data !== undefined) {
      return response.data;
    }
    return response as T;
  }

  private timedGet<T>(url: string, label: string): Observable<T> {
    const start = performance.now();
    return this.http.get<ApiResponse<T>>(url).pipe(
      retry(1),
      map(res => {
        console.log(`[PERF] ${label}: ${Math.round(performance.now() - start)}ms`);
        return this.extractData<T>(res);
      }),
      catchError(this.handleError)
    );
  }

  // ==========================
  // EMPLOYEES
  // ==========================
  getEmployees(): Observable<Employee[]> {
    return this.timedGet<Employee[]>(`${this.baseUrl}/Employees`, 'Employees API');
  }

  addEmployee(employee: Partial<Employee>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Employees`, employee).pipe(
      catchError(this.handleError)
    );
  }

  updateEmployee(id: number, employee: Partial<Employee>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Employees/${id}`, employee).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // DEPARTMENTS
  // ==========================
  getDepartments(): Observable<Department[]> {
    if (!this.departmentsCache$) {
      this.departmentsCache$ = this.timedGet<Department[]>(`${this.baseUrl}/Departments`, 'Departments API').pipe(
        shareReplay(1)
      );
    }
    return this.departmentsCache$;
  }

  addDepartment(department: Partial<Department>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Departments`, department).pipe(
      catchError(this.handleError)
    );
  }

  updateDepartment(id: number, department: Partial<Department>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Departments/${id}`, department).pipe(
      catchError(this.handleError)
    );
  }

  deleteDepartment(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Departments/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // ROLES
  // ==========================
  getRoles(): Observable<Role[]> {
    if (!this.rolesCache$) {
      this.rolesCache$ = this.timedGet<Role[]>(`${this.baseUrl}/Roles`, 'Roles API').pipe(
        shareReplay(1)
      );
    }
    return this.rolesCache$;
  }

  // ==========================
  // ATTENDANCE
  // ==========================
  getAttendances(): Observable<Attendance[]> {
    return this.timedGet<Attendance[]>(`${this.baseUrl}/Attendance`, 'Attendances API');
  }

  getMonthlyAttendance(month: number, year: number): Observable<Attendance[]> {
    return this.timedGet<Attendance[]>(`${this.baseUrl}/Attendance/monthly?month=${month}&year=${year}`, 'Monthly Attendance API');
  }

  addAttendance(attendance: Partial<Attendance>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Attendance`, attendance).pipe(
      catchError(this.handleError)
    );
  }

  updateAttendance(id: number, attendance: Partial<Attendance>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Attendance/${id}`, attendance).pipe(
      catchError(this.handleError)
    );
  }

  getTimesheet(empId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Attendance/timesheet/${empId}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  downloadTimesheetPdf(empId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Attendance/timesheet/pdf/${empId}`, {
      responseType: 'blob'
    }).pipe(
      catchError(this.handleError)
    );
  }

  // --- Self-Service Methods ---
  getMyAttendance(): Observable<Attendance[]> {
    return this.timedGet<Attendance[]>(`${this.baseUrl}/Attendance/my-attendance`, 'My Attendance API');
  }

  checkIn(workMode: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/Attendance/check-in`, `"${workMode}"`, {
      headers: { 'Content-Type': 'application/json' }
    }).pipe(
      catchError(this.handleError)
    );
  }

  checkOut(): Observable<any> {
    return this.http.put(`${this.baseUrl}/Attendance/check-out`, {}).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // PROJECTS
  // ==========================
  getProjects(): Observable<Project[]> {
    return this.timedGet<Project[]>(`${this.baseUrl}/Projects`, 'Projects API');
  }

  addProject(project: Partial<Project>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Projects`, project).pipe(
      catchError(this.handleError)
    );
  }

  updateProject(id: number, project: Partial<Project>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Projects/${id}`, project).pipe(
      catchError(this.handleError)
    );
  }

  deleteProject(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Projects/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // PROJECT ALLOCATIONS
  // ==========================
  getAllocations(): Observable<Allocation[]> {
    return this.timedGet<Allocation[]>(`${this.baseUrl}/Allocations`, 'Allocations API');
  }

  addAllocation(allocation: Partial<Allocation>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Allocations`, allocation).pipe(
      catchError(this.handleError)
    );
  }

  deleteAllocation(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Allocations/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // LEAVES
  // ==========================
  getLeaves(): Observable<Leave[]> {
    return this.timedGet<Leave[]>(`${this.baseUrl}/Leaves`, 'Leaves API');
  }

  getPendingLeaves(): Observable<Leave[]> {
    return this.timedGet<Leave[]>(`${this.baseUrl}/Leaves/pending`, 'Pending Leaves API');
  }

  applyLeave(leave: Partial<Leave>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Leaves`, leave).pipe(
      catchError(this.handleError)
    );
  }

  cancelLeave(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Leaves/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  approveLeave(id: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/Leaves/approve/${id}`, {}).pipe(
      catchError(this.handleError)
    );
  }

  rejectLeave(id: number, reason?: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/Leaves/reject/${id}`, { reason }).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // ANNOUNCEMENTS
  // ==========================
  getAlerts(): Observable<Announcement[]> {
    return this.timedGet<Announcement[]>(`${this.baseUrl}/Announcements`, 'Announcements API');
  }

  addAnnouncement(announcement: Partial<Announcement>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Announcements`, announcement).pipe(
      catchError(this.handleError)
    );
  }

  updateAnnouncement(id: number, announcement: Partial<Announcement>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Announcements/${id}`, announcement).pipe(
      catchError(this.handleError)
    );
  }

  deleteAnnouncement(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Announcements/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // CLIENTS
  // ==========================
  getClients(): Observable<Client[]> {
    return this.timedGet<Client[]>(`${this.baseUrl}/Clients`, 'Clients API');
  }

  addClient(client: Partial<Client>): Observable<any> {
    return this.http.post(`${this.baseUrl}/Clients`, client).pipe(
      catchError(this.handleError)
    );
  }

  updateClient(id: number, client: Partial<Client>): Observable<any> {
    return this.http.put(`${this.baseUrl}/Clients/${id}`, client).pipe(
      catchError(this.handleError)
    );
  }

  deleteClient(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Clients/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // ==========================
  // DASHBOARD
  // ==========================
  getDashboardSummary(): Observable<DashboardSummary> {
    if (!this.dashboardCache$) {
      this.dashboardCache$ = this.timedGet<DashboardSummary>(`${this.baseUrl}/Dashboard/summary`, 'Dashboard API').pipe(
        shareReplay(1)
      );
    }
    return this.dashboardCache$;
  }

  // ==========================
  // REPORTS / EXPORTS
  // ==========================
  private buildParams(params: Record<string, any>): string {
    const parts: string[] = [];
    for (const [key, value] of Object.entries(params)) {
      if (value !== null && value !== undefined && value !== '') {
        parts.push(`${key}=${encodeURIComponent(value)}`);
      }
    }
    return parts.length ? '?' + parts.join('&') : '';
  }

  downloadBlob(blob: Blob, filename: string) {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  exportEmployeesExcel(search?: string, status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/employees/excel${this.buildParams({ search, status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportEmployeesPdf(search?: string, status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/employees/pdf${this.buildParams({ search, status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportAttendanceExcel(empId?: number, month?: number, year?: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/attendance/excel${this.buildParams({ empId, month, year })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportAttendancePdf(empId?: number, month?: number, year?: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/attendance/pdf${this.buildParams({ empId, month, year })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportLeavesExcel(status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/leaves/excel${this.buildParams({ status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportLeavesPdf(status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/leaves/pdf${this.buildParams({ status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportProjectsExcel(status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/projects/excel${this.buildParams({ status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportProjectsPdf(status?: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/projects/pdf${this.buildParams({ status })}`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportClientsExcel(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/clients/excel`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

  exportDashboardPdf(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Reports/dashboard/pdf`, {
      responseType: 'blob'
    }).pipe(catchError(this.handleError));
  }

}
