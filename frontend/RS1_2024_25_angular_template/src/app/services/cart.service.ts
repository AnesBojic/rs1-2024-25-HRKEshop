import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { CartApi } from '../api/cart.api';
import { AuthService } from './auth-services/auth.service';
import {
  CartCheckoutRequest,
  CartCheckoutResponse,
  CartItemDto,
  CartResponse
} from '../dto/cart.dto';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly storageKey = 'hrkeshop_cart';
  private readonly itemsSubject = new BehaviorSubject<CartItemDto[]>([]);
  private readonly totalQuantitySubject = new BehaviorSubject<number>(0);
  private readonly totalAmountSubject = new BehaviorSubject<number>(0);

  readonly items$ = this.itemsSubject.asObservable();
  readonly totalQuantity$ = this.totalQuantitySubject.asObservable();
  readonly totalAmount$ = this.totalAmountSubject.asObservable();

  constructor(
    private cartApi: CartApi,
    private authService: AuthService
  ) {
    this.authService.loggedIn$.subscribe((isLoggedIn) => {
      if (isLoggedIn) {
        this.mergeLocalCartIntoServer().subscribe({
          next: () => this.refreshFromServer().subscribe(),
          error: () => this.refreshFromServer().subscribe()
        });
      } else {
        const current = this.itemsSubject.value;
        localStorage.setItem(this.storageKey, JSON.stringify(current));
        this.loadFromLocalStorage();
      }
    });
  }

  getSnapshot(): CartItemDto[] {
    return this.itemsSubject.value;
  }

  refresh(): Observable<CartResponse> {
    if (this.authService.isLoggedIn()) {
      return this.refreshFromServer();
    }

    this.loadFromLocalStorage();
    return of(this.toResponse(this.itemsSubject.value));
  }

  addItem(item: Omit<CartItemDto, 'lineTotal'> & { lineTotal?: number }): Observable<CartResponse> {
    if (this.authService.isLoggedIn()) {
      return this.cartApi.addItem({
        productSizeId: item.productSizeId,
        quantity: item.quantity
      }).pipe(tap((response) => this.applyResponse(response)));
    }

    const items = [...this.itemsSubject.value];
    const existing = items.find((i) => i.productSizeId === item.productSizeId);
    const nextQty = (existing?.quantity ?? 0) + item.quantity;

    if (nextQty > item.stock) {
      throw new Error(`Not enough stock. Available: ${item.stock}`);
    }

    if (existing) {
      existing.quantity = nextQty;
      existing.unitPrice = item.unitPrice;
      existing.stock = item.stock;
      existing.productName = item.productName;
      existing.sizeName = item.sizeName;
      existing.imageUrl = item.imageUrl;
      existing.lineTotal = existing.quantity * existing.unitPrice;
    } else {
      items.push({
        ...item,
        lineTotal: item.quantity * item.unitPrice
      });
    }

    this.persistLocal(items);
    return of(this.toResponse(items));
  }

  setQuantity(productSizeId: number, quantity: number): Observable<CartResponse> {
    if (this.authService.isLoggedIn()) {
      return this.cartApi.setQuantity({ productSizeId, quantity })
        .pipe(tap((response) => this.applyResponse(response)));
    }

    let items = [...this.itemsSubject.value];
    const existing = items.find((i) => i.productSizeId === productSizeId);
    if (!existing) {
      return of(this.toResponse(items));
    }

    if (quantity <= 0) {
      items = items.filter((i) => i.productSizeId !== productSizeId);
    } else {
      if (quantity > existing.stock) {
        throw new Error(`Not enough stock. Available: ${existing.stock}`);
      }
      existing.quantity = quantity;
      existing.lineTotal = existing.quantity * existing.unitPrice;
    }

    this.persistLocal(items);
    return of(this.toResponse(items));
  }

  removeItem(productSizeId: number): Observable<CartResponse> {
    if (this.authService.isLoggedIn()) {
      return this.cartApi.removeItem(productSizeId)
        .pipe(tap((response) => this.applyResponse(response)));
    }

    const items = this.itemsSubject.value.filter((i) => i.productSizeId !== productSizeId);
    this.persistLocal(items);
    return of(this.toResponse(items));
  }

  clear(): Observable<CartResponse> {
    if (this.authService.isLoggedIn()) {
      return this.cartApi.clear().pipe(tap((response) => this.applyResponse(response)));
    }

    this.persistLocal([]);
    return of(this.toResponse([]));
  }

  checkout(request: CartCheckoutRequest): Observable<CartCheckoutResponse> {
    return this.cartApi.checkout(request).pipe(
      tap(() => this.applyResponse(this.toResponse([])))
    );
  }

  private mergeLocalCartIntoServer(): Observable<CartResponse | null> {
    const localItems = this.readLocalStorage();
    if (!localItems.length) {
      return of(null);
    }

    return this.cartApi.merge({
      items: localItems.map((i) => ({
        productSizeId: i.productSizeId,
        quantity: i.quantity
      }))
    }).pipe(
      tap((response) => {
        localStorage.removeItem(this.storageKey);
        this.applyResponse(response);
      }),
      catchError(() => of(null))
    );
  }

  private refreshFromServer(): Observable<CartResponse> {
    return this.cartApi.getCart().pipe(
      tap((response) => this.applyResponse(response)),
      catchError(() => {
        this.applyResponse(this.toResponse([]));
        return of(this.toResponse([]));
      })
    );
  }

  private loadFromLocalStorage(): void {
    this.persistLocal(this.readLocalStorage(), false);
  }

  private readLocalStorage(): CartItemDto[] {
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (!raw) {
        return [];
      }
      const parsed = JSON.parse(raw) as CartItemDto[];
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  private persistLocal(items: CartItemDto[], writeStorage: boolean = true): void {
    const normalized = items.map((item) => ({
      ...item,
      lineTotal: item.quantity * item.unitPrice
    }));

    if (writeStorage) {
      localStorage.setItem(this.storageKey, JSON.stringify(normalized));
    }

    this.applyResponse(this.toResponse(normalized));
  }

  private applyResponse(response: CartResponse): void {
    this.itemsSubject.next(response.items ?? []);
    this.totalQuantitySubject.next(response.totalQuantity ?? 0);
    this.totalAmountSubject.next(response.totalAmount ?? 0);
  }

  private toResponse(items: CartItemDto[]): CartResponse {
    return {
      items,
      totalQuantity: items.reduce((sum, item) => sum + item.quantity, 0),
      totalAmount: items.reduce((sum, item) => sum + item.quantity * item.unitPrice, 0)
    };
  }
}
