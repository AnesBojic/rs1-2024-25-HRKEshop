export interface ColorGetAllResponse {
  id: number;
  name: string;
  hex_Code: string;
}

export interface ColorGetByIdResponse {
  id: number;
  name: string;
  hex_Code: string;
}

export interface ColorUpdateOrInsertRequest {
  id?: number;
  name: string;
  hex_Code: string;
}

export interface ColorGetAllPagedRequest {
  q?: string;
  hex_Code?: string;
  pageNumber: number;
  pageSize: number;
}

export interface ColorGetAllPagedResponse {
  id: number;
  name: string;
  hex_Code: string;
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
