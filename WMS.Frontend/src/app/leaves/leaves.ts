import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';

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
  styleUrls: ['./leaves.css']
})
export class LeavesComponent implements OnInit {
  leaveForm: FormGroup;
  leaves: any[] = [];
  pendingTeamLeaves: any[] = [];
  employees: any[] = [];
  activeTab: string = 'my-leaves';

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
      reason: ['', [Validators.required, Validators.maxLength(500)]]
    }, { validators: dateRangeValidator });
  }

  ngOnInit(): void {
    this.loadLeaves();
    if (this.authService.isAdmin()) {
      this.loadEmployees();
      this.loadPendingLeaves();
    }
  }

  loadLeaves() {
    this.api.getLeaves().subscribe({
      next: (data) => this.leaves = data,
      error: (err) => console.error(err)
    });
  }

  loadEmployees() {
    this.api.getEmployees().subscribe({
      next: (data) => this.employees = data,
      error: (err) => console.error(err)
    });
  }

  loadPendingLeaves() {
    this.api.getPendingLeaves().subscribe({
      next: (data) => this.pendingTeamLeaves = data,
      error: (err) => console.error(err)
    });
  }

  cancelLeave(id: number) {
    if (confirm('Cancel this leave request?')) {
      this.api.cancelLeave(id).subscribe({
        next: () => this.loadLeaves(),
        error: (err) => alert(err.error?.message || 'Error cancelling leave.')
      });
    }
  }

  approveLeave(id: number) {
    if (confirm('Approve this leave request?')) {
      this.api.approveLeave(id).subscribe({
        next: () => { this.loadLeaves(); this.loadPendingLeaves(); },
        error: (err) => alert(err.error?.message || 'Error approving leave.')
      });
    }
  }

  rejectLeave(id: number) {
    const reason = prompt('Enter rejection reason (optional):');
    this.api.rejectLeave(id, reason || '').subscribe({
      next: () => { this.loadLeaves(); this.loadPendingLeaves(); },
      error: (err) => alert(err.error?.message || 'Error rejecting leave.')
    });
  }

  onSubmit() {
    if (this.leaveForm.valid) {
      this.api.applyLeave(this.leaveForm.value).subscribe({
        next: () => {
          alert('Leave request submitted successfully!');
          this.leaveForm.reset();
          this.loadLeaves();
        },
        error: (err) => alert(err.error?.message || 'Error applying leave.')
      });
    }
  }
}
