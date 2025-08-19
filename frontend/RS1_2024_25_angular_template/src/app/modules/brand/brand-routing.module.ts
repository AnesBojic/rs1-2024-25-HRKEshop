import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrandListComponent } from './brand-list/brand-list.component';
import { BrandFormComponent } from './brand-form/brand-form.component';

const routes: Routes = [
  { path: '', component: BrandListComponent },
  { path: 'new', component: BrandFormComponent },
  { path: ':id', component: BrandFormComponent } // edit
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BrandRoutingModule {}
