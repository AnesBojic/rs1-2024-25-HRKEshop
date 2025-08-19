import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../my-config';
import { httpOptionsHelper } from '../helper/http-options.helper';

export interface BrandGetAllResponse {
  id: number;
  name: string;
  tenantId?: number;
}

export interface BrandGetByIdResponse {
  id: number;
  name: string;
  tenantId?: number;
}

export interface BrandUpdateOrInsertRequest {
  id?: number;
  name: string;
}

export interface BrandGetAllPagedRequest {
  name?: string;
  pageNumber: number;
  pageSize: number;
}

export interface BrandGetAllPagedResponse {
  id: number;
  name: string;
}

export interface MyPagedList<T> {
  dataItems: T[];
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class BrandApi {
  private readonly baseUrl = `${MyConfig.api_address}`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BrandGetAllResponse[]> {
    return this.http.get<BrandGetAllResponse[]>(`${this.baseUrl}/brands/all`, httpOptionsHelper());
  }

  getById(id: number): Observable<BrandGetByIdResponse> {
    return this.http.get<BrandGetByIdResponse>(`${this.baseUrl}/brands/${id}`, httpOptionsHelper());
  }

  updateOrInsert(data: BrandUpdateOrInsertRequest): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/brands/UpdateOrInsert`, data, httpOptionsHelper());
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/brands/delete/${id}`, httpOptionsHelper());
  }
}
