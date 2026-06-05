import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../services/api';
import { AuthService } from '../auth/auth';
import { MatTableModule } from '@angular/material/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatTableModule],
  templateUrl: './clients.html',
  styleUrls: ['./clients.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientsComponent implements OnInit, OnDestroy {
  private destroyRef = inject(DestroyRef);

  clientForm: FormGroup;
  clients: any[] = [];
  displayedColumns: string[] = ['name', 'phone', 'location', 'status'];
  isEditMode: boolean = false;
  currentClientId: number | null = null;
  serverError: string = '';
  exportingExcel = false;

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    public authService: AuthService
  ) {
    this.clientForm = this.fb.group({
      clientName: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]],
      clientPhoneNumber: ['', [
        Validators.required,
        Validators.pattern(/^[0-9]{10}$/)
      ]],
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

  ngOnDestroy(): void {}

  trackByClientId(_index: number, client: any): number {
    return client.clientId;
  }

  loadClients() {
    this.api.getClients().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (data) => this.clients = data,
      error: (err) => console.error(err)
    });
  }

  editClient(client: any) {
    this.isEditMode = true;
    this.currentClientId = client.clientId;
    this.serverError = '';
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
    this.serverError = '';
    this.clientForm.reset();
  }

  deleteClient(id: number) {
    if (confirm("Delete this client record permanently?")) {
      this.api.deleteClient(id).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => this.loadClients(),
        error: (err: any) => alert(err.error?.message || 'Error deleting client.')
      });
    }
  }

  onSubmit() {
    this.serverError = '';
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }

    const formData = { ...this.clientForm.value };
    formData.clientName = formData.clientName?.trim();
    formData.clientPhoneNumber = formData.clientPhoneNumber?.trim();
    formData.clientLocation = formData.clientLocation?.trim();
    formData.clientAddress = formData.clientAddress?.trim();

    if (this.isEditMode && this.currentClientId) {
      this.api.updateClient(this.currentClientId, formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Client updated!');
          this.cancelEdit();
          this.loadClients();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to update client.';
        }
      });
    } else {
      this.api.addClient(formData).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          alert('Client added successfully!');
          this.clientForm.reset();
          this.loadClients();
        },
        error: (err: any) => {
          this.serverError = err.error?.message || 'Failed to add client.';
        }
      });
    }
  }

  get f() { return this.clientForm.controls; }

  exportExcel() {
    this.exportingExcel = true;
    this.api.exportClientsExcel().pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (blob) => {
        this.api.downloadBlob(blob, `Clients_${new Date().toISOString().slice(0,10)}.xlsx`);
        this.exportingExcel = false;
      },
      error: () => { this.exportingExcel = false; alert('Failed to export Excel.'); }
    });
  }
}
