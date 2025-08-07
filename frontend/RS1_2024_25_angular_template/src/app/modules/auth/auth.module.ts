import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {AuthRoutingModule} from './auth-routing.module';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {ForgetPasswordComponent} from './forget-password/forget-password.component';
import {TwoFactorComponent} from './two-factor/two-factor.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {LogoutComponent} from './logout/logout.component';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {TranslatePipe} from "@ngx-translate/core";
import {SharedModule} from "../shared/shared.module";
import { SignupComponent } from './signup/signup.component';
import {MaterialModule} from '../material/material.module';
import {MatSlideToggle} from '@angular/material/slide-toggle';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ResendEmailComponent } from './resend-email/resend-email.component';


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ForgetPasswordComponent,
    TwoFactorComponent,
    LogoutComponent,
    AuthLayoutComponent,
    SignupComponent,
    ResetPasswordComponent,
    ConfirmEmailComponent,
    ResendEmailComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    FormsModule,
    TranslatePipe,
    SharedModule,
    MaterialModule,
    ReactiveFormsModule,
    MatSlideToggle,
    MaterialModule


  ]
})
export class AuthModule {
}
