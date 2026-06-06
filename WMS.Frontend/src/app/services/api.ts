import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, retry, throwError, map } from 'rxjs';
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

  constructor(private http: HttpClient) { }

  private handleError(error: any) {
    console.error('API Error:', error);
    return throwError(() => error);
  }

  private extractData<T>(response: ApiResponse<T>): T {
    return response.data;
  }

  // ==========================
  // EMPLOYEES
  // ==========================
  getEmployees(): Observable<Employee[]> {
    return this.http.get<ApiResponse<Employee[]>>(`${this.baseUrl}/Employees`).pipe(
      retry(1),
      map(res => this.extractData<Employee[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Department[]>>(`${this.baseUrl}/Departments`).pipe(
      retry(1),
      map(res => this.extractData<Department[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Role[]>>(`${this.baseUrl}/Roles`).pipe(
      retry(1),
      map(res => this.extractData<Role[]>(res)),
      catchError(this.handleError)
    );
  }

  // ==========================
  // ATTENDANCE
  // ==========================
  getAttendances(): Observable<Attendance[]> {
    return this.http.get<ApiResponse<Attendance[]>>(`${this.baseUrl}/Attendance`).pipe(
      retry(1),
      map(res => this.extractData<Attendance[]>(res)),
      catchError(this.handleError)
    );
  }

  getMonthlyAttendance(month: number, year: number): Observable<Attendance[]> {
    return this.http.get<ApiResponse<Attendance[]>>(`${this.baseUrl}/Attendance/monthly?month=${month}&year=${year}`).pipe(
      retry(1),
      map(res => this.extractData<Attendance[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Attendance[]>>(`${this.baseUrl}/Attendance/my-attendance`).pipe(
      retry(1),
      map(res => this.extractData<Attendance[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Project[]>>(`${this.baseUrl}/Projects`).pipe(
      retry(1),
      map(res => this.extractData<Project[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Allocation[]>>(`${this.baseUrl}/Allocations`).pipe(
      retry(1),
      map(res => this.extractData<Allocation[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Leave[]>>(`${this.baseUrl}/Leaves`).pipe(
      retry(1),
      map(res => this.extractData<Leave[]>(res)),
      catchError(this.handleError)
    );
  }

  getPendingLeaves(): Observable<Leave[]> {
    return this.http.get<ApiResponse<Leave[]>>(`${this.baseUrl}/Leaves/pending`).pipe(
      retry(1),
      map(res => this.extractData<Leave[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Announcement[]>>(`${this.baseUrl}/Announcements`).pipe(
      retry(1),
      map(res => this.extractData<Announcement[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<Client[]>>(`${this.baseUrl}/Clients`).pipe(
      retry(1),
      map(res => this.extractData<Client[]>(res)),
      catchError(this.handleError)
    );
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
    return this.http.get<ApiResponse<DashboardSummary>>(`${this.baseUrl}/Dashboard/summary`).pipe(
      retry(1),
      map(res => this.extractData<DashboardSummary>(res)),
      catchError(this.handleError)
    );
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
