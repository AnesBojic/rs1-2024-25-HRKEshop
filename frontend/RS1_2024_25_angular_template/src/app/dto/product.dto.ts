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
  imageUrl?: string | null;
}

export interface ProductDetailsSize {
  productSizeId: number;
  sizeName: string;
  price: number;
  stock: number;
}

export interface ProductColorVariant {
  id: number;
  name: string;
  price: number;
  colorId: number;
  colorName: string;
  colorHex: string;
  imageUrl?: string | null;
  availableStock: number;
  isCurrent: boolean;
}

export interface ProductDetailsResponse {
  id: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  colorName: string;
  colorHex: string;
  brandId: number;
  brandName: string;
  categoryId?: number | null;
  categoryName?: string;
  imageUrl?: string | null;
  sizes: ProductDetailsSize[];
  colorVariants: ProductColorVariant[];
}

export interface ProductGetByIdResponse {
  id: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
  categoryId?: number | null;
  categoryName?: string | null;
  tenantId: number;
  imageUrl?: string | null;
}

export interface ProductUpdateOrInsertRequest {
  id?: number;
  name: string;
  price: number;
  gender: Gender;
  colorId: number;
  brandId: number;
  categoryId?: number | null;
  newCategoryName?: string | null;
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




