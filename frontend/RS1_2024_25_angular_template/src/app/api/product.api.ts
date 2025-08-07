import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../my-config';
import {
  ProductGetAll1Response,
  ProductGetAll3Request,
  ProductGetAll3Response,
  ProductGetByIdResponse,
  ProductUpdateOrInsertRequest,
  MyPagedList

} from '../dto/product.dto'; // Ako su svi DTO-i u jednom fajlu
import { httpOptionsHelper } from '../helper/http-options.helper';


@Injectable({
  providedIn: 'root'
})
export class ProductsApi {

  private readonly baseUrl = `${MyConfig.api_address}`;

  constructor(private http: HttpClient) {}

  // Dohvati sve proizvode (bez filtera)
  getAll(): Observable<ProductGetAll1Response[]> {
    return this.http.get<ProductGetAll1Response[]>(
      `${this.baseUrl}/products/all`,
      httpOptionsHelper()
    );
  }

  // Dohvati proizvode s filterima i paginacijom
  filter(request: ProductGetAll3Request): Observable<MyPagedList<ProductGetAll3Response>> {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize);

    if (request.q) params = params.set('q', request.q);
    if (request.gender !== undefined) params = params.set('gender', request.gender);
    if (request.minPrice !== undefined) params = params.set('minPrice', request.minPrice);
    if (request.maxPrice !== undefined) params = params.set('maxPrice', request.maxPrice);
    if (request.colorId !== undefined) params = params.set('colorId', request.colorId);
    if (request.brandId !== undefined) params = params.set('brandId', request.brandId);

    return this.http.get<MyPagedList<ProductGetAll3Response>>(
      `${this.baseUrl}/product/filter`,
      {
        params,
        ...httpOptionsHelper()
      }
    );
  }

  // Dohvati jedan proizvod po ID-u
  getById(id: number): Observable<ProductGetByIdResponse> {
    return this.http.get<ProductGetByIdResponse>(
      `${this.baseUrl}/product/${id}`,
      httpOptionsHelper()
    );
  }

  // Dodaj ili ažuriraj proizvod
  updateOrInsert(data: ProductUpdateOrInsertRequest): Observable<number> {
    return this.http.post<number>(
      `${this.baseUrl}/products/UpdateOrInsert`,
      data,
      httpOptionsHelper()
    );
  }

  // Obriši proizvod po ID-u
  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/products/delete/${id}`,
      httpOptionsHelper()
    );
  }
}
