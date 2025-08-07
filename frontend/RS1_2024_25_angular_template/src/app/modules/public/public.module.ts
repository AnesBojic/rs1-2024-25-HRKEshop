import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {PublicRoutingModule} from './public-routing.module';
import {FormsModule} from '@angular/forms';
import {SharedModule} from '../shared/shared.module';
import {PublicLayoutComponent} from './public-layout/public-layout.component';

@NgModule({
  declarations: [
    PublicLayoutComponent
  ],
  imports: [
    CommonModule,
    PublicRoutingModule,
    FormsModule,
    SharedModule
  ],

})
export class PublicModule {
}
