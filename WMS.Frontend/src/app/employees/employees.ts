import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './employees.html',
  styleUrls: ['./employees.css']
})
export class EmployeesComponent implements OnInit {
  employeeForm: FormGroup;
  dataSource = new MatTableDataSource<any>();
  departments: any[] = [];
  roles: any[] = [];

  displayedColumns: string[] = ['id', 'name', 'email', 'department', 'role', 'status'];
  isEditMode: boolean = false;
  currentEmployeeId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.employeeForm = this.fb.group({
      employeeId: [null],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      gender: ['M', Validators.required],
      dateOfBirth: ['', Validators.required],
      dateOfJoining: ['', Validators.required],
      departmentId: [null, Validators.required],
      roleId: [null, Validators.required],
      status: ['Active']
    });
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
        data.department?.departmentName,
        data.role?.roleName
      ].join(' ').toLowerCase();
      return dataStr.includes(filter);
    };
  }

  loadData() {
    this.api.getEmployees().subscribe(data => {
      this.dataSource.data = data;
    });
    this.api.getDepartments().subscribe(data => this.departments = data);
    this.api.getRoles().subscribe(data => this.roles = data);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  editEmployee(emp: any) {
    this.isEditMode = true;
    this.currentEmployeeId = emp.employeeId;

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
    this.employeeForm.reset({ gender: 'M', status: 'Active', departmentId: null, roleId: null });
  }

  onSubmit() {
    if (this.employeeForm.valid) {
      const payload: any = { ...this.employeeForm.value };
      payload.departmentId = Number(payload.departmentId);
      payload.roleId = Number(payload.roleId);

      if (this.isEditMode && this.currentEmployeeId) {
        this.api.updateEmployee(this.currentEmployeeId, payload).subscribe({
          next: () => {
            alert('Employee details updated successfully!');
            this.cancelEdit();
            this.loadData();
          },
          error: (err: unknown) => console.error(err)
        });
      } else {
        delete payload.employeeId;
        this.api.addEmployee(payload).subscribe({
          next: () => {
            alert('Employee added! Their login (Email & Welcome@123) was auto-generated.');
            this.cancelEdit();
            this.loadData();
          },
          error: (err: unknown) => {
            console.error(err);
            alert('Failed to add employee.');
          }
        });
      }
    } else {
      alert("Please fill out all required fields correctly!");
    }
  }
}
