import { Injectable } from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {AuthService} from './auth.service';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptorService implements  HttpInterceptor{

  constructor(private  authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    const token = this.authService.getAccessToken();
    let headers = req.headers;

    if (token) {
      headers = headers.set('my-auth-token', token);
    }

    // The browser must set the multipart boundary for file uploads.
    if (req.body instanceof FormData && headers.has('Content-Type')) {
      headers = headers.delete('Content-Type');
    }

    if (headers === req.headers) {
      return next.handle(req);
    }

    return next.handle(req.clone({ headers }));
  }


}
