import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import { ProductGetAll3Response, MyPagedList } from '../../../dto/product.dto';

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

  constructor(
    private productsApi: ProductsApi,
    public router: Router,
  ) {}

  ngOnInit(): void {
    this.loadProducts(this.currentPage);
  }

  loadProducts(page: number): void {
    this.productsApi.getAllPaged(page, this.pageSize).subscribe((res: MyPagedList<ProductGetAll3Response>) => {
      this.products = res.dataItems;
      this.currentPage = res.currentPage;
      this.totalPages = res.totalPages;
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
    if (confirm('Da li ste sigurni da želite obrisati ovaj proizvod?')) {
      this.productsApi.delete(id).subscribe(() => {
        this.loadProducts(this.currentPage);
      });
    }
  }
}
