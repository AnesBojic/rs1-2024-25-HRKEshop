import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { CartRoutingModule } from './cart-routing.module';
import { CartPageComponent } from './cart-page/cart-page.component';

@NgModule({
  declarations: [CartPageComponent],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    CartRoutingModule
  ]
})
export class CartModule {}
