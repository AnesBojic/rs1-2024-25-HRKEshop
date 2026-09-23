import { Component, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import {
  ProductGetAll3Response,
  MyPagedList,
  ProductGetAll3Request
} from '../../../dto/product.dto';
import { BrandApi, BrandGetAllResponse } from '../../../api/brand.api';
import { ColorApi, ColorGetAllResponse } from '../../../api/color.api';
import { CartApi } from '../../../api/cart.api';
import { CartService } from '../../../services/cart.service';
import { AuthService } from '../../../services/auth-services/auth.service';
import { ProductSizeOption } from '../../../dto/cart.dto';
import { resolveApiAssetUrl } from '../../../helper/resolve-api-asset-url';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  standalone: false,
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {

  products: ProductGetAll3Response[] = [];

  currentPage = 1;
  totalPages = 1;
  pageSize = 6;

  isInfiniteScroll = false;
  isLoading = false;

  filters: Partial<ProductGetAll3Request> = {
    q: '',
    gender: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    brandId: undefined,
    colorId: undefined
  };

  brands: BrandGetAllResponse[] = [];
  colors: ColorGetAllResponse[] = [];

  sizesByProduct: Record<number, ProductSizeOption[]> = {};
  selectedSizeByProduct: Record<number, number | null> = {};
  loadingSizes: Record<number, boolean> = {};
  cartMessage = '';
  canManageProducts = false;

  constructor(
    private productsApi: ProductsApi,
    private brandsApi: BrandApi,
    private colorsApi: ColorApi,
    private cartApi: CartApi,
    private cartService: CartService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.canManageProducts = this.authService.isAdmin() || this.authService.isManager();
    this.loadProducts(this.currentPage);
    this.loadBrands();
    this.loadColors();
  }

  toggleScrollMode(): void {
    this.isInfiniteScroll = !this.isInfiniteScroll;
    this.products = [];
    this.currentPage = 1;
    this.totalPages = 1;
    this.pageSize = this.isInfiniteScroll ? 15 : 6;
    this.loadProducts(1);
  }

  loadProducts(page: number): void {
    if (this.isLoading) return;
    this.isLoading = true;

    const request: ProductGetAll3Request = {
      q: this.filters.q || undefined,
      gender: this.filters.gender,
      minPrice: this.filters.minPrice,
      maxPrice: this.filters.maxPrice,
      brandId: this.filters.brandId,
      colorId: this.filters.colorId,
      pageNumber: page,
      pageSize: this.pageSize
    };

    this.productsApi.filter(request).subscribe({
      next: (res: MyPagedList<ProductGetAll3Response>) => {
        if (this.isInfiniteScroll && page > 1) {
          this.products = [...this.products, ...res.dataItems];
        } else {
          this.products = res.dataItems;
        }

        this.currentPage = res.currentPage;
        this.totalPages = res.totalPages;
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  onDivScroll(event: any) {
    if (!this.isInfiniteScroll || this.isLoading) return;
    const element = event.target;
    if (element.scrollHeight - element.scrollTop <= element.clientHeight + 50) {
      if (this.currentPage < this.totalPages) {
        this.loadProducts(this.currentPage + 1);
      }
    }
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
    if (!this.isInfiniteScroll || this.isLoading) return;
    const distanceFromBottom = document.documentElement.scrollHeight - (window.innerHeight + window.scrollY);
    if (distanceFromBottom <= 200 && this.currentPage < this.totalPages) {
      this.loadProducts(this.currentPage + 1);
    }
  }

  loadBrands(): void {
    this.brandsApi.getAll().subscribe((res: BrandGetAllResponse[]) => {
      this.brands = res;
    });
  }

  loadColors(): void {
    this.colorsApi.getAll().subscribe((res: ColorGetAllResponse[]) => {
      this.colors = res;
    });
  }

  ensureSizesLoaded(productId: number): void {
    if (this.sizesByProduct[productId] || this.loadingSizes[productId]) {
      return;
    }

    this.loadingSizes[productId] = true;
    this.cartApi.getProductSizes(productId).subscribe({
      next: (sizes) => {
        this.sizesByProduct[productId] = sizes;
        const firstAvailable = sizes.find((s) => s.stock > 0) ?? sizes[0];
        this.selectedSizeByProduct[productId] = firstAvailable?.productSizeId ?? null;
        this.loadingSizes[productId] = false;
      },
      error: () => {
        this.sizesByProduct[productId] = [];
        this.loadingSizes[productId] = false;
      }
    });
  }

  addToCart(product: ProductGetAll3Response): void {
    this.cartMessage = '';
    this.ensureSizesLoaded(product.id);

    const sizes = this.sizesByProduct[product.id];
    if (!sizes) {
      this.cartApi.getProductSizes(product.id).subscribe({
        next: (loaded) => {
          this.sizesByProduct[product.id] = loaded;
          const firstAvailable = loaded.find((s) => s.stock > 0) ?? loaded[0];
          this.selectedSizeByProduct[product.id] = firstAvailable?.productSizeId ?? null;
          this.addSelectedSizeToCart(product);
        },
        error: () => {
          this.cartMessage = 'No sizes available for this product.';
        }
      });
      return;
    }

    this.addSelectedSizeToCart(product);
  }

  private addSelectedSizeToCart(product: ProductGetAll3Response): void {
    const selectedId = this.selectedSizeByProduct[product.id];
    const size = this.sizesByProduct[product.id]?.find((s) => s.productSizeId === selectedId);

    if (!size) {
      this.cartMessage = 'Select a size first.';
      return;
    }

    if (size.stock <= 0) {
      this.cartMessage = 'Selected size is out of stock.';
      return;
    }

    try {
      this.cartService.addItem({
        productId: product.id,
        productSizeId: size.productSizeId,
        productName: product.name,
        sizeName: size.sizeName,
        quantity: 1,
        unitPrice: Number(size.priceForItem),
        stock: size.stock,
        imageUrl: product.imageUrl
      }).subscribe({
        next: () => {
          this.cartMessage = `${product.name} (${size.sizeName}) added to cart.`;
        },
        error: (err) => {
          this.cartMessage = typeof err?.error === 'string'
            ? err.error
            : (err?.message || 'Could not add to cart.');
        }
      });
    } catch (err: any) {
      this.cartMessage = err?.message || 'Could not add to cart.';
    }
  }

  getProductImageUrl(product: ProductGetAll3Response): string {
    return resolveApiAssetUrl(
      product.imageUrl,
      `https://via.placeholder.com/200x150?text=${encodeURIComponent(product.name)}`
    );
  }

  onImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.src = 'https://via.placeholder.com/200x150?text=No+Image';
    }
  }

  openProduct(id: number) {
    this.router.navigate(['/products', id]);
  }

  editProduct(id: number) {
    this.router.navigate(['/products/edit', id]);
  }

  goToNewProduct() {
    this.router.navigate(['/products/new']);
  }

  deleteProduct(id: number) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productsApi.delete(id).subscribe(() => {
        this.currentPage = 1;
        this.loadProducts(1);
      });
    }
  }
}
