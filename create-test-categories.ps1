# PowerShell script to create test categories
# Make sure the API is running on https://localhost:7001

$apiBase = "https://localhost:7001/api/v1"

# Categories to create
$categories = @(
    @{ name = "Electronics" },
    @{ name = "Clothing" },
    @{ name = "Books" },
    @{ name = "Home & Garden" },
    @{ name = "Sports & Outdoors" }
)

Write-Host "Creating test categories..." -ForegroundColor Green

foreach ($category in $categories) {
    try {
        $body = $category | ConvertTo-Json
        Write-Host "Creating category: $($category.name)" -ForegroundColor Yellow
        
        $response = Invoke-RestMethod -Uri "$apiBase/categories/create" `
            -Method POST `
            -Body $body `
            -ContentType "application/json" `
            -SkipCertificateCheck
            
        Write-Host "✅ Successfully created: $($category.name)" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Failed to create $($category.name): $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "Response: $($_.Exception.Response)" -ForegroundColor Red
    }
}

Write-Host "`nTesting GetAllCategories endpoint..." -ForegroundColor Green
try {
    $allCategories = Invoke-RestMethod -Uri "$apiBase/categories" `
        -Method GET `
        -SkipCertificateCheck
        
    Write-Host "✅ Found $($allCategories.Count) categories:" -ForegroundColor Green
    foreach ($cat in $allCategories) {
        Write-Host "  - $($cat.name) (ID: $($cat.id))" -ForegroundColor Cyan
    }
}
catch {
    Write-Host "❌ Failed to get categories: $($_.Exception.Message)" -ForegroundColor Red
}
