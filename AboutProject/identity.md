# Identity & Auth — `DomainDrivenERP.Identity`

## Purpose

Authentication and authorization using ASP.NET Core Identity + JWT tokens + Claims-based permissions.

## Architecture

```
DomainDrivenERP.Identity/
├── Data/
│   └── IdentityDbContext.cs         # Separate DB from application data
├── Models/
│   ├── ApplicationUser.cs           # Extended IdentityUser
│   ├── JwtSettings.cs
│   └── EmailSettings.cs
├── Services/
│   ├── AuthService.cs               # Login, Register, RefreshToken
│   ├── IdentityService.cs           # User management
│   ├── RoleService.cs               # Role + Claim management
│   └── EmailsService.cs             # Email confirmation
├── Filters/
│   ├── PermissionAuthorizationHandler.cs
│   ├── PermissionPolicyProvider.cs
│   └── PermissionRequirement.cs
└── Migrations/                      # Separate migration set
```

## JWT Configuration

```json
{
  "JwtSettings": {
    "Key": "your-256-bit-secret-key-here",
    "Issuer": "https://your-domain.com/",
    "Audience": "https://your-domain.com/",
    "DurationInDays": 30
  }
}
```

## Permission System

Permissions are stored as Claims and checked via custom `IAuthorizationHandler`:

```csharp
// Define permissions
public static class GdprPermissions
{
    public const string Anonymize = "GDPR.Anonymize";
    public const string Download  = "GDPR.Download";
}

// Protect endpoint
[Authorize(Policy = GdprPermissions.Anonymize)]
public async Task<IActionResult> Anonymize(...) { }

// Assign permission to role
await _roleService.AddClaimToRoleAsync(roleId, "Permission", GdprPermissions.Anonymize);
```

## Auth Flow

```
POST /api/auth/login
  → Validate credentials
  → Generate JWT (15min access + 30day refresh)
  → Return tokens

POST /api/auth/refresh
  → Validate refresh token
  → Issue new access token

GET /api/invoices/customer/{id}
  → JWT validated by middleware
  → Permission claim checked
  → Handler executes
```
