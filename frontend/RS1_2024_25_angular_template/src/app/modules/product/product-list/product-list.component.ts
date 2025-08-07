import { Component, OnInit } from '@angular/core';
//import { ProductsApi } from 'src/app/api/products.api';
import { ProductsApi } from '../../../api/product.api';

/*
import {
  ProductGetAll3Request,
  ProductGetAll3Response,
  MyPagedList
} from 'src/app/dto/product.dto';

 */

import {
  ProductGetAll3Request,
  ProductGetAll3Response,
  MyPagedList
} from '../../../dto/product.dto';


@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html'
})
export class ProductListComponent implements OnInit {

  pagedData?: MyPagedList<ProductGetAll3Response>;

  filter: ProductGetAll3Request = {
    pageNumber: 1,
    pageSize: 5,
    q: '',
    gender: undefined,
    minPrice: undefined,
    maxPrice: undefined,
    colorId: undefined,
    brandId: undefined
  };

  constructor(private productsApi: ProductsApi) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.productsApi.filter(this.filter).subscribe(result => {
      this.pagedData = result;
    });
  }

  goToPage(page: number): void {
    this.filter.pageNumber = page;
    this.loadData();
  }

  next(): void {
    if (this.pagedData?.hasNext) {
      this.filter.pageNumber++;
      this.loadData();
    }
  }

  prev(): void {
    if (this.pagedData?.hasPrevious) {
      this.filter.pageNumber--;
      this.loadData();
    }
  }
}
