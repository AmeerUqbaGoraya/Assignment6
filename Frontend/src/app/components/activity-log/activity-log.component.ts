import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

interface ActivityLog {
  id: number;
  action: string;
  userId: string;
  userName: string;
  timestamp: string;
  details: string;
}

@Component({
  selector: 'app-activity-log',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule],
  template: `
    <div class="activity-log-container">
      <h1>Activity Log</h1>
      
      <div class="table-container">
        <table mat-table [dataSource]="activities" class="activities-table">
          <ng-container matColumnDef="timestamp">
            <th mat-header-cell *matHeaderCellDef>Date & Time</th>
            <td mat-cell *matCellDef="let activity">{{formatDate(activity.timestamp)}}</td>
          </ng-container>

          <ng-container matColumnDef="userName">
            <th mat-header-cell *matHeaderCellDef>User</th>
            <td mat-cell *matCellDef="let activity">{{activity.userName}}</td>
          </ng-container>

          <ng-container matColumnDef="action">
            <th mat-header-cell *matHeaderCellDef>Action</th>
            <td mat-cell *matCellDef="let activity">{{activity.action}}</td>
          </ng-container>

          <ng-container matColumnDef="details">
            <th mat-header-cell *matHeaderCellDef>Details</th>
            <td mat-cell *matCellDef="let activity">{{activity.details}}</td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>

        <div class="no-data" *ngIf="activities.length === 0">
          <mat-icon>history</mat-icon>
          <p>No activity logs found</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .activity-log-container {
      padding: 20px;
    }

    .table-container {
      margin-top: 20px;
    }

    .activities-table {
      width: 100%;
    }

    .no-data {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .no-data mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 16px;
    }
  `]
})
export class ActivityLogComponent implements OnInit {
  activities: ActivityLog[] = [];
  displayedColumns: string[] = ['timestamp', 'userName', 'action', 'details'];

  ngOnInit(): void {
    // Mock data for now
    this.activities = [
      {
        id: 1,
        action: 'Patient Created',
        userId: '1',
        userName: 'Admin User',
        timestamp: new Date().toISOString(),
        details: 'Created patient John Doe'
      },
      {
        id: 2,
        action: 'Patient Updated',
        userId: '1',
        userName: 'Admin User',
        timestamp: new Date(Date.now() - 3600000).toISOString(),
        details: 'Updated patient Jane Smith'
      }
    ];
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString();
  }
}
