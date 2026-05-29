import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
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
  
  displayedColumns: string[] = ['name', 'phone', 'location', 'status', 'actions'];
  isEditMode: boolean = false;
  currentClientId: number | null = null;

  constructor(private fb: FormBuilder, private api: ApiService) {
    this.clientForm = this.fb.group({
      clientId: [null],
      clientName: ['', Validators.required],
      clientPhoneNumber: [''],
      clientLocation: [''],
      clientAddress: [''],
      status: [true]
    });
  }

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients() {
    this.api.getClients().subscribe(data => this.clients = data);
  }

  editClient(client: any) {
    this.isEditMode = true;
    this.currentClientId = client.clientId;
    this.clientForm.patchValue(client);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditMode = false;
    this.currentClientId = null;
    this.clientForm.reset({ status: true });
  }

  deleteClient(id: number) {
    if(confirm("Are you sure you want to delete this client?")) {
      this.api.deleteClient(id).subscribe(() => this.loadClients());
    }
  }

  onSubmit() {
    if (this.clientForm.valid) {
      const formData = { ...this.clientForm.value };

      if (this.isEditMode && this.currentClientId) {
        this.api.updateClient(this.currentClientId, formData).subscribe(() => {
          alert('Client updated!');
          this.cancelEdit();
          this.loadClients();
        });
      } else {
        delete formData.clientId; // Prevent ID null crash on insert
        this.api.addClient(formData).subscribe(() => {
          alert('Client added!');
          this.clientForm.reset({ status: true });
          this.loadClients();
        });
      }
    }
  }
}