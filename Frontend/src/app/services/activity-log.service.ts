import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/interfaces';

export interface ActivityLog {
  logID: number;
  userName: string;
  action: string;
  details: string;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class ActivityLogService {
  private readonly API_URL = 'http://localhost:5001/api/activitylog';

  constructor(private http: HttpClient) { }

  getActivityLog(entityType?: string, startDate?: Date, endDate?: Date, pageNumber: number = 1, pageSize: number = 50): Observable<ApiResponse<ActivityLog[]>> {
    let params = new HttpParams();
    
    if (entityType) {
      params = params.set('entityType', entityType);
    }
    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    
    params = params.set('pageNumber', pageNumber.toString());
    params = params.set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<ActivityLog[]>>(this.API_URL, { params });
  }
}
