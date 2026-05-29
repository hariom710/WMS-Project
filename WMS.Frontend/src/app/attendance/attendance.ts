import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api';
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

  newAttendance = {
    empId: null,
    workMode: 'Office',
    attendanceDate: new Date().toISOString(),
    checkIn: new Date().toISOString()
  };

  // --- NEW: Monthly Filter State ---
  currentMonth: number = new Date().getMonth() + 1; // 1-12
  currentYear: number = new Date().getFullYear();
  
  months = [
    { value: 1, name: 'January' }, { value: 2, name: 'February' },
    { value: 3, name: 'March' }, { value: 4, name: 'April' },
    { value: 5, name: 'May' }, { value: 6, name: 'June' },
    { value: 7, name: 'July' }, { value: 8, name: 'August' },
    { value: 9, name: 'September' }, { value: 10, name: 'October' },
    { value: 11, name: 'November' }, { value: 12, name: 'December' }
  ];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadEmployees();
    this.loadData();
  }

  loadEmployees() {
    this.api.getEmployees().subscribe(data => this.employees = data);
  }

  // --- UPDATED: Fetch data and force timezone conversion ---
  loadData() {
    this.api.getMonthlyAttendance(this.currentMonth, this.currentYear).subscribe({
      next: (data) => {
        
        // THE FIX: Append 'Z' to tell Angular these are UTC times from Azure
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

  // --- NEW: Triggered when dropdowns change ---
  onFilterChange() {
    this.loadData();
  }

  onSubmit() {
    if (!this.newAttendance.empId) {
      alert("Please select an employee!");
      return;
    }

    this.newAttendance.checkIn = new Date().toISOString();

    this.api.addAttendance(this.newAttendance).subscribe({
      next: () => {
        alert('Clocked in successfully!');
        this.loadData(); 
        this.newAttendance.empId = null; 
      },
      error: (err: unknown) => console.error('Error logging attendance', err)
    });
  }

  onCheckOut(record: any) {
    if(confirm(`Are you sure you want to clock out ${record.employee?.firstName}?`)) {
      
      const updatedRecord = { ...record };
      
      // THE FIX: If the server time doesn't specify a timezone, force it to UTC by appending 'Z'
      let checkInStr = record.checkIn;
      if (!checkInStr.endsWith('Z')) {
          checkInStr += 'Z'; 
      }

      // Now both times are safely converted to accurate millisecond timestamps
      const checkInTime = new Date(checkInStr).getTime();
      const checkOutDate = new Date(); // Right now
      
      updatedRecord.checkOut = checkOutDate.toISOString();

      // Calculate the difference
      const diffInMilliseconds = checkOutDate.getTime() - checkInTime;
      const totalHrs = diffInMilliseconds / (1000 * 60 * 60); 
      
      // Round to 2 decimal places (e.g. 0.01 hours if you log out 30 seconds later)
      updatedRecord.totalHours = Math.round(totalHrs * 100) / 100; 

      this.api.updateAttendance(record.attendanceId, updatedRecord).subscribe({
        next: () => {
          alert('Employee checked out successfully!');
          this.loadData();
        },
        error: (err) => console.error('Error checking out', err)
      });
    }
  }

  // --- REPLACED CSV LOGIC WITH SECURE PDF DOWNLOAD ---
  generateTimesheet(record: any) {
    const empId = record.employee?.employeeId;
    if (!empId) {
      alert('Cannot generate report: Employee ID missing.');
      return;
    }

    // Call the new PDF endpoint from api.ts
    this.api.downloadTimesheetPdf(empId).subscribe({
      next: (blob: Blob) => {
        // Create a hidden link to trigger the browser's download manager
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        
        // Name the downloaded file dynamically based on the employee
        const safeName = record.employee?.firstName + '_' + record.employee?.lastName;
        link.download = `Timesheet_Report_${safeName}.pdf`;
        
        document.body.appendChild(link);
        link.click();
        
        // Clean up memory
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => alert('Error generating PDF report. Make sure your C# server is running and QuestPDF is installed!')
    });
  }
}