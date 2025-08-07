import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LoginComponent} from './login/login.component';
import {ForgetPasswordComponent} from './forget-password/forget-password.component';
import {TwoFactorComponent} from './two-factor/two-factor.component';
import {AdminErrorPageComponent} from '../admin/admin-error-page/admin-error-page.component';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LogoutComponent} from './logout/logout.component';
import {SignupComponent} from './signup/signup.component';
import {ResetPasswordComponent} from './reset-password/reset-password.component';
import {ConfirmEmailComponent} from './confirm-email/confirm-email.component';
import {ResendEmailComponent} from './resend-email/resend-email.component';

const routes: Routes = [
  {
    path: '', component: AuthLayoutComponent, children: [
      {path: '', redirectTo: 'login', pathMatch: 'full'},
      {path: 'login', component: LoginComponent},
      {path: 'logout', component: LogoutComponent},
      {path: 'forget-password', component: ForgetPasswordComponent},
      {path: 'two-factor', component: TwoFactorComponent},
      {path:'reset-password',component:ResetPasswordComponent},
      {path: 'signup',component:SignupComponent},
      {path:'confirm-email',component:ConfirmEmailComponent},
      {path:'resend-email',component:ResendEmailComponent},
      {path: '**', component: AdminErrorPageComponent}  // Default ruta koja vodi na public
    ]
  }
]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule {
}
