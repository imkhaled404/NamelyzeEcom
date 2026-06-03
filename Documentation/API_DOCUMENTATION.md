# API Documentation

## Authentication

All protected endpoints require a JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## Base URL
```
https://localhost:5001/api
```

## Endpoints

### Authentication Endpoints

#### Register
- **URL**: `POST /auth/register`
- **Description**: Register a new user
- **Request Body**:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "phoneNumber": "01711111111",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

#### Login
- **URL**: `POST /auth/login`
- **Description**: Login user
- **Request Body**:
```json
{
  "emailOrPhone": "john@example.com",
  "password": "SecurePass123!"
}
```

#### Get Profile
- **URL**: `GET /auth/profile/{userId}`
- **Description**: Get user profile
- **Auth Required**: Yes

#### Update Profile
- **URL**: `PUT /auth/profile/{userId}`
- **Description**: Update user profile
- **Auth Required**: Yes

#### Change Password
- **URL**: `POST /auth/change-password`
- **Description**: Change user password
- **Auth Required**: Yes

---

### Products Endpoints

#### Get All Products
- **URL**: `GET /products`
- **Query Params**: `pageNumber=1&pageSize=10`
- **Description**: Get paginated list of products

#### Get Product by ID
- **URL**: `GET /products/{id}`
- **Description**: Get a specific product

#### Get Product by Slug
- **URL**: `GET /products/slug/{slug}`
- **Description**: Get product by URL slug

#### Get Featured Products
- **URL**: `GET /products/featured`
- **Description**: Get featured products

#### Get New Arrivals
- **URL**: `GET /products/new-arrivals`
- **Description**: Get newly added products

#### Search Products
- **URL**: `GET /products/search?q=search-term`
- **Description**: Search products by keyword

#### Get Products by Category
- **URL**: `GET /products/category/{categoryId}`
- **Description**: Get all products in a category

#### Create Product (Admin)
- **URL**: `POST /products`
- **Auth Required**: Yes (Admin)
- **Request Body**:
```json
{
  "name": "Product Name",
  "description": "Product description",
  "sku": "SKU123",
  "barcode": "123456789",
  "brandId": 1,
  "categoryId": 1,
  "purchasePrice": 1000,
  "regularPrice": 1500,
  "salePrice": 1200,
  "currentStock": 50,
  "minimumStock": 10,
  "metaTitle": "SEO Title",
  "metaDescription": "SEO Description",
  "metaKeywords": "keyword1, keyword2"
}
```

#### Update Product (Admin)
- **URL**: `PUT /products/{id}`
- **Auth Required**: Yes (Admin)

#### Delete Product (Admin)
- **URL**: `DELETE /products/{id}`
- **Auth Required**: Yes (Admin)

---

### Orders Endpoints

#### Get Order by ID
- **URL**: `GET /orders/{id}`
- **Description**: Get specific order details

#### Get Order by Number
- **URL**: `GET /orders/number/{orderNumber}`
- **Description**: Track order by order number

#### Get User Orders
- **URL**: `GET /orders/user/{userId}`
- **Description**: Get all orders of a user
- **Auth Required**: Yes

#### Get Orders by Status
- **URL**: `GET /orders/status/{status}`
- **Description**: Get orders by status
- **Status Values**: 1=Pending, 2=Confirmed, 3=Processing, 4=Packed, 5=Shipped, 6=Delivered, 7=Returned, 8=Cancelled

#### Create Order
- **URL**: `POST /orders`
- **Auth Required**: Yes
- **Request Body**:
```json
{
  "userId": 1,
  "customerName": "John Doe",
  "customerPhone": "01711111111",
  "deliveryAddress": "Street address",
  "district": "Dhaka",
  "area": "Dhanmondi",
  "shippingType": "Inside City",
  "paymentMethodId": 1,
  "couponCode": "DISCOUNT10",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "variantColor": "Red",
      "variantSize": "M"
    }
  ]
}
```

#### Update Order Status (Admin)
- **URL**: `PATCH /orders/{id}/status`
- **Auth Required**: Yes (Admin)
- **Request Body**:
```json
2
```

