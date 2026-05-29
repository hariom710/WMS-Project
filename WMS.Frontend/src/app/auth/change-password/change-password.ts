import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../auth';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.html'
})
export class ChangePasswordComponent {
  passwordForm: FormGroup;
  message: string = '';
  isError: boolean = false;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.passwordForm = this.fb.group({
      // Automatically grab the logged-in user's username from local storage
      username: [localStorage.getItem('username'), Validators.required],
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.passwordForm.valid) {
      this.authService.changePassword(this.passwordForm.value).subscribe({
        next: (res: any) => {
          this.isError = false;
          this.message = res.message;
          this.passwordForm.reset({ username: localStorage.getItem('username') });
        },
        error: (err) => {
          this.isError = true;
          this.message = err.error.message || 'An error occurred.';
        }
      });
    }
  }
}