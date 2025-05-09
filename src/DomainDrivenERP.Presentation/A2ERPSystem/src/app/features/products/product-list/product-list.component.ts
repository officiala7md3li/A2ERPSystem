import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService, Product, ApiResponse } from '../../../core/services/product.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  loading = false;
  error: string | null = null;
  columns = ['name', 'sku', 'price', 'stockQuantity', 'categoryId', 'actions'];

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = null;
    
    // Using a default quantity of 0 to get all products
    this.productService.getProductsByStockQuantity(0).subscribe({
      next: (response: ApiResponse<Product[]>) => {
        if (response.isSuccess) {
          this.products = response.data;
        } else {
          this.error = response.errors?.join(', ') || 'Failed to load products';
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = 'An error occurred while loading products';
        console.error('Error loading products:', err);
        this.loading = false;
      }
    });
  }
}