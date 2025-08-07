import { Injectable } from '@angular/core';
import * as  jwtDecode from 'jwt-decode'
import {BehaviorSubject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private  readonly  tokenKey = 'accessToken';
  private  readonly  refreshTokenKey = 'refreshToken';
  private readonly CLAIM_NAME_ID = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
  private readonly CLAIM_EMAIL = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
  private readonly CLAIM_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  private readonly  CLAIM_NAME='http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';


  private loggedInSubject = new BehaviorSubject<boolean>(this.isLoggedIn());
  loggedIn$ = this.loggedInSubject.asObservable();

  constructor() { }


  setTokens(accessToken:string,refreshToken:string) : void
  {
    localStorage.setItem(this.tokenKey,accessToken);
    localStorage.setItem(this.refreshTokenKey,refreshToken);
    this.loggedInSubject.next(true);

  }

  clearTokens(): void
  {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    this.loggedInSubject.next(false);

  }


  getAccessToken() : string | null
  {
    return  localStorage.getItem(this.tokenKey);
  }

  private  decodeToken():any | null
  {
    const token = this.getAccessToken();

    if(!token) return null;

    try {
      const decoded =  jwtDecode.jwtDecode(token);

      return decoded;
    }catch (err)
    {
      console.error('Invalid JWT token',err);
      return null;
    }
  }

  isLoggedIn(): boolean
  {
    const decoded = this.decodeToken();
    if(!decoded) return false;

    const exp = decoded['exp'];
    return exp ? Date.now() < exp * 1000 : false;
  }

  getUserId() : string | null
{
  return this.decodeToken()?.[this.CLAIM_NAME_ID] ?? null;
}

getEmail() : string | null
{
  return this.decodeToken()?.[this.CLAIM_EMAIL] ?? null;
}
getRole() : string | null
{
  return this.decodeToken()?.[this.CLAIM_ROLE] ?? null;
}
getName() : string | null
{
  return this.decodeToken()?.[this.CLAIM_NAME] ?? null;
}

getTenantId() : string | null
{
  return this.decodeToken()?.['tenant_id'] ?? null;

}

isAdmin(): boolean {
    return  this.getRole() === 'Admin';
}
isManager() : boolean
{
  return this.getRole() === 'Manager';
}
isCustomer() : boolean
{
  return this.getRole() === 'Customer';
}




}
