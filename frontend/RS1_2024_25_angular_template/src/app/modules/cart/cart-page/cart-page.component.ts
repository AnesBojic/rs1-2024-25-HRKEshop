import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../../services/cart.service';
import { AuthService } from '../../../services/auth-services/auth.service';
import { CartCheckoutResponse, CartItemDto } from '../../../dto/cart.dto';
import { MyConfig } from '../../../my-config';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.css'],
  standalone: false
})
export class CartPageComponent implements OnInit {
  items: CartItemDto[] = [];
  totalQuantity = 0;
  totalAmount = 0;
  shippingAddress = '';
  isLoading = false;
  isCheckingOut = false;
  errorMessage = '';
  successMessage = '';
  isLoggedIn = false;

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
    this.authService.loggedIn$.subscribe((status: boolean) => this.isLoggedIn = status);

    this.isLoading = true;
    this.cartService.refresh().subscribe({
      next: () => {
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });

    this.cartService.items$.subscribe((items: CartItemDto[]) => this.items = items);
    this.cartService.totalQuantity$.subscribe((qty: number) => this.totalQuantity = qty);
    this.cartService.totalAmount$.subscribe((amount: number) => this.totalAmount = amount);
  }

  getImageUrl(item: CartItemDto): string {
    if (!item.imageUrl) {
      return `https://via.placeholder.com/120x90?text=${encodeURIComponent(item.productName)}`;
    }
    if (/^(https?:|data:|blob:)/i.test(item.imageUrl)) {
      return item.imageUrl;
    }
    const normalized = item.imageUrl.startsWith('/') ? item.imageUrl : `/${item.imageUrl}`;
    return `${MyConfig.api_address}${normalized}`;
  }

  updateQuantity(item: CartItemDto, quantity: number): void {
    this.errorMessage = '';
    try {
      this.cartService.setQuantity(item.productSizeId, Number(quantity)).subscribe({
        error: (err: any) => {
          this.errorMessage = err?.error || err?.message || 'Could not update quantity.';
        }
      });
    } catch (err: any) {
      this.errorMessage = err?.message || 'Could not update quantity.';
    }
  }

  removeItem(item: CartItemDto): void {
    this.cartService.removeItem(item.productSizeId).subscribe();
  }

  clearCart(): void {
    this.cartService.clear().subscribe();
  }

  checkout(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.items.length) {
      this.errorMessage = 'Your cart is empty.';
      return;
    }

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: '/cart' } });
      return;
    }

    if (!this.authService.isCustomer()) {
      this.errorMessage = 'Only customer accounts can place orders.';
      return;
    }

    if (!this.shippingAddress.trim()) {
      this.errorMessage = 'Please enter a shipping address.';
      return;
    }

    this.isCheckingOut = true;
    this.cartService.checkout({ shippingAddress: this.shippingAddress.trim() }).subscribe({
      next: (res: CartCheckoutResponse) => {
        this.isCheckingOut = false;
        this.successMessage = res.message;
        this.shippingAddress = '';
      },
      error: (err: any) => {
        this.isCheckingOut = false;
        this.errorMessage = typeof err?.error === 'string'
          ? err.error
          : (err?.error?.message || 'Checkout failed.');
      }
    });
  }
}
