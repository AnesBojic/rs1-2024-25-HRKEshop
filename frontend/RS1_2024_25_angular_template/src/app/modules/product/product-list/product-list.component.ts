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

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  standalone: false,
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {

  // --- VARIJABLE ---
  products: ProductGetAll3Response[] = [];

  // Paginacija
  currentPage = 1;
  totalPages = 1;
  pageSize = 6;

  // Statusi
  isInfiniteScroll = false;
  isLoading = false;

  // Filteri
  filters: Partial<ProductGetAll3Request> = {
    q: '',
    gender: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    brandId: undefined,
    colorId: undefined
  };

  // Podaci za dropdown menije
  brands: BrandGetAllResponse[] = [];
  colors: ColorGetAllResponse[] = [];

  constructor(
    private productsApi: ProductsApi,
    private brandsApi: BrandApi,
    private colorsApi: ColorApi,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.loadProducts(this.currentPage);
    this.loadBrands();
    this.loadColors();
  }

  // --- GLAVNA LOGIKA ---

  toggleScrollMode(): void {
    this.isInfiniteScroll = !this.isInfiniteScroll;

    // Reset svega
    this.products = [];
    this.currentPage = 1;
    this.totalPages = 1;

    if (this.isInfiniteScroll) {
      this.pageSize = 15; // Bigger number for infinite scroll
    } else {
      this.pageSize = 6;  // Smaller number for paging
    }

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
          // APPEND (dodaj na kraj)
          this.products = [...this.products, ...res.dataItems];
        } else {
          // REPLACE (zamijeni sve)
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
        console.log("Učitavam novu stranicu (Div Scroll)...");
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

  getProductImageUrl(product: any): string {
    return `https://via.placeholder.com/200x150?text=${encodeURIComponent(product.name)}`;
  }

  onImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    if (target) {
      target.src = 'https://via.placeholder.com/200x150?text=No+Image';
    }
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
