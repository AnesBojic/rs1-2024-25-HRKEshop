import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductsApi } from '../../../api/product.api';
import { BrandApi } from '../../../api/brand.api';
import { ColorApi } from '../../../api/color.api';
import { CategoryApi } from '../../../api/category.api';
import { ImageApi } from '../../../api/image.api';
import {
  ProductUpdateOrInsertRequest,
  ProductGetByIdResponse,
  Gender
} from '../../../dto/product.dto';
import { ImageGetByEntityResponse } from '../../../dto/image.dto';
import { resolveApiAssetUrl } from '../../../helper/resolve-api-asset-url';

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
  isSaving = false;
  imageError = '';

  brands: { id: number; name: string }[] = [];
  colors: { id: number; name: string }[] = [];
  categories: { id: number; name: string }[] = [];

  selectedFile: File | null = null;
  previewUrl: string | null = null;
  existingImage?: ImageGetByEntityResponse;

  @ViewChild('imageInput') imageInput?: ElementRef<HTMLInputElement>;

  private readonly allowedExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.webp'];
  private readonly maxFileSize = 5 * 1024 * 1024;

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
    private colorApi: ColorApi,
    private categoryApi: CategoryApi,
    private imageApi: ImageApi
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      gender: [Gender.Male, Validators.required],
      colorId: [null, Validators.required],
      brandId: [null, Validators.required],
      categoryChoice: [null, Validators.required],
      newCategoryName: ['']
    });

    this.form.get('categoryChoice')?.valueChanges.subscribe((value) => {
      const nameControl = this.form.get('newCategoryName');
      if (!nameControl) {
        return;
      }
      if (value === 'new') {
        nameControl.setValidators([Validators.required, Validators.minLength(2), Validators.maxLength(80)]);
      } else {
        nameControl.clearValidators();
        nameControl.setValue('', { emitEvent: false });
      }
      nameControl.updateValueAndValidity({ emitEvent: false });
    });

    this.loadBrands();
    this.loadColors();
    this.loadCategories();

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

  loadCategories() {
    this.categoryApi.getAll().subscribe(categories => {
      this.categories = categories
        .map(c => ({ id: c.id, name: c.name }))
        .sort((a, b) => a.name.localeCompare(b.name));
    });
  }

  loadProduct(id: number): void {
    this.productsApi.getById(id).subscribe((data: ProductGetByIdResponse) => {
      this.form.patchValue({
        name: data.name,
        price: data.price,
        gender: data.gender,
        colorId: data.colorId,
        brandId: data.brandId,
        categoryChoice: data.categoryId ?? null
      });

      if (data.imageUrl) {
        this.previewUrl = resolveApiAssetUrl(data.imageUrl);
      }

      this.loadExistingImage(id);
    });
  }

  loadExistingImage(productId: number): void {
    this.imageApi.getImageByEntity({
      ImageableId: productId,
      ImageableType: 'products'
    }).subscribe({
      next: (images) => {
        if (images.length > 0) {
          this.existingImage = images[images.length - 1];
          this.previewUrl = resolveApiAssetUrl(this.existingImage.url, this.previewUrl ?? '');
        }
      },
      error: () => {
        // Keep any URL already loaded from the product payload.
      }
    });
  }

  onImageSelected(event: Event): void {
    const file = (event.target as HTMLInputElement)?.files?.[0];
    if (!file) {
      return;
    }

    const extension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    if (!this.allowedExtensions.includes(extension)) {
      this.imageError = 'Use a JPG, PNG, GIF, or WebP image.';
      this.clearFileInput();
      return;
    }

    if (file.size > this.maxFileSize) {
      this.imageError = 'Image must be 5 MB or smaller.';
      this.clearFileInput();
      return;
    }

    this.imageError = '';
    this.selectedFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  clearSelectedImage(): void {
    this.selectedFile = null;
    this.imageError = '';
    this.clearFileInput();

    if (this.existingImage?.url) {
      this.previewUrl = resolveApiAssetUrl(this.existingImage.url);
    } else {
      this.previewUrl = null;
    }
  }

  createNewColor() {
    this.router.navigate(['/color'], { relativeTo: this.route });
  }

  createNewBrand() {
    this.router.navigate(['/brand'], { relativeTo: this.route });
  }

  submit(): void {
    if (this.form.invalid || this.isSaving) return;

    this.isSaving = true;
    const isNewCategory = this.form.value.categoryChoice === 'new';
    const request: ProductUpdateOrInsertRequest = {
      name: this.form.value.name,
      price: this.form.value.price,
      gender: this.form.value.gender,
      colorId: this.form.value.colorId,
      brandId: this.form.value.brandId,
      id: this.isEditMode ? this.productId : undefined,
      categoryId: isNewCategory ? null : Number(this.form.value.categoryChoice),
      newCategoryName: isNewCategory ? String(this.form.value.newCategoryName).trim() : null
    };

    this.productsApi.updateOrInsert(request).subscribe({
      next: (id) => {
        const productId = this.isEditMode ? this.productId! : Number(id);
        if (this.selectedFile) {
          this.saveProductImage(productId);
        } else {
          this.finishSave();
        }
      },
      error: () => {
        this.isSaving = false;
        this.imageError = 'Could not save the product.';
      }
    });
  }

  private saveProductImage(productId: number): void {
    if (!this.selectedFile) {
      this.finishSave();
      return;
    }

    const formData = new FormData();
    formData.append('Name', this.form.value.name || this.selectedFile.name);
    formData.append('ImageableId', productId.toString());
    formData.append('Imageabletype', 'products');
    formData.append('ImageableType', 'products');
    formData.append('File', this.selectedFile, this.selectedFile.name);

    if (this.existingImage?.id) {
      formData.append('Id', this.existingImage.id.toString());
      this.imageApi.imageUpdate(formData).subscribe({
        next: () => this.finishSave(),
        error: () => {
          this.isSaving = false;
          this.imageError = 'Product saved, but the image could not be uploaded.';
        }
      });
      return;
    }

    this.imageApi.imageUpload(formData).subscribe({
      next: () => this.finishSave(),
      error: () => {
        this.isSaving = false;
        this.imageError = 'Product saved, but the image could not be uploaded.';
      }
    });
  }

  private finishSave(): void {
    this.isSaving = false;
    alert('Product has been updated.');
    this.router.navigate(['/products']);
  }

  private clearFileInput(): void {
    if (this.imageInput?.nativeElement) {
      this.imageInput.nativeElement.value = '';
    }
  }
}
