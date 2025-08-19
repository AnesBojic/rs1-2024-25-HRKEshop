import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import { ProductGetAll3Response, MyPagedList, ProductGetAll3Request } from '../../../dto/product.dto';
import { BrandApi, BrandGetAllResponse } from '../../../api/brand.api';
import { ColorApi, ColorGetAllResponse } from '../../../api/color.api';

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


  loadProducts(page: number): void {
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

    this.productsApi.filter(request)
      .subscribe((res: MyPagedList<ProductGetAll3Response>) => {
        this.products = res.dataItems;
        this.currentPage = res.currentPage;
        this.totalPages = res.totalPages;
      });
  }

  // 🔹 Loading brendova
  loadBrands(): void {
    this.brandsApi.getAll().subscribe((res: BrandGetAllResponse[]) => {
      this.brands = res;
    });
  }

  // 🔹
  loadColors(): void {
    this.colorsApi.getAll().subscribe((res: ColorGetAllResponse[]) => {
      this.colors = res;
    });
  }

  getProductImageUrl(product: ProductGetAll3Response): string {
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
        this.loadProducts(this.currentPage);
      });
    }
  }
}
