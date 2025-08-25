import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { PatientService } from '../../../services/patient.service';
import { AuthService } from '../../../services/auth.service';
import { LoaderService } from '../../../services/loader.service';
import { Patient } from '../../../models/interfaces';

@Component({
  selector: 'app-patient-view',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule,
    MatCardModule, 
    MatButtonModule, 
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './patient-view.component.html',
  styleUrl: './patient-view.component.scss'
})
export class PatientViewComponent implements OnInit {
  patient: Patient | null = null;
  patientId: number = 0;

  constructor(
    private patientService: PatientService,
    private authService: AuthService,
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
    this.loaderService.show();
    this.patientService.getPatient(this.patientId).subscribe({
      next: (response) => {
        this.loaderService.hide();
        if (response.success && response.data) {
          this.patient = response.data;
        } else {
          this.snackBar.open('Patient not found', 'Close', { duration: 3000 });
          this.router.navigate(['/patients']);
        }
      },
      error: (error) => {
        this.loaderService.hide();
        this.snackBar.open('Failed to load patient', 'Close', { duration: 3000 });
        this.router.navigate(['/patients']);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/patients']);
  }

  editPatient(): void {
    const patientId = this.patient?.patientID || this.patient?.id;
    if (patientId) {
      this.router.navigate(['/patients/edit', patientId]);
    }
  }

  canEdit(): boolean {
    return this.authService.canEditPatients();
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }
}
