import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {tap} from 'rxjs/operators';
import {MyConfig} from '../../my-config';
import {MyAuthService} from '../../services/auth-services/my-auth.service';
import {LoginTokenDto} from '../../services/auth-services/dto/login-token-dto';
import {MyBaseEndpointAsync} from '../../helper/my-base-endpoint-async.interface';
import {AuthService} from '../../services/auth-services/auth.service';

export interface LoginRequest {
  email: string;
  password: string;
}
export  interface  LoginResponse
{
  token :string;
  refreshToken : string;
  email:string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthLoginEndpointService implements MyBaseEndpointAsync<LoginRequest, LoginResponse> {
  private apiUrl = `${MyConfig.api_address}/auth1/login`;

  constructor(private httpClient: HttpClient, private authService: AuthService) {
  }

  handleAsync(request:LoginRequest)
  {
    return this.httpClient.post<LoginResponse>(this.apiUrl,request).pipe(
      tap((response)=>
      {
        this.authService.setTokens(response.token,response.refreshToken);
      })

    )


  }


}
