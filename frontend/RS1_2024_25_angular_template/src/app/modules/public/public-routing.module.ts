import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {PublicLayoutComponent} from './public-layout/public-layout.component';
import {NavbarComponent} from '../shared/navbar/navbar.component';

const routes: Routes = [
  {
    path: '', component: PublicLayoutComponent, children: [
      {path: '', redirectTo: 'home', pathMatch: 'full'},
      {path:'navbar',component:NavbarComponent},
      {path: '**', redirectTo: 'home', pathMatch: 'full'}  // Default ruta koja vodi na public
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
