import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ImageGetByEntityResponse,ImageUploadResponse,ImageGetByEntityRequest,ImageUpdateResponse} from '../dto/image.dto';
import {Observable} from 'rxjs';
import {MyConfig} from '../my-config';
import {buildHttpParams} from '../helper/http-params.helper';


@Injectable({
  providedIn: 'root'
})
export class ImageApi {



  constructor(private  http:HttpClient) { }




  getImageByEntity(request: ImageGetByEntityRequest) : Observable<ImageGetByEntityResponse[]>
  {
    let params = buildHttpParams(request);

    return this.http.get<ImageGetByEntityResponse[]>(`${MyConfig.api_address}/images/by-entity`, {params});
  }

  imageUpload(formData:FormData) : Observable<ImageUploadResponse>{

    return  this.http.post<ImageUploadResponse>(`${MyConfig.api_address}/images/upload`,formData);
  }

  imageUpdate(formData:FormData) :Observable<ImageUpdateResponse>
  {
    return this.http.put<ImageUpdateResponse>(`${MyConfig.api_address}/images/update`,formData);
  }
}
