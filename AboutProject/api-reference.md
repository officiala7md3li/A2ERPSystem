# A2ERP API Reference

Base URL: `https://localhost:7124/api`

All endpoints require `Authorization: Bearer {jwt_token}` unless noted.

---

## Authentication

### Login
```
POST /auth/login
```
```json
{
  "email": "admin@company.com",
  "password": "password123"
}
```
**Response:**
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "...",
  "expiresIn": 2592000
}
```

---

## Customer Invoices

### Create Invoice (Draft)
```
POST /invoices/customer
```
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "companyId":  "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "currencyId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
  "invoiceDate": "2026-03-22",
  "taxOrderSetting": "AfterDiscount",
  "stackingMode": "NoStacking",
  "notes": "Optional notes"
}
```
**Response 201:**
```json
{
  "invoiceId": "...",
  "customerId": "...",
  "invoiceDate": "2026-03-22",
  "status": "Draft"
}
```

---

### Get Invoice Detail
```
GET /invoices/customer/{id}
```
**Response 200:**
```json
{
  "id": "...",
  "sequenceNumber": "INV-20260322-ABC123",
  "status": "Posted",
  "subTotal": 5000.00,
  "totalLineDiscount": 250.00,
  "totalTax": 658.00,
  "totalHiddenDiscount": 0.00,
  "grandTotal": 5408.00,
  "lines": [
    {
      "id": "...",
      "itemId": "...",
      "quantity": 10,
      "quantityUnit": "PCS",
      "unitPrice": 500.00,
      "currency": "EGP",
      "subTotal": 5000.00,
      "totalDiscountAmount": 250.00,
      "totalTaxAmount": 658.00,
      "finalLineTotal": 5408.00,
      "taxBreakdowns": [
        { "taxCode": "VAT", "taxName": "Value Added Tax", "rate": 0.14, "taxAmount": 658.00, "isWithholding": false }
      ],
      "discountBreakdowns": [
        { "source": "PriceList", "type": "Percentage", "discountValue": 5.0, "discountAmount": 250.00 }
      ]
    }
  ]
}
```

---

### Add Line
```
POST /invoices/customer/{id}/lines
```
```json
{
  "itemId": "...",
  "quantity": 10,
  "quantityUnit": "PCS",
  "unitPrice": 500.00,
  "currency": "EGP",
  "taxGroupId": "...",
  "discountGroupId": "..."
}
```

---

### Submit (Draft → Pending)
```
POST /invoices/customer/{id}/submit
```
Response: `204 No Content`

---

### Post (Pending → Posted)
```
POST /invoices/customer/{id}/post
```
**Response 200:**
```json
{
  "invoiceId": "...",
  "sequenceNumber": "INV-20260322-ABC123",
  "grandTotal": 5408.00,
  "status": "Posted"
}
```

---

### Cancel
```
POST /invoices/customer/{id}/cancel
```
```json
{ "reason": "Customer requested cancellation" }
```
Response: `204 No Content`

---

## Error Response Format

All errors follow this structure:
```json
{
  "code": "CustomerInvoice.NotFound",
  "description": "Invoice '3fa85f64...' not found."
}
```

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 204 | No Content |
| 400 | Validation error or business rule failure |
| 401 | Unauthorized |
| 403 | Forbidden (insufficient permissions) |
| 404 | Not found |
| 500 | Internal server error |
