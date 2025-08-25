import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Patient, CreatePatientRequest, UpdatePatientRequest, ApiResponse, PagedResult } from '../models/interfaces';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private readonly API_URL = 'http://localhost:5001/api/patients';

  constructor(private http: HttpClient) { }

  getPatients(): Observable<ApiResponse<Patient[]>> {
    return this.http.get<ApiResponse<PagedResult<Patient>>>(this.API_URL)
      .pipe(
        map(response => ({
          ...response,
          data: response.data?.items || []
        }))
      );
  }

  getPatient(id: number): Observable<ApiResponse<Patient>> {
    return this.http.get<ApiResponse<Patient>>(`${this.API_URL}/${id}`);
  }

  createPatient(patient: CreatePatientRequest): Observable<ApiResponse<Patient>> {
    return this.http.post<ApiResponse<Patient>>(this.API_URL, patient);
  }

  updatePatient(patient: UpdatePatientRequest): Observable<ApiResponse<Patient>> {
    return this.http.put<ApiResponse<Patient>>(`${this.API_URL}/${patient.id}`, patient);
  }

  deletePatient(id: number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.API_URL}/${id}`);
  }

  searchPatients(searchTerm: string): Observable<ApiResponse<Patient[]>> {
    const params = new HttpParams().set('searchTerm', searchTerm);
    return this.http.get<ApiResponse<PagedResult<Patient>>>(this.API_URL, { params })
      .pipe(
        map(response => ({
          ...response,
          data: response.data?.items || []
        }))
      );
  }
}
