import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-attendance',
  standalone: true,
  imports: [CommonModule, MatTableModule, FormsModule],
  templateUrl: './attendance.html',
  styleUrls: ['./attendance.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AttendanceComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  attendances: any[] = [];
  employees: any[] = [];
  serverError: string = '';

  displayedColumns: string[] = ['id', 'employee', 'date', 'checkIn', 'workMode', 'checkOut', 'totalHours', 'reports'];

  exportingExcel = false;
  exportingPdf = false;

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

  ngOnDestroy(): void {}

  trackByEmployeeId(_index: number, emp: any): number {
    return emp.employeeId;
  }

  trackByMonthValue(_index: number, m: any): number {
    return m.value;
  }

  trackByAttendanceId(_index: number, record: any): number {
    return record.attendanceId;
  }

  loadEmployees() {
    this.api.getEmployees().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.employees = data);
  }

  loadData() {
    this.api.getMonthlyAttendance(this.currentMonth, this.currentYear).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
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
    this.serverError = '';
    if (this.authService.isAdmin()) {
      if (!this.newAttendance.empId) {
        this.serverError = 'Please select an employee.';
        return;
      }
      this.newAttendance.checkIn = new Date().toISOString();
      this.newAttendance.attendanceDate = new Date().toISOString();
      this.api.addAttendance(this.newAttendance).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Clocked in successfully!');
          this.loadData();
          this.newAttendance.empId = null;
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Error logging attendance.';
        }
      });
    } else {
      this.api.checkIn(this.newAttendance.workMode).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Clocked in successfully!');
          this.loadData();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Error logging attendance.';
        }
      });
    }
  }

  onCheckOut(record: any) {
    this.serverError = '';
    if (this.authService.isAdmin()) {
      if (confirm(`Are you sure you want to clock out ${record.employee?.firstName}?`)) {
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
        this.api.updateAttendance(record.attendanceId, updatedRecord).pipe(
          takeUntilDestroyed(this.destroyRef)
        ).subscribe({
          next: () => {
            alert('Employee checked out successfully!');
            this.loadData();
          },
          error: (err: any) => {
            this.serverError = err.error?.message || 'Error checking out.';
          }
        });
      }
    } else {
      this.api.checkOut().pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Checked out successfully!');
          this.loadData();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Error checking out.';
        }
      });
    }
  }

  generateTimesheet(record: any) {
    this.serverError = '';
    const empId = record.employee?.employeeId;
    if (!empId) {
      this.serverError = 'Cannot generate report: Employee ID missing.';
      return;
    }
    this.api.downloadTimesheetPdf(empId).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
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
      error: (err: any) => {
        this.serverError = err.error?.message || 'Error generating PDF report.';
      }
    });
  }

  exportExcel() {
    this.exportingExcel = true;
    this.api.exportAttendanceExcel(undefined, this.currentMonth, this.currentYear).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Attendance_${this.currentMonth}_${this.currentYear}.xlsx`);
        this.exportingExcel = false;
      },
      error: () => { this.exportingExcel = false; alert('Failed to export Excel.'); }
    });
  }

  exportPdf() {
    this.exportingPdf = true;
    this.api.exportAttendancePdf(undefined, this.currentMonth, this.currentYear).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Attendance_${this.currentMonth}_${this.currentYear}.pdf`);
        this.exportingPdf = false;
      },
      error: () => { this.exportingPdf = false; alert('Failed to export PDF.'); }
    });
  }
}
