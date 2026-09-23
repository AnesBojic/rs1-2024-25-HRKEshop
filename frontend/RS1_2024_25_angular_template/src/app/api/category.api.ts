import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../my-config';
import { httpOptionsHelper } from '../helper/http-options.helper';

export interface CategoryGetAllResponse {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class CategoryApi {
  private readonly baseUrl = `${MyConfig.api_address}`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<CategoryGetAllResponse[]> {
    return this.http.get<CategoryGetAllResponse[]>(`${this.baseUrl}/category/all`, httpOptionsHelper());
  }
}
