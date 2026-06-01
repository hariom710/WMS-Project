import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './departments.html',
  styleUrls: ['./departments.css']
})
export class DepartmentsComponent implements OnInit {
  departmentForm: FormGroup;
  departments: any[] = [];
  displayedColumns: string[] = ['departmentId', 'departmentName', 'description', 'createdOn'];
  isEditMode: boolean = false;
  currentDepartmentId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.departmentForm = this.fb.group({
      departmentId: [null],
      departmentName: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('actions');
    }
    this.loadDepartments();
  }

  loadDepartments() {
    this.api.getDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => console.error(err)
    });
  }

  editDepartment(dept: any) {
    this.isEditMode = true;
    this.currentDepartmentId = dept.departmentId;
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
    this.departmentForm.reset();
  }

  deleteDepartment(id: number) {
    if(confirm("Are you sure you want to completely delete this department?")) {
      this.api.deleteDepartment(id).subscribe({
        next: (res) => {
          alert(res.message);
          this.loadDepartments();
        },
        error: (err) => alert(err.error?.message)
      });
    }
  }

  onSubmit() {
    if (this.departmentForm.valid) {
      const formData = { ...this.departmentForm.value };

      if (this.isEditMode && this.currentDepartmentId) {
        this.api.updateDepartment(this.currentDepartmentId, formData).subscribe({
          next: () => {
            alert('Department updated successfully!');
            this.cancelEdit();
            this.loadDepartments();
          },
          error: (err) => console.error(err)
        });
      } else {
        delete formData.departmentId;
        this.api.addDepartment(formData).subscribe({
          next: () => {
            alert('Department successfully created!');
            this.departmentForm.reset();
            this.loadDepartments();
          },
          error: (err) => {
            console.error(err);
            alert('Failed to save.');
          }
        });
      }
    }
  }
}
