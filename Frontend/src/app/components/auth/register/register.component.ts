import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth.service';
import { LoaderService } from '../../../services/loader.service';
import { RegisterRequest } from '../../../models/interfaces';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, MatSnackBarModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  userData: RegisterRequest = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    role: ''
  };

  roleOptions = [
    { value: 'Admin', label: 'Administrator' },
    { value: 'Receptionist', label: 'Receptionist' },
    { value: 'Doctor', label: 'Doctor' }
  ];

  constructor(
    private authService: AuthService,
    private loaderService: LoaderService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onSubmit(): void {
    if (!this.isFormValid()) {
      this.snackBar.open('Please fill in all fields', 'Close', { duration: 3000 });
      return;
    }

    this.loaderService.show();
    this.authService.register(this.userData).subscribe({
      next: (response) => {
        this.loaderService.hide();
        if (response.success) {
          this.snackBar.open('Registration successful! Please login.', 'Close', { duration: 3000 });
          this.router.navigate(['/login']);
        } else {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.loaderService.hide();
        this.snackBar.open('Registration failed. Please try again.', 'Close', { duration: 3000 });
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  private isFormValid(): boolean {
    return !!(
      this.userData.email &&
      this.userData.password &&
      this.userData.firstName &&
      this.userData.lastName &&
      this.userData.role
    );
  }
}
