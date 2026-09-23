import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import { Gender, ProductDetailsResponse, ProductDetailsSize } from '../../../dto/product.dto';
import { CartService } from '../../../services/cart.service';
import { resolveApiAssetUrl } from '../../../helper/resolve-api-asset-url';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css'],
  standalone: false
})
export class ProductDetailComponent implements OnInit {
  product: ProductDetailsResponse | null = null;
  selectedSizeId: number | null = null;
  loading = true;
  message = '';
  notFound = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productsApi: ProductsApi,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (!id) {
        this.notFound = true;
        this.loading = false;
        return;
      }
      this.load(id);
    });
  }

  load(id: number): void {
    this.loading = true;
    this.notFound = false;
    this.message = '';
    this.productsApi.getDetails(id).subscribe({
      next: (product) => {
        this.product = product;
        const available = product.sizes.find((size) => size.stock > 0) ?? product.sizes[0];
        this.selectedSizeId = available?.productSizeId ?? null;
        this.loading = false;
      },
      error: () => {
        this.product = null;
        this.notFound = true;
        this.loading = false;
      }
    });
  }

  imageUrl(path?: string | null): string {
    return resolveApiAssetUrl(path, 'https://via.placeholder.com/320x240?text=No+Image');
  }

  genderLabel(gender: Gender): string {
    if (gender === Gender.Male) return 'PRODUCT.MALE';
    if (gender === Gender.Female) return 'PRODUCT.FEMALE';
    return 'PRODUCT.OTHER';
  }

  openVariant(id: number): void {
    if (id === this.product?.id) {
      return;
    }
    this.router.navigate(['/products', id]);
  }

  selectedSize(): ProductDetailsSize | undefined {
    return this.product?.sizes.find((size) => size.productSizeId === this.selectedSizeId);
  }

  addToCart(): void {
    if (!this.product) {
      return;
    }

    const size = this.selectedSize();
    if (!size) {
      this.message = 'Select a size first.';
      return;
    }
    if (size.stock <= 0) {
      this.message = 'Selected size is out of stock.';
      return;
    }

    this.cartService.addItem({
      productId: this.product.id,
      productSizeId: size.productSizeId,
      productName: this.product.name,
      sizeName: size.sizeName,
      quantity: 1,
      unitPrice: Number(size.price),
      stock: size.stock,
      imageUrl: this.product.imageUrl
    }).subscribe({
      next: () => {
        this.message = `${this.product?.name} (${size.sizeName}) added to cart.`;
      },
      error: (err) => {
        this.message = typeof err?.error === 'string'
          ? err.error
          : (err?.message || 'Could not add to cart.');
      }
    });
  }

  back(): void {
    this.router.navigate(['/products']);
  }
}
