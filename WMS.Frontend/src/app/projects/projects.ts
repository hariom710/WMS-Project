import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './projects.html'
})
export class ProjectsComponent implements OnInit {
  projectForm: FormGroup;
  projects: any[] = [];
  editProject: any = null;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.projectForm = this.fb.group({
      projectName: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      status: ['Active', Validators.required],
      clientId: [null]
    });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects() {
    this.api.getProjects().subscribe({
      next: (data) => this.projects = data,
      error: (err) => console.error(err)
    });
  }

  onEdit(project: any) {
    this.editProject = project;
    this.projectForm.patchValue({
      projectName: project.projectName,
      startDate: project.startDate ? project.startDate.split('T')[0] : '',
      endDate: project.endDate ? project.endDate.split('T')[0] : '',
      status: project.status,
      clientId: project.clientId
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.editProject = null;
    this.projectForm.reset({ status: 'Active', clientId: null });
  }

  onDelete(id: number) {
    if (confirm('Delete this project permanently?')) {
      this.api.deleteProject(id).subscribe({
        next: () => this.loadProjects(),
        error: (err) => alert(err.error?.message || 'Error deleting project')
      });
    }
  }

  onSubmit() {
    if (this.projectForm.valid) {
      const value = this.projectForm.value;

      if (this.editProject) {
        this.api.updateProject(this.editProject.projectId, value).subscribe({
          next: () => {
            alert('Project updated!');
            this.cancelEdit();
            this.loadProjects();
          },
          error: (err) => alert('Update failed.')
        });
      } else {
        this.api.addProject(value).subscribe({
          next: () => {
            alert('New Project Created Successfully!');
            this.projectForm.reset({ status: 'Active', clientId: null });
            this.loadProjects();
          },
          error: (err) => {
            console.error(err);
            alert('Failed to create project.');
          }
        });
      }
    }
  }
}
