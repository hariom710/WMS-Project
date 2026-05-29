import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-leaves',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './leaves.html'
})
export class LeavesComponent implements OnInit {
  leaveForm: FormGroup;
  leaves: any[] = [];
  pendingTeamLeaves: any[] = [];
  
  // NEW: Array to hold your employees for the dropdown
  employees: any[] = [];
  
  // Controls which screen is currently visible
  activeTab: 'my-leaves' | 'manager-approvals' = 'my-leaves';

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.leaveForm = this.fb.group({
      empId: [null, Validators.required], // <-- NEW: Employee ID field added
      leaveType: ['Sick', Validators.required],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      reason: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadMyLeaves();
    this.loadPendingLeaves();
    
    // NEW: Fetch employees from the backend when the page loads
    this.api.getEmployees().subscribe(data => this.employees = data);
  }

  loadMyLeaves() {
    this.api.getLeaves().subscribe(data => this.leaves = data);
  }

  loadPendingLeaves() {
    this.api.getPendingLeaves().subscribe(data => this.pendingTeamLeaves = data);
  }

  // --- EMPLOYEE ACTION ---
  onSubmit() {
    if (this.leaveForm.valid) {
      
      // Ensure empId is sent as a number
      const payload = { ...this.leaveForm.value };
      payload.empId = Number(payload.empId);

      this.api.applyLeave(payload).subscribe({
        next: () => {
          alert('Leave Application Submitted Successfully!');
          this.leaveForm.reset({ leaveType: 'Sick', empId: null });
          this.loadMyLeaves();
          this.loadPendingLeaves(); // Refresh manager queue
        },
        error: (err) => alert('Failed to submit application.')
      });
    } else {
      alert('Please fill out all required fields.');
    }
  }

  cancelLeave(id: number) {
    if (confirm("Are you sure you want to cancel this leave application?")) {
      this.api.cancelLeave(id).subscribe({
        next: () => {
          this.loadMyLeaves();
          this.loadPendingLeaves();
        },
        error: (err) => alert(err.error?.message || 'Error cancelling leave.')
      });
    }
  }

  // --- MANAGER ACTIONS ---
  approveLeave(id: number) {
    this.api.approveLeave(id).subscribe({
      next: () => {
        this.loadPendingLeaves();
        this.loadMyLeaves();
      },
      error: () => alert('Error approving leave.')
    });
  }

  rejectLeave(id: number) {
    if(confirm("Are you sure you want to reject this leave request?")) {
      this.api.rejectLeave(id).subscribe({
        next: () => {
          this.loadPendingLeaves();
          this.loadMyLeaves();
        },
        error: () => alert('Error rejecting leave.')
      });
    }
  }
}