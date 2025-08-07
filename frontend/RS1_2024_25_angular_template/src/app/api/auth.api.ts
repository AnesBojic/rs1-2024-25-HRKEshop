  import { Injectable } from '@angular/core';
  import  {MyConfig} from '../my-config';
  import {HttpClient} from '@angular/common/http';
  import {
    ConfirmEmailRequestDto,
    ForgotPasswordDto,
    LoginRequestDto,
    LoginResponseDto, ResendEmailRequstDto, ResetPasswordRequestDto, ResetPasswordResponseDto,
    SignupRequestDto,
    SignupResponseDto
  } from '../dto/auth.dto';
  import {Observable} from 'rxjs';
  import {httpOptionsHelper} from '../helper/http-options.helper';

  @Injectable({
    providedIn: 'root'
  })
  export class AuthApi {

    private  readonly  baseUrl = `${MyConfig.api_address}`;


    constructor(private  http:HttpClient) { }


    signup(data:SignupRequestDto) : Observable<SignupResponseDto>
    {
      return this.http.post<SignupResponseDto>(`${this.baseUrl}/appusers/add`,data);
    }
    logout() {
      return this.http.post(`${this.baseUrl}/auth/logout1`,{},{responseType:"text"});
    }
    login(data:LoginRequestDto) : Observable<LoginResponseDto>
    {
      return this.http.post<LoginResponseDto>(`${this.baseUrl}/auth1/login`,data,httpOptionsHelper())
    }
    forgotPassword(data:ForgotPasswordDto) : Observable<any>
    {
      return  this.http.post(`${this.baseUrl}/auth1/forgot-password`,data);
    }
    resetPassword(data:ResetPasswordRequestDto) : Observable<ResetPasswordResponseDto>
    {
      return this.http.post<ResetPasswordResponseDto>(`${this.baseUrl}/auth1/reset-password`,data);
    }
    confirmEmail(data:ConfirmEmailRequestDto) : Observable<any>
    {
      return this.http.post<any>(`${this.baseUrl}/auth1/confirm-email`,data);
    }
    resendEmail(data:ResendEmailRequstDto) : Observable<any>
    {
      return  this.http.post<any>(`${this.baseUrl}/auth1/resend-email`,data);
    }


  }
