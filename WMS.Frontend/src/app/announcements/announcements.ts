import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, MatTableModule, ReactiveFormsModule],
  templateUrl: './announcements.html',
  styleUrls: ['./announcements.css']
})
export class AnnouncementsComponent implements OnInit {
  alertForm: FormGroup;
  alerts: any[] = [];
  isEditMode: boolean = false;
  currentAnnouncementId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.alertForm = this.fb.group({
      title: ['', Validators.required],
      message: ['', Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.api.getAlerts().subscribe({
      next: (data) => this.alerts = data,
      error: (err) => console.error(err)
    });
  }

  editAlert(alert: any) {
    this.isEditMode = true;
    this.currentAnnouncementId = alert.announcementId;
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
    this.alertForm.reset({ isActive: true });
  }

  deleteAlert(id: number) {
    if (confirm("Permanently delete this announcement?")) {
      this.api.deleteAnnouncement(id).subscribe({
        next: () => this.loadAnnouncements(),
        error: (err) => alert(err.error?.message)
      });
    }
  }

  onSubmit() {
    if (this.alertForm.valid) {
      if (this.isEditMode && this.currentAnnouncementId) {
        this.api.updateAnnouncement(this.currentAnnouncementId, this.alertForm.value).subscribe({
          next: () => {
            alert('Announcement updated!');
            this.cancelEdit();
            this.loadAnnouncements();
          },
          error: (err) => alert('Failed to update.')
        });
      } else {
        this.api.addAnnouncement(this.alertForm.value).subscribe({
          next: () => {
            alert('Announcement posted successfully!');
            this.alertForm.reset({ isActive: true });
            this.loadAnnouncements();
          },
          error: (err) => {
            console.error(err);
            alert('Failed to create announcement.');
          }
        });
      }
    }
  }
}
