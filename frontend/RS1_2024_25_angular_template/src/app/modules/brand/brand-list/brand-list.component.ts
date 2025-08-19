import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BrandApi, BrandGetAllResponse } from '../../../api/brand.api';

@Component({
  selector: 'app-brand-list',
  templateUrl: './brand-list.component.html',
  styleUrls: ['./brand-list.component.css'],
  standalone: false
})
export class BrandListComponent implements OnInit {
  brands: BrandGetAllResponse[] = [];

  constructor(
    private brandApi: BrandApi,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.brandApi.getAll().subscribe((brands) => {
      this.brands = brands;
    });
  }

  createBrand(): void {
    this.router.navigate(['new'], { relativeTo: this.route });
  }

  editBrand(brand: BrandGetAllResponse): void {
    this.router.navigate([brand.id], { relativeTo: this.route });
  }

  deleteBrand(brand: BrandGetAllResponse): void {
    if (confirm(`Are you sure you want to delete ${brand.name}?`)) {
      this.brandApi.delete(brand.id!).subscribe(() => {
        this.brands = this.brands.filter(b => b.id !== brand.id);
      });
    }
  }
}
