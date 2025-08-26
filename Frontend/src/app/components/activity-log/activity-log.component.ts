import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivityLogService, ActivityLog } from '../../services/activity-log.service';

@Component({
  selector: 'app-activity-log',
  standalone: true,
  imports: [
    CommonModule, 
    MatTableModule, 
    MatButtonModule, 
    MatIconModule, 
    MatCardModule, 
    MatProgressSpinnerModule
  ],
  templateUrl: './activity-log.component.html',
  styleUrl: './activity-log.component.scss'
})
export class ActivityLogComponent implements OnInit {
  activities: ActivityLog[] = [];
  displayedColumns: string[] = ['timestamp', 'userName', 'action', 'details'];
  isLoading = false;

  constructor(private activityLogService: ActivityLogService) {}

  ngOnInit(): void {
    this.loadActivityLog();
  }

  loadActivityLog(): void {
    this.isLoading = true;
    console.log('Loading activity log...');
    
    this.activityLogService.getActivityLog().subscribe({
      next: (response) => {
        this.isLoading = false;
        console.log('Activity log response:', response);
        if (response.success && response.data) {
          this.activities = response.data;
        } else {
          console.error('Failed to load activity log:', response.message);
          this.activities = [];
        }
      },
      error: (error) => {
        this.isLoading = false;
        console.error('Activity log error:', error);
        this.activities = [];
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }
}
