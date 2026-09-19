export interface CartItemDto {
  id?: number;
  productId: number;
  productSizeId: number;
  productName: string;
  sizeName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
  stock: number;
  imageUrl?: string | null;
}

export interface CartResponse {
  items: CartItemDto[];
  totalQuantity: number;
  totalAmount: number;
}

export interface CartAddRequest {
  productSizeId: number;
  quantity: number;
}

export interface CartSetQuantityRequest {
  productSizeId: number;
  quantity: number;
}

export interface CartMergeRequest {
  items: Array<{ productSizeId: number; quantity: number }>;
}

export interface CartCheckoutRequest {
  shippingAddress: string;
}

export interface CartCheckoutResponse {
  orderId: number;
  totalAmount: number;
  message: string;
}

export interface ProductSizeOption {
  productName: string;
  productSizeId: number;
  sizeId: number;
  sizeName: string;
  priceForItem: number;
  stock: number;
}
