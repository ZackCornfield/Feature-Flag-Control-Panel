import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface FeatureFlagDto {
  id: number;
  key: string;
  isEnabled: boolean;
  environment: string;
}

export interface FeatureFlagOverrideDto {
  id: number;
  featureFlagId: number;
  userId: string;
  isEnabled: boolean;
}

export interface FeatureFlagRequestDto {
  key: string;
  isEnabled: boolean;
  environment: string;
}

export interface FeatureFlagToggleRequestDto {
  isEnabled: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class FeatureFlagService {
  constructor(private http: HttpClient) {}

  private readonly API_URL = environment.API_URL + '/featureflag';

  getAllFeatureFlags(): Observable<FeatureFlagDto[]> {
    return this.http.get<FeatureFlagDto[]>(`${this.API_URL}/`);
  }

  getFeatureFlagById(id: number): Observable<FeatureFlagDto> {
    return this.http.get<FeatureFlagDto>(`${this.API_URL}/${id}`);
  }

  createFeatureFlag(request: FeatureFlagRequestDto): Observable<FeatureFlagDto> {
    return this.http.post<FeatureFlagDto>(`${this.API_URL}/`, request);
  }

  updateFeatureFlag(id: number, request: FeatureFlagRequestDto): Observable<FeatureFlagDto> {
    return this.http.put<FeatureFlagDto>(`${this.API_URL}/${id}`, request);
  }

  deleteFeatureFlag(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.API_URL}/${id}`);
  }

  toggleFeatureFlag(id: number, request: FeatureFlagToggleRequestDto): Observable<boolean> {
    return this.http.patch<boolean>(`${this.API_URL}/${id}/toggle`, request);
  }

  getFeatureFlagOverrides(): Observable<FeatureFlagOverrideDto[]> {
    return this.http.get<FeatureFlagOverrideDto[]>(`${this.API_URL}/override`);
  }

  addFeatureFlagOverrideForUser(
    id: number,
    userId: string,
    request: FeatureFlagToggleRequestDto,
  ): Observable<FeatureFlagOverrideDto> {
    return this.http.post<FeatureFlagOverrideDto>(
      `${this.API_URL}/${id}/override/${userId}`,
      request,
    );
  }

  removeFeatureFlagOverrideForUser(id: number, userId: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.API_URL}/${id}/override/${userId}`);
  }

  toggleFeatureFlagOverrideForUser(
    id: number,
    userId: string,
    request: FeatureFlagToggleRequestDto,
  ): Observable<boolean> {
    return this.http.patch<boolean>(`${this.API_URL}/${id}/override/${userId}/toggle`, request);
  }

  evaluateFeatureFlag(key: string, userId: string, environment: string): Observable<boolean> {
    return this.http.get<boolean>(
      `${this.API_URL}/evaluate?key=${key}&userId=${userId}&environment=${environment}`,
    );
  }

  evaluateAllFeatureFlagsForUser(
    userId: string,
    environment: string,
  ): Observable<Record<string, boolean>> {
    return this.http.get<Record<string, boolean>>(
      `${this.API_URL}/evaluate/all?userId=${userId}&environment=${environment}`,
    );
  }
}
