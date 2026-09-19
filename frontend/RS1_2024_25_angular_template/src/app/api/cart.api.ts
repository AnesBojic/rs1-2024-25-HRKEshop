import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MyConfig } from '../my-config';
import {
  CartAddRequest,
  CartCheckoutRequest,
  CartCheckoutResponse,
  CartMergeRequest,
  CartResponse,
  CartSetQuantityRequest,
  ProductSizeOption
} from '../dto/cart.dto';

@Injectable({
  providedIn: 'root'
})
export class CartApi {
  private readonly baseUrl = MyConfig.api_address;

  constructor(private http: HttpClient) {}

  getCart(): Observable<CartResponse> {
    return this.http.get<CartResponse>(`${this.baseUrl}/cart`);
  }

  addItem(request: CartAddRequest): Observable<CartResponse> {
    return this.http.post<CartResponse>(`${this.baseUrl}/cart/items`, request);
  }

  setQuantity(request: CartSetQuantityRequest): Observable<CartResponse> {
    return this.http.put<CartResponse>(`${this.baseUrl}/cart/items/set-quantity`, request);
  }

  removeItem(productSizeId: number): Observable<CartResponse> {
    return this.http.delete<CartResponse>(`${this.baseUrl}/cart/items/${productSizeId}`);
  }

  clear(): Observable<CartResponse> {
    return this.http.delete<CartResponse>(`${this.baseUrl}/cart`);
  }

  merge(request: CartMergeRequest): Observable<CartResponse> {
    return this.http.post<CartResponse>(`${this.baseUrl}/cart/merge`, request);
  }

  checkout(request: CartCheckoutRequest): Observable<CartCheckoutResponse> {
    return this.http.post<CartCheckoutResponse>(`${this.baseUrl}/cart/checkout`, request);
  }

  getProductSizes(productId: number): Observable<ProductSizeOption[]> {
    return this.http.get<ProductSizeOption[]>(`${this.baseUrl}/products/${productId}/sizes`);
  }
}
