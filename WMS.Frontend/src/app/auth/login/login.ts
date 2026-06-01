import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth'; 
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html' 
})
export class LoginComponent {
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

  setupAdmin() {
    this.http.post('https://hariomwmsapi8501.azurewebsites.net/api/Auth/setup-default-admin', {}).subscribe({
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
      this.authService.login(this.loginForm.value).subscribe({
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