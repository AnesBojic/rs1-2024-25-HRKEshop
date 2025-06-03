

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MyConfig } from '../my-config';
import { BrandGetAll1Response } from '../models/brand-get-all1-response';
import { MyBaseEndpointAsync } from '../my-endpoint-base';

export interface BrandGetAll1Response {
  id: number;
  name: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class BrandGetAll1EndpointService implements MyBaseEndpointAsync<void, BrandGetAll1Response[]> {
  private apiUrl = `${MyConfig.api_address}/brands/all`;

  constructor(private httpClient: HttpClient) {
  }

  handleAsync() {
    return this.httpClient.get<BrandGetAll1Response[]>(this.apiUrl);
  }
