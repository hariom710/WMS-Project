import { Component, OnDestroy, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth';
import { HttpClient } from '@angular/common/http';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent implements OnDestroy {
  private destroyRef = inject(DestroyRef);

  loginForm: FormGroup;
  errorMessage: string = '';
  setupMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private http: HttpClient
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  ngOnDestroy(): void {}

  setupAdmin() {
    this.http.post(`${environment.apiUrl}/Auth/setup-default-admin`, {}).pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: (res: any) => {
        this.setupMessage = res.message;
      },
      error: () => {
        this.setupMessage = 'Error setting up admin. Check server.';
      }
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).pipe(
        takeUntilDestroyed(this.destroyRef)
      ).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          this.errorMessage = 'Invalid Username or Password';
          console.error(err);
        }
      });
    }
  }
}
