// src/app/api/color.api.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../my-config';
import { httpOptionsHelper } from '../helper/http-options.helper';

export interface ColorGetAllResponse {
  id: number;
  name: string;
  hex_Code: string;
}

export interface ColorUpdateOrInsertRequest {
  id?: number;
  name: string;
  hex_Code: string;
}

@Injectable({
  providedIn: 'root'
})
export class ColorApi {
  private readonly baseUrl = `${MyConfig.api_address}`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ColorGetAllResponse[]> {
    return this.http.get<ColorGetAllResponse[]>(
      `${this.baseUrl}/colors/all`,
      httpOptionsHelper()
    );
  }

  getById(id: number): Observable<ColorGetAllResponse> {
    return this.http.get<ColorGetAllResponse>(
      `${this.baseUrl}/color/${id}`,
      httpOptionsHelper()
    );
  }

  updateOrInsert(data: ColorUpdateOrInsertRequest): Observable<number> {
    return this.http.post<number>(
      `${this.baseUrl}/colors/updateOrInsert`,
      data,
      httpOptionsHelper()
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/colors/${id}`,
      httpOptionsHelper()
    );
  }
}
