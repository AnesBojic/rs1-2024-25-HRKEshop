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
    return this.http.get<BrandGetAllResponse[]>(
      `${this.baseUrl}/brands/all`,
      httpOptionsHelper()
    );
  }

  getAllPaged(request: BrandGetAllPagedRequest): Observable<MyPagedList<BrandGetAllPagedResponse>> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.name) {
      params = params.set('name', request.name);
    }

    return this.http.get<MyPagedList<BrandGetAllPagedResponse>>(
      `${this.baseUrl}/brands/filter`,
      {
        params,
        ...httpOptionsHelper(),
      }
    );
  }
}
