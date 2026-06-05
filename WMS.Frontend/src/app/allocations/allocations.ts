import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-allocations',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './allocations.html',
  styleUrls: ['./allocations.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AllocationsComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  allocationForm: FormGroup;
  allocations: any[] = [];
  employees: any[] = [];
  projects: any[] = [];
  serverError: string = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.allocationForm = this.fb.group({
      empId: [null, Validators.required],
      projectId: [null, Validators.required],
      assignedOn: [new Date().toISOString().split('T')[0], Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {}

  trackByEmployeeId(_index: number, emp: any): number {
    return emp.employeeId;
  }

  trackByProjectId(_index: number, project: any): number {
    return project.projectId;
  }

  trackByAllocationId(_index: number, alloc: any): number {
    return alloc.allocationId;
  }

  loadData() {
    this.api.getAllocations().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.allocations = data);

    this.api.getEmployees().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.employees = data);

    this.api.getProjects().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => {
      this.projects = data.filter((p: any) => p.status === 'Active');
    });
  }

  deleteAllocation(id: number) {
    if (confirm("Remove this employee from the project?")) {
      this.api.deleteAllocation(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadData(),
        error: (err: any) => alert(err.error?.message || 'Error removing allocation.')
      });
    }
  }

  onSubmit() {
    this.serverError = '';
    if (this.allocationForm.invalid) {
      this.allocationForm.markAllAsTouched();
      return;
    }

    this.api.addAllocation(this.allocationForm.value).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => {
        alert('Employee successfully assigned to project!');
        this.allocationForm.reset({ assignedOn: new Date().toISOString().split('T')[0] });
        this.loadData();
      },
      error: (err: any) => {
        this.serverError = err.error?.message || 'Error assigning employee.';
      }
    });
  }

  get f() { return this.allocationForm.controls; }
}
