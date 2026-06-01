import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatTableModule],
  templateUrl: './clients.html'
})
export class ClientsComponent implements OnInit {
  clientForm: FormGroup;
  clients: any[] = [];
  displayedColumns: string[] = ['name', 'phone', 'location', 'status'];
  isEditMode: boolean = false;
  currentClientId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.clientForm = this.fb.group({
      clientName: ['', Validators.required],
      clientPhoneNumber: ['', Validators.required],
      clientLocation: ['', Validators.required],
      clientAddress: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('actions');
    }
    this.loadClients();
  }

  loadClients() {
    this.api.getClients().subscribe({
      next: (data) => this.clients = data,
      error: (err) => console.error(err)
    });
  }

  editClient(client: any) {
    this.isEditMode = true;
    this.currentClientId = client.clientId;
    this.clientForm.patchValue({
      clientName: client.clientName,
      clientPhoneNumber: client.clientPhoneNumber,
      clientLocation: client.clientLocation,
      clientAddress: client.clientAddress
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentClientId = null;
    this.clientForm.reset();
  }

  deleteClient(id: number) {
    if (confirm("Delete this client record permanently?")) {
      this.api.deleteClient(id).subscribe({
        next: () => this.loadClients(),
        error: (err) => alert('Error deleting client.')
      });
    }
  }

  onSubmit() {
    if (this.clientForm.valid) {
      if (this.isEditMode && this.currentClientId) {
        this.api.updateClient(this.currentClientId, this.clientForm.value).subscribe({
          next: () => {
            alert('Client updated!');
            this.cancelEdit();
            this.loadClients();
          },
          error: (err) => alert('Update failed.')
        });
      } else {
        this.api.addClient(this.clientForm.value).subscribe({
          next: () => {
            alert('Client added successfully!');
            this.clientForm.reset();
            this.loadClients();
          },
          error: (err) => {
            console.error(err);
            alert('Failed to add client.');
          }
        });
      }
    }
  }
}
