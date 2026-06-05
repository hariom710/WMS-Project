import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

const dateRangeValidator: ValidatorFn = (form: AbstractControl): ValidationErrors | null => {
    const start = form.get('fromDate')?.value;
    const end = form.get('toDate')?.value;
    if (start && end && new Date(end) < new Date(start)) {
      return { dateRange: true };
    }
    return null;
};

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './leaves.html',
  styleUrls: ['./leaves.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LeavesComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  leaveForm: FormGroup;
  leaves: any[] = [];
  pendingTeamLeaves: any[] = [];
  employees: any[] = [];
  activeTab: string = 'my-leaves';
  serverError: string = '';
  exportingExcel = false;
  exportingPdf = false;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.leaveForm = this.fb.group({
      empId: [null],
      leaveType: ['', Validators.required],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      reason: ['', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(500)
      ]]
    }, { validators: dateRangeValidator });
  }

  ngOnInit(): void {
    this.loadLeaves();
    if (this.authService.isAdmin()) {
      this.loadEmployees();
      this.loadPendingLeaves();
    }
  }

  ngOnDestroy(): void {}

  trackByEmployeeId(_index: number, emp: any): number {
    return emp.employeeId;
  }

  trackByLeaveId(_index: number, leave: any): number {
    return leave.leaveId;
  }

  loadLeaves() {
    this.api.getLeaves().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.leaves = data,
      error: (err) => console.error(err)
    });
  }

  loadEmployees() {
    this.api.getEmployees().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.employees = data,
      error: (err) => console.error(err)
    });
  }

  loadPendingLeaves() {
    this.api.getPendingLeaves().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.pendingTeamLeaves = data,
      error: (err) => console.error(err)
    });
  }

  cancelLeave(id: number) {
    if (confirm('Cancel this leave request?')) {
      this.api.cancelLeave(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadLeaves(),
        error: (err) => alert(err.error?.message || 'Error cancelling leave.')
      });
    }
  }

  approveLeave(id: number) {
    if (confirm('Approve this leave request?')) {
      this.api.approveLeave(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => { this.loadLeaves(); this.loadPendingLeaves(); },
        error: (err) => alert(err.error?.message || 'Error approving leave.')
      });
    }
  }

  rejectLeave(id: number) {
    const reason = prompt('Enter rejection reason (optional):');
    this.api.rejectLeave(id, reason || '').pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => { this.loadLeaves(); this.loadPendingLeaves(); },
      error: (err) => alert(err.error?.message || 'Error rejecting leave.')
    });
  }

  onSubmit() {
    this.serverError = '';
    if (this.leaveForm.invalid) {
      this.leaveForm.markAllAsTouched();
      return;
    }

    const formData = { ...this.leaveForm.value };
    if (formData.reason) formData.reason = formData.reason.trim();

    this.api.applyLeave(formData).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        alert('Leave request submitted successfully!');
        this.leaveForm.reset();
        this.loadLeaves();
      },
      error: (err: any) => {
        this.serverError = err.error?.message || 'Error applying leave.';
      }
    });
  }

  get f() { return this.leaveForm.controls; }

  exportExcel() {
    this.exportingExcel = true;
    this.api.exportLeavesExcel().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Leaves_${new Date().toISOString().slice(0,10)}.xlsx`);
        this.exportingExcel = false;
      },
      error: () => { this.exportingExcel = false; alert('Failed to export Excel.'); }
    });
  }

  exportPdf() {
    this.exportingPdf = true;
    this.api.exportLeavesPdf().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Leaves_${new Date().toISOString().slice(0,10)}.pdf`);
        this.exportingPdf = false;
      },
      error: () => { this.exportingPdf = false; alert('Failed to export PDF.'); }
    });
  }
}
