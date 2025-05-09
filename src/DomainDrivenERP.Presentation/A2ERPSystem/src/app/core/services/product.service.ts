import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  sku: string;
  stockQuantity: number;
  categoryId: string;
  createdAt: string;
  updatedAt: string;
}

export interface AddProductRequest {
  name: string;
  description: string;
  price: number;
  sku: string;
  stockQuantity: number;
  categoryId: string;
}

export interface UpdateProductNameRequest {
  productId: string;
  newName: string;
}

export interface UpdateProductPriceRequest {
  productId: string;
  newPrice: number;
}

export interface ApplyDiscountRequest {
  productId: string;
  discountPercentage: number;
}

export interface ApiResponse<T> {
  data: T;
  isSuccess: boolean;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private apiService: ApiService) {}

  getProductById(productId: string): Observable<ApiResponse<Product>> {
    return this.apiService.get<ApiResponse<Product>>(`v1/products/${productId}`);
  }

  getProductBySku(sku: string): Observable<ApiResponse<Product>> {
    return this.apiService.get<ApiResponse<Product>>('v1/products/by-sku', { SKU: sku });
  }

  getProductsByCategory(categoryId: string, fromDate: Date, toDate: Date): Observable<ApiResponse<Product[]>> {
    return this.apiService.get<ApiResponse<Product[]>>('v1/products/by-category', {
      categoryId,
      fromDate: fromDate.toISOString(),
      toDate: toDate.toISOString()
    });
  }

  getProductsByStockQuantity(quantity: number): Observable<ApiResponse<Product[]>> {
    return this.apiService.get<ApiResponse<Product[]>>('v1/products/by-stock-quantity', { quantity });
  }

  addProduct(product: AddProductRequest): Observable<ApiResponse<Product>> {
    return this.apiService.post<ApiResponse<Product>>('v1/products/add', product);
  }

  updateProductName(request: UpdateProductNameRequest): Observable<ApiResponse<boolean>> {
    return this.apiService.put<ApiResponse<boolean>>('v1/products/update-name', request);
  }

  updateProductPrice(request: UpdateProductPriceRequest): Observable<ApiResponse<boolean>> {
    return this.apiService.put<ApiResponse<boolean>>('v1/products/update-price', request);
  }

  applyDiscount(request: ApplyDiscountRequest): Observable<ApiResponse<number>> {
    return this.apiService.post<ApiResponse<number>>('v1/products/apply-discount', request);
  }
}