# generate-types.ps1
# Regenerate TypeScript API types from Swagger and prepend timestamp

# Set timestamp for the file header
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# URL of the backend Swagger JSON
$swaggerUrl = "https://localhost:7212/swagger/v1/swagger.json"

# Check if backend is responding
try {
    Invoke-WebRequest -Uri $swaggerUrl -UseBasicParsing -TimeoutSec 5 | Out-Null
} catch {
    Write-Error "Cannot reach backend at $swaggerUrl. Make sure the development server is running."
    exit 1
}

# Temporarily allow self-signed certs (for local dev)
$env:NODE_TLS_REJECT_UNAUTHORIZED="0"

# Generate the TypeScript types from Swagger
npx openapi-typescript $swaggerUrl -o src/types/api.d.ts

# Reset TLS verification
$env:NODE_TLS_REJECT_UNAUTHORIZED="1"

# Prepend a timestamp comment to the generated file
$header = @"
/**
 * Generated on: $timestamp
 */
"@

# Read the generated file and prepend the header
$content = Get-Content -Raw src/types/api.d.ts
Set-Content -Path src/types/api.d.ts -Value ($header + "`n" + $content)

Write-Host "api.d.ts regenerated with timestamp $timestamp"
Write-Host "======= TYPES GENERATED FROM BACKEND SWAGGER SCHEMAS =========="