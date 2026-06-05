import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

function projectDateRangeValidator(): ValidatorFn {
  return (form: AbstractControl): ValidationErrors | null => {
    const start = form.get('startDate')?.value;
    const end = form.get('endDate')?.value;
    if (start && end && new Date(end) < new Date(start)) {
      return { projectDateRange: true };
    }
    return null;
  };
}

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './projects.html',
  styleUrls: ['./projects.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectsComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  projectForm: FormGroup;
  projects: any[] = [];
  editProject: any = null;
  serverError: string = '';
  exportingExcel = false;
  exportingPdf = false;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.projectForm = this.fb.group({
      projectName: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      status: ['Active', Validators.required],
      clientId: [null]
    }, { validators: projectDateRangeValidator() });
  }

  ngOnInit(): void {
    this.loadProjects();
  }

  ngOnDestroy(): void {}

  trackByProjectId(_index: number, project: any): number {
    return project.projectId;
  }

  loadProjects() {
    this.api.getProjects().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.projects = data,
      error: (err) => console.error(err)
    });
  }

  onEdit(project: any) {
    this.editProject = project;
    this.serverError = '';
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
    this.serverError = '';
    this.projectForm.reset({ status: 'Active', clientId: null });
  }

  onDelete(id: number) {
    if (confirm('Delete this project permanently?')) {
      this.api.deleteProject(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadProjects(),
        error: (err) => alert(err.error?.message || 'Error deleting project.')
      });
    }
  }

  onSubmit() {
    this.serverError = '';
    if (this.projectForm.invalid) {
      this.projectForm.markAllAsTouched();
      return;
    }

    const value = { ...this.projectForm.value };
    value.projectName = value.projectName?.trim();

    if (this.editProject) {
      this.api.updateProject(this.editProject.projectId, value).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Project updated!');
          this.cancelEdit();
          this.loadProjects();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to update project.';
        }
      });
    } else {
      this.api.addProject(value).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('New Project Created Successfully!');
          this.projectForm.reset({ status: 'Active', clientId: null });
          this.loadProjects();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to create project.';
        }
      });
    }
  }

  get f() { return this.projectForm.controls; }

  exportExcel() {
    this.exportingExcel = true;
    this.api.exportProjectsExcel().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Projects_${new Date().toISOString().slice(0,10)}.xlsx`);
        this.exportingExcel = false;
      },
      error: () => { this.exportingExcel = false; alert('Failed to export Excel.'); }
    });
  }

  exportPdf() {
    this.exportingPdf = true;
    this.api.exportProjectsPdf().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Projects_${new Date().toISOString().slice(0,10)}.pdf`);
        this.exportingPdf = false;
      },
      error: () => { this.exportingPdf = false; alert('Failed to export PDF.'); }
    });
  }
}
