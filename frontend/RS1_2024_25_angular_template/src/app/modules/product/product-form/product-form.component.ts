import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import { BrandApi } from '../../../api/brand.api';
import { ColorApi } from '../../../api/color.api';
import {
  ProductUpdateOrInsertRequest,
  ProductGetByIdResponse,
  Gender
} from '../../../dto/product.dto';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  standalone: false,
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {

  form!: FormGroup;
  isEditMode: boolean = false;
  productId?: number;

  brands: { id: number; name: string }[] = [];
  colors: { id: number; name: string }[] = [];

  // Gender enum options
  genders = [
    { id: Gender.Male, name: 'Muški' },
    { id: Gender.Female, name: 'Ženski' },
    { id: Gender.Other, name: 'Ostalo' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productsApi: ProductsApi,
    private brandApi: BrandApi,
    private colorApi: ColorApi
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      gender: [Gender.Male, Validators.required],
      colorId: [null, Validators.required],
      brandId: [null, Validators.required]
    });

    this.loadBrands();
    this.loadColors();

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.productId = +params['id'];
        this.loadProduct(this.productId);
      }
    });
  }

  loadBrands() {
    this.brandApi.getAll().subscribe(brands => {
      this.brands = brands.map(b => ({ id: b.id, name: b.name }));
    });
  }

  loadColors() {
    this.colorApi.getAll().subscribe(colors => {
      this.colors = colors.map(c => ({ id: c.id, name: c.name }));
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
      alert('Proizvod je uspješno sačuvan.');
      this.router.navigate(['/products']);
    });
  }
}
