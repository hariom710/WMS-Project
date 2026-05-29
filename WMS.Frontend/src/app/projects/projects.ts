import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './projects.html'
})
export class ProjectsComponent implements OnInit {
  projectForm: FormGroup;
  projects: any[] = [];

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.projectForm = this.fb.group({
      projectName: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      status: ['Active', Validators.required],
      clientId: [null] // Added to safely pass null to the backend
    });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects() {
    this.api.getProjects().subscribe({
      next: (data) => this.projects = data,
      error: (err) => console.error('Error fetching projects', err)
    });
  }

  onSubmit() {
    if (this.projectForm.valid) {
      this.api.addProject(this.projectForm.value).subscribe({
        next: () => {
          alert('New Project Created Successfully!');
          this.projectForm.reset({ status: 'Active', clientId: null });
          this.loadProjects(); // Refresh the table
        },
        error: (err) => {
          console.error('Error creating project', err);
          alert('Failed to create project. Please check the console.');
        }
      });
    }
  }
}