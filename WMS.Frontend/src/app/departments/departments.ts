import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './departments.html',
  styleUrls: ['./departments.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentsComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  departmentForm: FormGroup;
  departments: any[] = [];
  displayedColumns: string[] = ['departmentId', 'departmentName', 'description', 'createdOn'];
  isEditMode: boolean = false;
  currentDepartmentId: number | null = null;
  serverError: string = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.departmentForm = this.fb.group({
      departmentId: [null],
      departmentName: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100),
        Validators.pattern(/^(?!\s*$).+/)
      ]],
      description: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('actions');
    }
    this.loadDepartments();
  }

  ngOnDestroy(): void {}

  trackByDepartmentId(_index: number, dept: any): number {
    return dept.departmentId;
  }

  loadDepartments() {
    this.api.getDepartments().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.departments = data,
      error: (err) => console.error(err)
    });
  }

  editDepartment(dept: any) {
    this.isEditMode = true;
    this.currentDepartmentId = dept.departmentId;
    this.serverError = '';
    this.departmentForm.patchValue({
      departmentId: dept.departmentId,
      departmentName: dept.departmentName,
      description: dept.description
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentDepartmentId = null;
    this.serverError = '';
    this.departmentForm.reset();
  }

  deleteDepartment(id: number) {
    if (confirm("Are you sure you want to completely delete this department?")) {
      this.api.deleteDepartment(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: (res) => {
          alert(res.message);
          this.loadDepartments();
        },
        error: (err) => alert(err.error?.message || 'Failed to delete department.')
      });
    }
  }

  onSubmit() {
    this.serverError = '';
    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    const formData = { ...this.departmentForm.value };
    formData.departmentName = formData.departmentName?.trim();
    if (formData.description) formData.description = formData.description.trim();

    if (this.isEditMode && this.currentDepartmentId) {
      this.api.updateDepartment(this.currentDepartmentId, formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Department updated successfully!');
          this.cancelEdit();
          this.loadDepartments();
        },
        error: (err) => {
          this.serverError = err.error?.message || 'Failed to update department.';
        }
      });
    } else {
      delete formData.departmentId;
      this.api.addDepartment(formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Department successfully created!');
          this.departmentForm.reset();
          this.loadDepartments();
        },
        error: (err) => {
          this.serverError = err.error?.message || 'Failed to create department.';
        }
      });
    }
  }

  get f() { return this.departmentForm.controls; }
}
