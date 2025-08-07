export  interface SignupRequestDto
{
  name:string,
  surname:string,
  email:string,
  password:string,
  phone?:string

}
export  interface SignupResponseDto
{
  id:number,
  message:string
}

export interface  LoginRequestDto
{
  email:string,
  password:string
}
export interface  LoginResponseDto
{
  token:string,
  refreshToken:string,
  email:string,
}
export interface ForgotPasswordDto
{
  email:string
}
export interface ResetPasswordRequestDto
{
  token:string,
  newPassword:string,
}
export interface ResetPasswordResponseDto
{
  id:number,
  message:string
}
export interface ConfirmEmailRequestDto
{
  token:string
}
export interface ResendEmailRequstDto
{
  email:string
}
