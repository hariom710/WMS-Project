import { Component, OnInit } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../services/api';
import { CommonModule } from '@angular/common';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class DashboardComponent implements OnInit {
  
  // 1. Summary Cards Data
  totalEmployees = 0;
  presentToday = 0;
  activeProjects = 0;
  pendingLeaves = 0;

  // 5. Alerts Data
  alerts: any[] = [];

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    // Fetch KPI Data
    this.api.getEmployees().subscribe(data => this.totalEmployees = data.length);
    
    this.api.getProjects().subscribe(data => {
      this.activeProjects = data.filter((p: any) => p.status === 'Active').length;
      this.buildProjectChart(data); // 4. Project Counts Chart
    });

    this.api.getLeaves().subscribe(data => {
      this.pendingLeaves = data.filter((l: any) => l.status === 'Pending').length;
      this.buildLeaveChart(data); // 3. Leave Statistics Chart
    });

    // --- UPDATED: Mapping the SQL 'createdOn' to the HTML 'date' ---
    this.api.getAlerts().subscribe(data => {
      this.alerts = data.map((a: any) => ({
        title: a.title,
        message: a.message,
        date: a.createdOn 
      }));
    });

    this.api.getAttendances().subscribe(data => {
      const today = new Date().toDateString();
      this.presentToday = data.filter((a: any) => new Date(a.attendanceDate).toDateString() === today).length;
      this.buildAttendanceChart(data); // 2. Attendance Chart
    });
  }

  // --- CHART BUILDERS ---

  buildAttendanceChart(attendances: any[]) {
    new Chart('attendanceChart', {
      type: 'line',
      data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'], // Placeholder for the week
        datasets: [{
          label: 'Daily Check-ins',
          data: [12, 15, 14, 16, this.presentToday], // Injecting today's real data at the end
          borderColor: '#3f51b5',
          backgroundColor: 'rgba(63, 81, 181, 0.2)',
          fill: true,
          tension: 0.3
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  }

  buildLeaveChart(leaves: any[]) {
    const typeCounts: { [key: string]: number } = { 'Sick': 0, 'Casual': 0, 'Earned': 0 };
    leaves.forEach(l => { if (typeCounts[l.leaveType] !== undefined) typeCounts[l.leaveType]++; });

    new Chart('leaveChart', {
      type: 'doughnut',
      data: {
        labels: Object.keys(typeCounts),
        datasets: [{
          data: Object.values(typeCounts),
          backgroundColor: ['#e91e63', '#ff9800', '#4caf50']
        }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  }

  buildProjectChart(projects: any[]) {
    const statusCounts: { [key: string]: number } = { 'Active': 0, 'Completed': 0 };
    projects.forEach(p => { if (statusCounts[p.status] !== undefined) statusCounts[p.status]++; });

    new Chart('projectChart', {
      type: 'bar',
      data: {
        labels: Object.keys(statusCounts),
        datasets: [{
          label: 'Project Counts',
          data: Object.values(statusCounts),
          backgroundColor: ['#2196f3', '#9e9e9e'],
          borderRadius: 4
        }]
      },
      options: { responsive: true, maintainAspectRatio: false, scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } } }
    });
  }
}