import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
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
  
  // Added 'actions' to the columns array
  displayedColumns: string[] = ['departmentId', 'departmentName', 'description', 'createdOn', 'actions'];

  // Form State
  isEditMode: boolean = false;
  currentDepartmentId: number | null = null;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.departmentForm = this.fb.group({
      departmentId: [null], // Required to send the ID back during an update
      departmentName: ['', Validators.required],
      description: [''] 
    });
  }

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments() {
    this.api.getDepartments().subscribe({
      next: (data) => {
        this.departments = data;
      },
      error: (err) => console.error('Error fetching data', err)
    });
  }

  // --- EDIT LOGIC ---
  editDepartment(dept: any) {
    this.isEditMode = true;
    this.currentDepartmentId = dept.departmentId;
    
    // Populate the form with the selected department's data
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

  // --- DELETE LOGIC ---
  deleteDepartment(id: number) {
    if(confirm("Are you sure you want to completely delete this department?")) {
      this.api.deleteDepartment(id).subscribe({
        next: (res) => {
          alert(res.message);
          this.loadDepartments();
        },
        error: (err) => alert(err.error?.message || 'Error deleting department')
      });
    }
  }

  // --- SUBMIT LOGIC ---
  onSubmit() {
    if (this.departmentForm.valid) {
      
      // CRITICAL FIX: Create a copy of the form data so we can modify it safely
      const formData = { ...this.departmentForm.value };

      if (this.isEditMode && this.currentDepartmentId) {
        // UPDATE EXISTING (Keep the ID intact so C# knows which one to update)
        this.api.updateDepartment(this.currentDepartmentId, formData).subscribe({
          next: () => {
            alert('Department updated successfully!');
            this.cancelEdit();
            this.loadDepartments();
          },
          error: (err) => console.error('Error updating department:', err)
        });
      } else {
        // ADD NEW
        // Delete the null ID so C# doesn't crash during validation!
        delete formData.departmentId;

        this.api.addDepartment(formData).subscribe({
          next: () => {
            alert('Department successfully created!');
            this.departmentForm.reset();
            this.loadDepartments(); 
          },
          error: (err) => {
            console.error('Error creating department:', err);
            alert('Failed to save. Press F12 and check the Network tab for details.');
          }
        });
      }
    }
  }
}