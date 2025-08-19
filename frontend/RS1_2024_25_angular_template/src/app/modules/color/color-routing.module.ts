import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ColorListComponent } from './color-list/color-list.component';
import { ColorFormComponent } from './color-form/color-form.component';

const routes: Routes = [
  { path: '', component: ColorListComponent },
  { path: 'new', component: ColorFormComponent },
  { path: ':id', component: ColorFormComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ColorRoutingModule { }
