import { Component, OnDestroy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../auth';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './change-password.html'
})
export class ChangePasswordComponent implements OnDestroy {
  private destroyRef = inject(DestroyRef);

  passwordForm: FormGroup;
  message: string = '';
  isError: boolean = false;

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.passwordForm = this.fb.group({
      username: [this.authService.getUsername(), Validators.required],
      oldPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnDestroy(): void {}

  onSubmit() {
    if (this.passwordForm.valid) {
      this.authService.changePassword(this.passwordForm.value).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: (res: any) => {
          this.isError = false;
          this.message = res.message;
          this.passwordForm.reset({ username: this.authService.getUsername() });
        },
        error: (err) => {
          this.isError = true;
          this.message = err.error.message || 'An error occurred.';
        }
      });
    }
  }
}
