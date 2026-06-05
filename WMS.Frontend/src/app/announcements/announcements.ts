import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './announcements.html',
  styleUrls: ['./announcements.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnnouncementsComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  alertForm: FormGroup;
  alerts: any[] = [];
  isEditMode: boolean = false;
  currentAnnouncementId: number | null = null;
  serverError: string = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.alertForm = this.fb.group({
      title: ['', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(200)
      ]],
      message: ['', [
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(2000)
      ]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  ngOnDestroy(): void {}

  trackByAnnouncementId(_index: number, alert: any): number {
    return alert.announcementId;
  }

  loadAnnouncements() {
    this.api.getAlerts().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.alerts = data,
      error: (err) => console.error(err)
    });
  }

  editAlert(alert: any) {
    this.isEditMode = true;
    this.currentAnnouncementId = alert.announcementId;
    this.serverError = '';
    this.alertForm.patchValue({
      title: alert.title,
      message: alert.message,
      isActive: alert.isActive
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentAnnouncementId = null;
    this.serverError = '';
    this.alertForm.reset({ isActive: true });
  }

  deleteAlert(id: number) {
    if (confirm("Permanently delete this announcement?")) {
      this.api.deleteAnnouncement(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadAnnouncements(),
        error: (err: any) => alert(err.error?.message || 'Error deleting announcement.')
      });
    }
  }

  onSubmit() {
    this.serverError = '';
    if (this.alertForm.invalid) {
      this.alertForm.markAllAsTouched();
      return;
    }

    const formData = { ...this.alertForm.value };
    formData.title = formData.title?.trim();
    formData.message = formData.message?.trim();

    if (this.isEditMode && this.currentAnnouncementId) {
      this.api.updateAnnouncement(this.currentAnnouncementId, formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Announcement updated!');
          this.cancelEdit();
          this.loadAnnouncements();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to update announcement.';
        }
      });
    } else {
      this.api.addAnnouncement(formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Announcement posted successfully!');
          this.alertForm.reset({ isActive: true });
          this.loadAnnouncements();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to create announcement.';
        }
      });
    }
  }

  get f() { return this.alertForm.controls; }
}
