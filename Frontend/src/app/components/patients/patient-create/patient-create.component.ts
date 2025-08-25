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
import { PatientService } from '../../../services/patient.service';
import { LoaderService } from '../../../services/loader.service';
import { CreatePatientRequest } from '../../../models/interfaces';

@Component({
  selector: 'app-patient-create',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatSelectModule, 
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './patient-create.component.html',
  styleUrl: './patient-create.component.scss'
})
export class PatientCreateComponent {
  patient: CreatePatientRequest = {
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
    phone: '',
    email: '',
    address: '',
    emergencyContact: '',
    emergencyPhone: '',
    insuranceInfo: ''
  };

  genderOptions = ['Male', 'Female', 'Other'];

  constructor(
    private patientService: PatientService,
    private loaderService: LoaderService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onSubmit(): void {
    if (!this.isFormValid()) {
      this.snackBar.open('Please fill in all required fields', 'Close', { duration: 3000 });
      return;
    }

    this.loaderService.show();
    this.patientService.createPatient(this.patient).subscribe({
      next: (response) => {
        this.loaderService.hide();
        if (response.success) {
          this.snackBar.open('Patient created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/patients']);
        } else {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.loaderService.hide();
        this.snackBar.open('Failed to create patient', 'Close', { duration: 3000 });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/patients']);
  }

  private isFormValid(): boolean {
    return !!(
      this.patient.firstName &&
      this.patient.lastName &&
      this.patient.dateOfBirth &&
      this.patient.gender &&
      this.patient.phone &&
      this.patient.email
    );
  }
}
