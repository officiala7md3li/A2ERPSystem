# DomainDrivenERP.Web - MVC Frontend

This is an ASP.NET Core MVC frontend application for the Domain Driven ERP system. It provides a web-based user interface that consumes the DomainDrivenERP.API backend.

## Features

- **Authentication & Authorization**: JWT-based authentication with cookie storage
- **Product Management**: Create, view, and edit products
- **Order Management**: Create and manage customer orders
- **Customer Management**: View customer information
- **Responsive UI**: Bootstrap-based responsive design with Font Awesome icons
- **Dashboard**: Overview of key business metrics and quick actions

## Architecture

The project follows the MVC (Model-View-Controller) pattern with the following structure:

### Controllers
- `AuthController`: Handles user authentication (login, register, logout)
- `HomeController`: Dashboard and landing pages
- `ProductsController`: Product management operations
- `OrdersController`: Order management operations
- `CustomersController`: Customer management operations

### Services
- `IApiService` / `ApiService`: HTTP client wrapper for API communication
- `IAuthService` / `AuthService`: Authentication and session management

### Models
- **Auth**: Login/Register request/response models
- **Products**: Product entities and DTOs
- **Orders**: Order entities and DTOs
- **Customers**: Customer entities
- **Common**: Shared models like ApiResponse

## Configuration

### API Settings
Configure the API connection in `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7182",
    "Timeout": 30
  }
}
```

### JWT Settings
JWT configuration for token validation:

```json
{
  "JwtSettings": {
    "Key": "your-secret-key",
    "Issuer": "https://localhost:7124/",
    "Audience": "https://localhost:7124/",
    "DurationInDays": 30
  }
}
```

## Running the Application

1. **Prerequisites**: Ensure the DomainDrivenERP.API is running
2. **Build**: `dotnet build`
3. **Run**: `dotnet run`
4. **Access**: Navigate to `https://localhost:7200` or `http://localhost:5200`

## API Integration

The application communicates with the backend API through:

- **Authentication**: `/api/v1/auth/Login`, `/api/v1/auth/Register`
- **Products**: `/api/v1/products/*`
- **Orders**: `/api/v1/orders/*`
- **Customers**: `/api/v1/customers/*`

## Authentication Flow

1. User logs in through the web interface
2. Credentials are sent to the API
3. JWT token is received and stored in authentication cookies
4. Token is automatically included in subsequent API requests
5. User session is maintained until logout or token expiration

## Development Notes

- The application uses cookie-based authentication for web sessions
- JWT tokens are automatically attached to API requests
- Error handling includes user-friendly messages and logging
- The UI is responsive and works on desktop and mobile devices

## Future Enhancements

- Add more comprehensive error handling
- Implement real-time notifications
- Add data export functionality
- Enhance the dashboard with charts and analytics
- Add bulk operations for products and orders
