import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnauthorizedComponent } from './modules/shared/unauthorized/unauthorized.component';

import { AuthGuard } from './auth-guards/auth-guard.service';
import { AppShellComponent } from './modules/shared/app-shell/app-shell.component';

const routes: Routes = [
  { path: 'unauthorized', component: UnauthorizedComponent },
  {
    path: '',
    component: AppShellComponent,
    children: [
      {
        path: 'public',
        loadChildren: () => import('./modules/public/public.module').then(m => m.PublicModule)
      },
      {
        path: 'products',
        loadChildren: () => import('./modules/product/product.module').then(m => m.ProductModule)
      },
      {
        path: 'brand',
        loadChildren: () => import('./modules/brand/brand.module').then(m => m.BrandModule)
      },
      {
        path: 'color',
        loadChildren: () => import('./modules/color/color.module').then(m => m.ColorModule)
      },
      {
        path: 'client',
        loadChildren: () => import('./modules/client/client.module').then(m => m.ClientModule)
      },
      {
        path: 'auth',
        loadChildren: () => import('./modules/auth/auth.module').then(m => m.AuthModule)
      },
      { path: '', redirectTo: 'public', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'public', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
