import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrandRoutingModule } from './brand-routing.module';
import { BrandListComponent } from './brand-list/brand-list.component';
import { BrandFormComponent } from './brand-form/brand-form.component';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [
    BrandListComponent,
    BrandFormComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrandRoutingModule,
    TranslateModule // ⚡ OVO JE KLJUČ
  ]
})
export class BrandModule {}
