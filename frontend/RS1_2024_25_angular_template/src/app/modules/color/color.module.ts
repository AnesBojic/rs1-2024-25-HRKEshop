import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ColorRoutingModule } from './color-routing.module';
import { ColorListComponent } from './color-list/color-list.component';
import { ColorFormComponent } from './color-form/color-form.component';
import { TranslateModule } from '@ngx-translate/core'; //

@NgModule({
  declarations: [
    ColorListComponent,
    ColorFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ColorRoutingModule,
    TranslateModule   //
  ]
})
export class ColorModule { }
