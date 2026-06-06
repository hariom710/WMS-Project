import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

function minimumAgeValidator(minAge: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const dob = new Date(control.value);
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const monthDiff = today.getMonth() - dob.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
      age--;
    }
    return age < minAge ? { minimumAge: { required: minAge, actual: age } } : null;
  };
}

function futureDateValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const date = new Date(control.value);
    return date > new Date() ? { futureDate: true } : null;
  };
}

function dateRangeValidator(): ValidatorFn {
  return (form: AbstractControl): ValidationErrors | null => {
    const dob = form.get('dateOfBirth')?.value;
    const doj = form.get('dateOfJoining')?.value;
    if (!dob || !doj) return null;
    if (new Date(doj) < new Date(dob)) {
      return { dateRangeInvalid: true };
    }
    return null;
  };
}

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './employees.html',
  styleUrls: ['./employees.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeesComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  employeeForm: FormGroup;
  dataSource = new MatTableDataSource<any>();
  departments: any[] = [];
  roles: any[] = [];

  displayedColumns: string[] = ['id', 'name', 'email', 'department', 'role', 'status'];
  isEditMode: boolean = false;
  currentEmployeeId: number | null = null;
  serverError: string = '';
  exportingExcel = false;
  exportingPdf = false;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.employeeForm = this.fb.group({
      employeeId: [null],
      firstName: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
        Validators.pattern(/^[A-Za-z ]+$/)
      ]],
      lastName: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
        Validators.pattern(/^[A-Za-z ]+$/)
      ]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(80)]],
      phoneNumber: ['', [
        Validators.required,
        Validators.pattern(/^[0-9]{10}$/)
      ]],
      gender: ['M', Validators.required],
      dateOfBirth: ['', [Validators.required, minimumAgeValidator(18)]],
      dateOfJoining: ['', [Validators.required, futureDateValidator()]],
      departmentId: [null, Validators.required],
      roleId: [null, Validators.required],
      status: ['Active']
    }, { validators: dateRangeValidator() });
  }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('actions');
    }
    this.loadData();

    this.dataSource.filterPredicate = (data: any, filter: string) => {
      const dataStr = [
        data.employeeId,
        data.firstName,
        data.lastName,
        data.email,
        data.departmentName,
        data.roleName
      ].join(' ').toLowerCase();
      return dataStr.includes(filter);
    };
  }

  ngOnDestroy(): void {}

  trackByEmployeeId(_index: number, emp: any): number {
    return emp.employeeId;
  }

  trackByDepartmentId(_index: number, dept: any): number {
    return dept.departmentId;
  }

  trackByRoleId(_index: number, role: any): number {
    return role.roleId;
  }

  loadData() {
    this.api.getEmployees().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.dataSource.data = data);

    this.api.getDepartments().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.departments = data);

    this.api.getRoles().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(data => this.roles = data);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  editEmployee(emp: any) {
    this.isEditMode = true;
    this.currentEmployeeId = emp.employeeId;
    this.serverError = '';

    const dob = emp.dateOfBirth ? new Date(emp.dateOfBirth).toISOString().split('T')[0] : '';
    const doj = emp.dateOfJoining ? new Date(emp.dateOfJoining).toISOString().split('T')[0] : '';

    this.employeeForm.patchValue({
      employeeId: emp.employeeId,
      firstName: emp.firstName,
      lastName: emp.lastName,
      email: emp.email,
      phoneNumber: emp.phoneNumber,
      gender: emp.gender,
      dateOfBirth: dob,
      dateOfJoining: doj,
      departmentId: emp.departmentId,
      roleId: emp.roleId,
      status: emp.status
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentEmployeeId = null;
    this.serverError = '';
    this.employeeForm.reset({ gender: 'M', status: 'Active', departmentId: null, roleId: null });
  }

  onSubmit() {
    this.serverError = '';
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const payload: any = { ...this.employeeForm.value };
    payload.firstName = payload.firstName?.trim();
    payload.lastName = payload.lastName?.trim();
    payload.email = payload.email?.trim().toLowerCase();
    payload.phoneNumber = payload.phoneNumber?.trim();
    payload.departmentId = Number(payload.departmentId);
    payload.roleId = Number(payload.roleId);

    if (this.isEditMode && this.currentEmployeeId) {
      this.api.updateEmployee(this.currentEmployeeId, payload).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Employee details updated successfully!');
          this.cancelEdit();
          this.loadData();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to update employee.';
        }
      });
    } else {
      delete payload.employeeId;
      this.api.addEmployee(payload).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Employee added! Their login (Email & Welcome@123) was auto-generated.');
          this.cancelEdit();
          this.loadData();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to add employee.';
        }
      });
    }
  }

  get f() { return this.employeeForm.controls; }

  exportExcel() {
    this.exportingExcel = true;
    this.api.exportEmployeesExcel().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Employees_${new Date().toISOString().slice(0,10)}.xlsx`);
        this.exportingExcel = false;
      },
      error: () => { this.exportingExcel = false; alert('Failed to export Excel.'); }
    });
  }

  exportPdf() {
    this.exportingPdf = true;
    this.api.exportEmployeesPdf().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Employees_${new Date().toISOString().slice(0,10)}.pdf`);
        this.exportingPdf = false;
      },
      error: () => { this.exportingPdf = false; alert('Failed to export PDF.'); }
    });
  }
}
