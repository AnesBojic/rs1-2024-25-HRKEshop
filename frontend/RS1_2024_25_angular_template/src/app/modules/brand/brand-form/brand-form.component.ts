import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BrandApi, BrandGetByIdResponse, BrandUpdateOrInsertRequest } from '../../../api/brand.api';

@Component({
  selector: 'app-brand-form',
  templateUrl: './brand-form.component.html',
  styleUrls: ['./brand-form.component.css'],
  standalone: false
})
export class BrandFormComponent implements OnInit {
  brandForm!: FormGroup;
  brandId?: number;

  constructor(
    private fb: FormBuilder,
    private brandApi: BrandApi,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.brandForm = this.fb.group({
      name: ['', Validators.required]
    });

    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.brandId = +idParam;
        this.loadBrand(this.brandId);
      }
    });
  }

  loadBrand(id: number): void {
    this.brandApi.getById(id).subscribe((brand: BrandGetByIdResponse) => {
      this.brandForm.patchValue({
        name: brand.name
      });
    });
  }

  onSubmit(): void {
    if (this.brandForm.invalid) return;

    const request: BrandUpdateOrInsertRequest = {
      id: this.brandId,
      name: this.brandForm.value.name
    };

    this.brandApi.updateOrInsert(request).subscribe(() => {
      this.router.navigate(['/brand']);
    });
  }

  onCancel(): void {
    this.router.navigate(['/brand']);
  }
}
