import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-allocations',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './allocations.html'
})
export class AllocationsComponent implements OnInit {
  allocationForm: FormGroup;
  allocations: any[] = [];
  employees: any[] = [];
  projects: any[] = [];

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.allocationForm = this.fb.group({
      empId: [null, Validators.required],
      projectId: [null, Validators.required],
      assignedOn: [new Date().toISOString().split('T')[0], Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    // Load existing allocations for the table
    this.api.getAllocations().subscribe(data => this.allocations = data);
    
    // Load dropdown data
    this.api.getEmployees().subscribe(data => this.employees = data);
    
    // Only allow assignment to projects that are actually "Active"
    this.api.getProjects().subscribe(data => {
      this.projects = data.filter((p: any) => p.status === 'Active');
    }); 
  }

  deleteAllocation(id: number) {
    if(confirm("Remove this employee from the project?")) {
      this.api.deleteAllocation(id).subscribe({
        next: () => this.loadData(),
        error: (err) => alert('Error removing allocation.')
      });
    }
  }

  onSubmit() {
    if (this.allocationForm.valid) {
      this.api.addAllocation(this.allocationForm.value).subscribe({
        next: () => {
          alert('Employee successfully assigned to project!');
          this.allocationForm.reset({ assignedOn: new Date().toISOString().split('T')[0] });
          this.loadData();
        },
        error: (err) => alert(err.error?.message || 'Error assigning employee. Check console.')
      });
    }
  }
}