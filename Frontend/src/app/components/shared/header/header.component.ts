import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/interfaces';
import { RoleDisplayPipe } from '../../../pipes/role-display.pipe';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule, MatDividerModule, RoleDisplayPipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  currentUser: User | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  navigateToPatients(): void {
    this.router.navigate(['/patients']);
  }

  navigateToCreatePatient(): void {
    this.router.navigate(['/patients/create']);
  }

  navigateToActivityLog(): void {
    this.router.navigate(['/activity-log']);
  }

  canCreatePatients(): boolean {
    return this.authService.canCreatePatients();
  }

  canViewActivityLog(): boolean {
    return this.authService.canViewActivityLog();
  }
}
