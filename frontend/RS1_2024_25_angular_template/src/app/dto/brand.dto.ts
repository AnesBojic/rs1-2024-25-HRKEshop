export interface BrandGetAllResponse {
  id: number;
  name: string;
  tenantId?: number;
}

export interface BrandGetAllPagedRequest {
  name?: string;
  pageNumber: number;
  pageSize: number;
}

export interface BrandGetAllPagedResponse {
  id: number;
  name: string;
}

export interface BrandGetByIdResponse {
  id: number;
  name: string;
  tenantId?: number;
}

export interface BrandUpdateOrInsertRequest {
  id?: number;
  name: string;
}

// Paginirani odgovor - generalni interface, isto kao kod proizvoda
export interface MyPagedList<T> {
  dataItems: T[];
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}
