import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  ColorApi,
  ColorUpdateOrInsertRequest,
  ColorGetAllResponse
} from '../../../api/color.api';

@Component({
  selector: 'app-color-form',
  templateUrl: './color-form.component.html',
  styleUrls: ['./color-form.component.css'],
  standalone: false
})
export class ColorFormComponent implements OnInit {
  form!: FormGroup;
  colorId?: number;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private colorApi: ColorApi
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      hex_Code: [
        '#000000',
        [Validators.required, Validators.pattern(/^#([0-9A-Fa-f]{6})$/)]
      ]
    });

    this.colorId = Number(this.route.snapshot.paramMap.get('id'));

    if (this.colorId) {
      this.loadColor();
    }
  }

  loadColor() {
    this.loading = true;
    this.colorApi.getById(this.colorId!).subscribe({
      next: (color: ColorGetAllResponse) => {
        this.form.patchValue(color);
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  save() {
    if (this.form.invalid) return;

    const request: ColorUpdateOrInsertRequest = {
      id: this.colorId,
      ...this.form.value
    };

    this.colorApi.updateOrInsert(request).subscribe(() => {
      this.router.navigate(['../'], { relativeTo: this.route });
    });
  }

  cancel() {
    this.router.navigate(['../'], { relativeTo: this.route });
  }
}
