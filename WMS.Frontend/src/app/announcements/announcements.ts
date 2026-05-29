import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './announcements.html'
})
export class AnnouncementsComponent implements OnInit {
  alertForm: FormGroup;
  alerts: any[] = [];
  
  // Form State
  isEditMode: boolean = false;
  currentAlertId: number | null = null;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.alertForm = this.fb.group({
      announcementId: [null], // Required for updates
      title: ['', Validators.required],
      message: ['', Validators.required],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadAlerts();
  }

  loadAlerts() {
    this.api.getAlerts().subscribe(data => this.alerts = data);
  }

  // --- EDIT LOGIC ---
  editAlert(alert: any) {
    this.isEditMode = true;
    this.currentAlertId = alert.announcementId;
    
    this.alertForm.patchValue({
      announcementId: alert.announcementId,
      title: alert.title,
      message: alert.message,
      isActive: alert.isActive
    });
    
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentAlertId = null;
    this.alertForm.reset({ isActive: true });
  }

  // --- DELETE LOGIC ---
  deleteAlert(id: number) {
    if(confirm("Are you sure you want to permanently delete this notice?")) {
      this.api.deleteAnnouncement(id).subscribe({
        next: () => this.loadAlerts(),
        error: (err) => alert('Error deleting announcement')
      });
    }
  }

  // --- SUBMIT LOGIC ---
  onSubmit() {
    if (this.alertForm.valid) {
      // Create a copy of the form data
      const payload = { ...this.alertForm.value };

      if (this.isEditMode && this.currentAlertId) {
        // UPDATE EXISTING
        this.api.updateAnnouncement(this.currentAlertId, payload).subscribe({
          next: () => {
            alert('System Announcement Updated!');
            this.cancelEdit();
            this.loadAlerts();
          },
          error: (err) => alert('Error updating announcement: ' + (err.error?.title || err.message))
        });
      } else {
        // CREATE NEW
        // CRITICAL FIX: Remove the null ID so C# model validation doesn't crash!
        delete payload.announcementId;

        this.api.addAnnouncement(payload).subscribe({
          next: () => {
            alert('System Announcement Posted!');
            this.alertForm.reset({ isActive: true });
            this.loadAlerts(); 
          },
          error: (err) => alert('Error posting announcement: ' + (err.error?.title || err.message))
        });
      }
    } else {
      // NO MORE SILENT FAILURES
      alert("Please make sure both the Notice Title and Message Details are filled out!");
    }
  }
}