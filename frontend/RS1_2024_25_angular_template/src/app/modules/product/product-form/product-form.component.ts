import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import {
  ProductUpdateOrInsertRequest,
  ProductGetByIdResponse,
  Gender
} from '../../../dto/product.dto';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {

  form!: FormGroup;
  isEditMode: boolean = false;
  productId?: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productsApi: ProductsApi
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      gender: [0, Validators.required],
      colorId: [null, Validators.required],
      brandId: [null, Validators.required]
    });

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.productId = +params['id'];
        this.loadProduct(this.productId);
      }
    });
  }

  loadProduct(id: number): void {
    this.productsApi.getById(id).subscribe((data: ProductGetByIdResponse) => {
      this.form.patchValue({
        name: data.name,
        price: data.price,
        gender: data.gender,
        colorId: data.colorId,
        brandId: data.brandId
      });
    });
  }

  submit(): void {
    if (this.form.invalid) return;

    const request: ProductUpdateOrInsertRequest = {
      ...this.form.value,
      id: this.isEditMode ? this.productId : undefined
    };

    this.productsApi.updateOrInsert(request).subscribe(() => {
      alert('Product has been updated successfully.!');
      this.router.navigate(['/products']);
    });
  }
}
