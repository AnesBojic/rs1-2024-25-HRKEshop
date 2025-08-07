export enum Gender {
  Male = 1,
  Female = 2,
  Other = 3
}

export interface ProductDto {
  id?: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
  tenantId?: number;
}

export interface ProductGetAll1Response {
  id: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
  tenantId: number;
}

export interface ProductGetAll3Request {
  q?: string;
  gender?: Gender;
  minPrice?: number;
  maxPrice?: number;
  colorId?: number;
  brandId?: number;
  pageNumber: number;
  pageSize: number;
}

export interface ProductGetAll3Response {
  id: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
}

export interface ProductGetByIdResponse {
  id: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
  tenantId: number;
}

export interface ProductUpdateOrInsertRequest {
  id?: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
}


export interface MyPagedList<T> {
  dataItems: T[];
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}