#### Get Total Sales (Admin)
- **URL**: `GET /orders/sales/total?startDate=2026-01-01&endDate=2026-12-31`
- **Auth Required**: Yes (Admin)

---

### Categories Endpoints

#### Get All Categories
- **URL**: `GET /categories`
- **Description**: Get all categories

#### Get Parent Categories
- **URL**: `GET /categories/parents`
- **Description**: Get top-level categories

#### Get Child Categories
- **URL**: `GET /categories/parent/{parentId}`
- **Description**: Get subcategories

#### Get Category by ID
- **URL**: `GET /categories/{id}`
- **Description**: Get specific category

#### Get Category by Slug
- **URL**: `GET /categories/slug/{slug}`
- **Description**: Get category by URL slug

#### Create Category (Admin)
- **URL**: `POST /categories`
- **Auth Required**: Yes (Admin)

#### Update Category (Admin)
- **URL**: `PUT /categories/{id}`
- **Auth Required**: Yes (Admin)

#### Delete Category (Admin)
- **URL**: `DELETE /categories/{id}`
- **Auth Required**: Yes (Admin)

---

### Coupons Endpoints

#### Get Coupon by ID
- **URL**: `GET /coupons/{id}`
- **Description**: Get coupon details

#### Get Coupon by Code
- **URL**: `GET /coupons/code/{code}`
- **Description**: Get coupon by code

#### Get Active Coupons
- **URL**: `GET /coupons/active`
- **Description**: Get all active/valid coupons

#### Validate Coupon
- **URL**: `POST /coupons/validate`
- **Request Body**:
```json
{
  "code": "DISCOUNT10",
  "orderAmount": 5000
}
```

#### Create Coupon (Admin)
- **URL**: `POST /coupons`
- **Auth Required**: Yes (Admin)

#### Update Coupon (Admin)
- **URL**: `PUT /coupons/{id}`
- **Auth Required**: Yes (Admin)

---

### Reviews Endpoints

#### Get Review by ID
- **URL**: `GET /reviews/{id}`
- **Description**: Get specific review

#### Get Product Reviews
- **URL**: `GET /reviews/product/{productId}`
- **Description**: Get all reviews for a product

#### Get Pending Reviews (Admin)
- **URL**: `GET /reviews/pending`
- **Auth Required**: Yes (Admin)

#### Create Review
- **URL**: `POST /reviews?userId=1`
- **Auth Required**: Yes
- **Request Body**:
```json
{
  "productId": 1,
  "rating": 5,
  "title": "Excellent product",
  "comment": "Very satisfied with the purchase"
}
```

#### Approve Review (Admin)
- **URL**: `POST /reviews/{id}/approve`
- **Auth Required**: Yes (Admin)

#### Reject Review (Admin)
- **URL**: `POST /reviews/{id}/reject`
- **Auth Required**: Yes (Admin)

#### Delete Review (Admin)
- **URL**: `DELETE /reviews/{id}`
- **Auth Required**: Yes (Admin)

---

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {},
  "errors": null
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message",
  "data": null,
  "errors": ["Error 1", "Error 2"]
}
```

---

## Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created
- `400 Bad Request` - Invalid request
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Permission denied
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

---

## Order Statuses

| Code | Status | Description |
|------|--------|-------------|
| 1 | Pending | Order placed, awaiting confirmation |
| 2 | Confirmed | Order confirmed by admin |
| 3 | Processing | Order being prepared |
| 4 | Packed | Order packed and ready |
| 5 | Shipped | Order shipped |
| 6 | Delivered | Order delivered |
| 7 | Returned | Order returned |
| 8 | Cancelled | Order cancelled |
| 9 | Refunded | Order refunded |

---

## Payment Methods

| Code | Method |
|------|--------|
| 1 | Cash on Delivery |
| 2 | bKash |
| 3 | Nagad |
| 4 | Rocket |
| 5 | SSLCommerz |
| 6 | Visa Card |
| 7 | MasterCard |

---

## Rate Limiting

API rate limit: 1000 requests per hour per IP address.

---

## Pagination

For paginated endpoints, use:
- `pageNumber` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)

---

**Last Updated**: June 2026
**API Version**: 1.0
