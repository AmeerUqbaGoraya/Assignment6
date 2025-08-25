import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { Patient } from '../../../models/interfaces';
import { PatientService } from '../../../services/patient.service';
import { AuthService } from '../../../services/auth.service';
import { LoaderService } from '../../../services/loader.service';
import { DialogConfirmComponent } from '../../shared/dialog-confirm/dialog-confirm.component';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    FormsModule,
    MatTableModule, 
    MatButtonModule, 
    MatIconModule, 
    MatFormFieldModule, 
    MatInputModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltipModule
  ],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss'
})
export class PatientListComponent implements OnInit {
  patients: Patient[] = [];
  allPatients: Patient[] = [];
  searchTerm: string = '';
  private searchSubject = new Subject<string>();
  displayedColumns: string[] = ['name', 'dateOfBirth', 'gender', 'phone', 'email', 'actions'];

  constructor(
    private patientService: PatientService,
    private authService: AuthService,
    private loaderService: LoaderService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.performSearch());
  }

  ngOnInit(): void {
    this.loadPatients();
    console.log('PatientListComponent initialized');
    console.log('Current user:', this.authService.getCurrentUser());
    console.log('Can create patients:', this.canCreatePatients());
    console.log('Can edit patients:', this.canEditPatients());
    console.log('Can delete patients:', this.canDeletePatients());
  }

  loadPatients(): void {
    this.loaderService.show();
    this.patientService.getPatients().subscribe({
      next: (response) => {
        this.loaderService.hide();
        if (response.success && response.data) {
          console.log('Loaded patients:', response.data);
          this.patients = response.data;
          this.allPatients = [...response.data];
        } else {
          this.snackBar.open(response.message, 'Close', { duration: 3000 });
        }
      },
      error: (error) => {
        this.loaderService.hide();
        console.error('Error loading patients:', error);
        this.snackBar.open('Failed to load patients', 'Close', { duration: 3000 });
      }
    });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchTerm);
  }

  performSearch(): void {
    if (!this.searchTerm.trim()) {
      this.patients = [...this.allPatients];
      return;
    }

    const searchLower = this.searchTerm.toLowerCase();
    this.patients = this.allPatients.filter(patient =>
      this.getFullName(patient).toLowerCase().includes(searchLower) ||
      patient.email.toLowerCase().includes(searchLower) ||
      patient.phone.toLowerCase().includes(searchLower)
    );
  }

  searchPatients(): void {
    this.performSearch();
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.patients = [...this.allPatients];
  }

  deletePatient(patient: Patient): void {
    const patientId = patient.patientID || patient.id;
    const dialogRef = this.dialog.open(DialogConfirmComponent, {
      data: {
        title: 'Delete Patient',
        message: `Are you sure you want to delete ${patient.firstName} ${patient.lastName}?`
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed && patientId) {
        this.loaderService.show();
        this.patientService.deletePatient(patientId).subscribe({
          next: (response) => {
            this.loaderService.hide();
            if (response.success) {
              this.snackBar.open('Patient deleted successfully', 'Close', { duration: 3000 });
              this.loadPatients();
            } else {
              this.snackBar.open(response.message, 'Close', { duration: 3000 });
            }
          },
          error: (error) => {
            this.loaderService.hide();
            this.snackBar.open('Failed to delete patient', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  canCreatePatients(): boolean {
    return this.authService.canCreatePatients();
  }

  canEditPatients(): boolean {
    return this.authService.canEditPatients();
  }

  canDeletePatients(): boolean {
    return this.authService.canDeletePatients();
  }

  getFullName(patient: Patient): string {
    return `${patient.firstName} ${patient.lastName}`;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }
}
