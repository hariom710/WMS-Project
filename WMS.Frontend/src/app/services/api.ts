import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://hariomwmsapi8501.azurewebsites.net/api';
  constructor(private http: HttpClient) { }

  // ==========================cd
  // EMPLOYEES
  // ==========================
  getEmployees(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Employees`); 
  }
  addEmployee(employee: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Employees`, employee); 
  }
  updateEmployee(id: number, employee: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Employees/${id}`, employee);
  }

  // ==========================
  // DEPARTMENTS
  // ==========================
  getDepartments(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Departments`); 
  }
  addDepartment(department: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Departments`, department); 
  }
  updateDepartment(id: number, department: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Departments/${id}`, department);
  }
  deleteDepartment(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Departments/${id}`);
  }

  // ==========================
  // ROLES
  // ==========================
  getRoles(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Roles`); 
  }

// ==========================
  // ATTENDANCE
  // ==========================
  getAttendances(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Attendance`); 
  }
  getMonthlyAttendance(month: number, year: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Attendance/monthly?month=${month}&year=${year}`);
  }
  addAttendance(attendance: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Attendance`, attendance); 
  }
  updateAttendance(id: number, attendance: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Attendance/${id}`, attendance);
  }
  getTimesheet(empId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Attendance/timesheet/${empId}`);
  }

  // ---> ADD THIS NEW PDF METHOD HERE <---
  downloadTimesheetPdf(empId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Attendance/timesheet/pdf/${empId}`, {
      responseType: 'blob' // Required to handle PDF downloads properly
    });
  }

  // --- Self-Service Methods ---
  getMyAttendance(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Attendance/my-attendance`); 
  }
  checkIn(workMode: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/Attendance/check-in`, `"${workMode}"`, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
  checkOut(): Observable<any> {
    return this.http.put(`${this.baseUrl}/Attendance/check-out`, {});
  }

  // ==========================
  // PROJECTS
  // ==========================
  getProjects(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Projects`); 
  }
  addProject(project: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Projects`, project); 
  }

  // ==========================
  // PROJECT ALLOCATIONS
  // ==========================
  getAllocations(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Allocations`); 
  }
  addAllocation(allocation: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Allocations`, allocation); 
  }
  deleteAllocation(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Allocations/${id}`);
  }

  // ==========================
  // LEAVES
  // ==========================
  getLeaves(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Leaves`); 
  }
  getPendingLeaves(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Leaves/pending`); 
  }
  applyLeave(leave: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Leaves`, leave); 
  }
  cancelLeave(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Leaves/${id}`);
  }
  approveLeave(id: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/Leaves/approve/${id}`, {});
  }
  rejectLeave(id: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/Leaves/reject/${id}`, {});
  }

  // ==========================
  // ANNOUNCEMENTS
  // ==========================
  getAlerts(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Announcements`); 
  }
  addAnnouncement(announcement: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Announcements`, announcement); 
  }
  updateAnnouncement(id: number, announcement: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Announcements/${id}`, announcement);
  }
  deleteAnnouncement(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Announcements/${id}`);
  }

  // ==========================
  // CLIENTS
  // ==========================
  getClients(): Observable<any> { 
    return this.http.get(`${this.baseUrl}/Clients`); 
  }
  addClient(client: any): Observable<any> { 
    return this.http.post(`${this.baseUrl}/Clients`, client); 
  }
  updateClient(id: number, client: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Clients/${id}`, client);
  }
  deleteClient(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Clients/${id}`);
  }
}