import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {UserProfileResponse,AppuserUpdateRequestDto} from '../dto/appUser.dto';
import {MyConfig} from '../my-config';


@Injectable({
  providedIn: 'root'
})
export class AppUserApi {

  constructor(private http:HttpClient) {
  }

  getUserProfile() : Observable<UserProfileResponse>{
    return this.http.get<UserProfileResponse>(`${MyConfig.api_address}/appusers/profile`);}

  updateUserProfile(request:AppuserUpdateRequestDto) : Observable<any> {
    return  this.http.patch(`${MyConfig.api_address}/appusers/profile`,request);
  }



}
