import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, MatTableModule, FormsModule],
  templateUrl: './attendance.html',
  styleUrls: ['./attendance.css']
})
export class AttendanceComponent implements OnInit {
  attendances: any[] = [];
  employees: any[] = [];

  displayedColumns: string[] = ['id', 'employee', 'date', 'checkIn', 'workMode', 'checkOut', 'totalHours', 'reports'];

  newAttendance: any = {
    empId: null,
    workMode: 'Office',
    attendanceDate: new Date().toISOString(),
    checkIn: new Date().toISOString()
  };

  currentMonth: number = new Date().getMonth() + 1;
  currentYear: number = new Date().getFullYear();

  months = [
    { value: 1, name: 'January' }, { value: 2, name: 'February' },
    { value: 3, name: 'March' }, { value: 4, name: 'April' },
    { value: 5, name: 'May' }, { value: 6, name: 'June' },
    { value: 7, name: 'July' }, { value: 8, name: 'August' },
    { value: 9, name: 'September' }, { value: 10, name: 'October' },
    { value: 11, name: 'November' }, { value: 12, name: 'December' }
  ];

  constructor(
    private api: ApiService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.loadEmployees();
    }
    this.loadData();
  }

  loadEmployees() {
    this.api.getEmployees().subscribe(data => this.employees = data);
  }

  loadData() {
    this.api.getMonthlyAttendance(this.currentMonth, this.currentYear).subscribe({
      next: (data) => {
        this.attendances = data.map((record: any) => {
          if (record.checkIn && !record.checkIn.endsWith('Z')) {
            record.checkIn += 'Z';
          }
          if (record.checkOut && !record.checkOut.endsWith('Z')) {
            record.checkOut += 'Z';
          }
          return record;
        });
      },
      error: (err) => console.error('Error fetching attendance', err)
    });
  }

  onFilterChange() {
    this.loadData();
  }

  onSubmit() {
    if (this.authService.isAdmin()) {
      if (!this.newAttendance.empId) {
        alert("Please select an employee!");
        return;
      }
      this.newAttendance.checkIn = new Date().toISOString();
      this.newAttendance.attendanceDate = new Date().toISOString();
      this.api.addAttendance(this.newAttendance).subscribe({
        next: () => {
          alert('Clocked in successfully!');
          this.loadData();
          this.newAttendance.empId = null;
        },
        error: (err: unknown) => console.error('Error logging attendance', err)
      });
    } else {
      this.api.checkIn(this.newAttendance.workMode).subscribe({
        next: () => {
          alert('Clocked in successfully!');
          this.loadData();
        },
        error: (err: unknown) => console.error('Error logging attendance', err)
      });
    }
  }

  onCheckOut(record: any) {
    if (this.authService.isAdmin()) {
      if(confirm(`Are you sure you want to clock out ${record.employee?.firstName}?`)) {
        const updatedRecord = { ...record };
        let checkInStr = record.checkIn;
        if (!checkInStr.endsWith('Z')) {
            checkInStr += 'Z';
        }
        const checkInTime = new Date(checkInStr).getTime();
        const checkOutDate = new Date();
        updatedRecord.checkOut = checkOutDate.toISOString();
        const diffInMilliseconds = checkOutDate.getTime() - checkInTime;
        const totalHrs = diffInMilliseconds / (1000 * 60 * 60);
        updatedRecord.totalHours = Math.round(totalHrs * 100) / 100;
        this.api.updateAttendance(record.attendanceId, updatedRecord).subscribe({
          next: () => {
            alert('Employee checked out successfully!');
            this.loadData();
          },
          error: (err) => console.error('Error checking out', err)
        });
      }
    } else {
      this.api.checkOut().subscribe({
        next: () => {
          alert('Checked out successfully!');
          this.loadData();
        },
        error: (err) => alert(err.error?.message || 'Error checking out')
      });
    }
  }

  generateTimesheet(record: any) {
    const empId = record.employee?.employeeId;
    if (!empId) {
      alert('Cannot generate report: Employee ID missing.');
      return;
    }
    this.api.downloadTimesheetPdf(empId).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        const safeName = record.employee?.firstName + '_' + record.employee?.lastName;
        link.download = `Timesheet_Report_${safeName}.pdf`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => alert('Error generating PDF report.')
    });
  }
}
