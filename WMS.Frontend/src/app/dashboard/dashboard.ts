import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DashboardSummary } from '../models';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);
  private charts: Chart[] = [];

  loading = true;
  exportingPdf = false;
  username = '';
  today = new Date();
  summary: DashboardSummary | null = null;

  constructor(private api: ApiService, private authService: AuthService) {}

  ngOnInit(): void {
    this.username = this.authService.getUsername();
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    this.charts.forEach(c => c.destroy());
  }

  private loadDashboard(): void {
    this.api.getDashboardSummary().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => {
        this.summary = data;
        this.loading = false;
        setTimeout(() => this.buildCharts(), 0);
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private buildCharts(): void {
    this.charts.forEach(c => c.destroy());
    this.charts = [];

    if (!this.summary) return;

    this.buildAttendanceChart();
    this.buildLeaveChart();
    this.buildProjectChart();
    this.buildDepartmentChart();
    this.buildLeaveTrendChart();
  }

  private buildAttendanceChart(): void {
    const trend = this.summary!.attendance.monthlyTrend;
    if (trend.length === 0) return;

    const chart = new Chart('attendanceChart', {
      type: 'line',
      data: {
        labels: trend.map(t => t.month),
        datasets: [{
          label: 'Check-ins',
          data: trend.map(t => t.count),
          borderColor: '#2563EB',
          backgroundColor: 'rgba(37, 99, 235, 0.08)',
          fill: true,
          tension: 0.4,
          borderWidth: 2.5,
          pointBackgroundColor: '#2563EB',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 4,
          pointHoverRadius: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: { beginAtZero: true, grid: { color: '#F1F5F9' }, ticks: { color: '#94A3B8' } },
          x: { grid: { display: false }, ticks: { color: '#94A3B8' } }
        }
      }
    });
    this.charts.push(chart);
  }

  private buildLeaveChart(): void {
    const leaves = this.summary!.leaves;
    const chart = new Chart('leaveChart', {
      type: 'doughnut',
      data: {
        labels: ['Approved', 'Pending', 'Rejected'],
        datasets: [{
          data: [leaves.approvedCount, leaves.pendingCount, leaves.rejectedCount],
          backgroundColor: ['#22C55E', '#F59E0B', '#EF4444'],
          borderWidth: 0,
          hoverOffset: 8
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '65%',
        plugins: { legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true, pointStyle: 'circle' } } }
      }
    });
    this.charts.push(chart);
  }

  private buildProjectChart(): void {
    const dist = this.summary!.projects.statusDistribution;
    if (dist.length === 0) return;

    const colors: { [key: string]: string } = {
      'Active': '#2563EB',
      'Completed': '#22C55E',
      'On Hold': '#F59E0B',
      'Cancelled': '#EF4444'
    };

    const chart = new Chart('projectChart', {
      type: 'bar',
      data: {
        labels: dist.map(d => d.status),
        datasets: [{
          label: 'Projects',
          data: dist.map(d => d.count),
          backgroundColor: dist.map(d => colors[d.status] || '#94A3B8'),
          borderRadius: 8,
          borderSkipped: false,
          barThickness: 40
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: { beginAtZero: true, ticks: { stepSize: 1, color: '#94A3B8' }, grid: { color: '#F1F5F9' } },
          x: { grid: { display: false }, ticks: { color: '#94A3B8' } }
        }
      }
    });
    this.charts.push(chart);
  }

  private buildDepartmentChart(): void {
    const depts = this.summary!.departments.employeeCounts;
    if (depts.length === 0) return;

    const chart = new Chart('departmentChart', {
      type: 'bar',
      data: {
        labels: depts.map(d => d.departmentName),
        datasets: [{
          label: 'Employees',
          data: depts.map(d => d.employeeCount),
          backgroundColor: '#3B82F6',
          borderRadius: 6,
          borderSkipped: false,
          barThickness: 32
        }]
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          x: { beginAtZero: true, grid: { color: '#F1F5F9' }, ticks: { color: '#94A3B8', stepSize: 1 } },
          y: { grid: { display: false }, ticks: { color: '#475569' } }
        }
      }
    });
    this.charts.push(chart);
  }

  private buildLeaveTrendChart(): void {
    const trend = this.summary!.leaves.monthlyTrend;
    if (trend.length === 0) return;

    const chart = new Chart('leaveTrendChart', {
      type: 'bar',
      data: {
        labels: trend.map(t => t.month),
        datasets: [{
          label: 'Leaves',
          data: trend.map(t => t.count),
          backgroundColor: '#F59E0B',
          borderRadius: 6,
          borderSkipped: false,
          barThickness: 32
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: { beginAtZero: true, grid: { color: '#F1F5F9' }, ticks: { color: '#94A3B8', stepSize: 1 } },
          x: { grid: { display: false }, ticks: { color: '#94A3B8' } }
        }
      }
    });
    this.charts.push(chart);
  }

  trackByActivityIndex(_index: number, activity: any): number {
    return activity.auditId;
  }

  getActionBadgeClass(action: string): string {
    if (action.includes('Create') || action.includes('Login')) return 'badge-success';
    if (action.includes('Delete')) return 'badge-danger';
    if (action.includes('Update') || action.includes('Approve')) return 'badge-info';
    if (action.includes('Reject') || action.includes('Failed')) return 'badge-warning';
    return 'badge-primary';
  }

  exportDashboardPdf() {
    this.exportingPdf = true;
    this.api.exportDashboardPdf().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Dashboard_${new Date().toISOString().slice(0,10)}.pdf`);
        this.exportingPdf = false;
      },
      error: () => { this.exportingPdf = false; alert('Failed to export Dashboard PDF.'); }
    });
  }
}
