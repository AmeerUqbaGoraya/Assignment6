import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { PatientService } from '../../../services/patient.service';
import { LoaderService } from '../../../services/loader.service';
import { Patient, UpdatePatientRequest } from '../../../models/interfaces';

@Component({
  selector: 'app-patient-edit',
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
  templateUrl: './patient-edit.component.html',
  styleUrl: './patient-edit.component.scss'
})
export class PatientEditComponent implements OnInit {
  patient: UpdatePatientRequest = {
    id: 0,
    firstName: '',
    lastName: '',
    dateOfBirth: '',
    gender: '',
    phone: '',
    email: '',
    address: '',
    emergencyContact: '',
    emergencyPhone: ''
  };

  genderOptions = ['Male', 'Female', 'Other'];
  patientId: number = 0;
  isLoading = false;

  constructor(
    private patientService: PatientService,
    private loaderService: LoaderService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.patientId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.patientId) {
      this.loadPatient();
    }
  }

  loadPatient(): void {
    this.isLoading = true;
    this.patientService.getPatient(this.patientId).subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Load patient response:', response);
        if (response.success && response.data) {
          const patient = response.data;
          this.patient = {
            id: patient.patientID || patient.id || 0,
            firstName: patient.firstName,
            lastName: patient.lastName,
            dateOfBirth: patient.dateOfBirth,
            gender: patient.gender,
            phone: patient.phone,
            email: patient.email,
            address: patient.address,
            emergencyContact: patient.emergencyContact,
            emergencyPhone: patient.emergencyPhone
          };
        } else {
          this.snackBar.open('Patient not found', 'Close', { duration: 3000 });
          this.router.navigate(['/patients']);
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Load patient error:', error);
        this.snackBar.open('Failed to load patient', 'Close', { duration: 3000 });
        this.router.navigate(['/patients']);
      }
    });
  }

  onSubmit(): void {
    if (!this.isFormValid()) {
      this.snackBar.open('Please fill in all required fields', 'Close', { duration: 3000 });
      return;
    }

    this.isLoading = true;
    console.log('Updating patient:', this.patient);
    
    this.patientService.updatePatient(this.patient).subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Update response:', response);
        if (response.success) {
          this.snackBar.open('Patient updated successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/patients']);
        } else {
          this.snackBar.open(response.message || 'Failed to update patient', 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Update error:', error);
        this.snackBar.open('Failed to update patient', 'Close', { duration: 3000 });
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
