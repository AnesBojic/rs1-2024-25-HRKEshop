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
} from '../dto/product.dto';
import { httpOptionsHelper } from '../helper/http-options.helper';

@Injectable({
  providedIn: 'root'
})
export class ProductsApi {

  private readonly baseUrl = `${MyConfig.api_address}`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ProductGetAll1Response[]> {
    return this.http.get<ProductGetAll1Response[]>(
      `${this.baseUrl}/products/all`,
      httpOptionsHelper()
    );
  }

  getAllPaged(pageNumber: number = 1, pageSize: number = 10) {
    return this.http.get<MyPagedList<ProductGetAll3Response>>(
      `${this.baseUrl}/product/filter?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      httpOptionsHelper()
    );
  }

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

  getById(id: number): Observable<ProductGetByIdResponse> {
    return this.http.get<ProductGetByIdResponse>(
      `${this.baseUrl}/product/${id}`,
      httpOptionsHelper()
    );
  }

  updateOrInsert(data: ProductUpdateOrInsertRequest): Observable<number> {
    return this.http.post<number>(
      `${this.baseUrl}/products/UpdateOrInsert`,
      data,
      httpOptionsHelper()
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.baseUrl}/products/delete/${id}`,
      httpOptionsHelper()
    );
  }
}
