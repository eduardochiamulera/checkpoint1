# One-time cleanup script that bypasses execution policy
# Run with: powershell -ExecutionPolicy Bypass -File cleanup-old-structure-onetime.ps1

Write-Host "Starting cleanup of old project structure..." -ForegroundColor Yellow

# Remove old folders
$oldPaths = @(
    "src/Controllers",
    "src/Data",
    "src/Domains",
    "src/Exceptions",
    "src/Migrations",
    "src/Models",
    "src/Services",
    "src/Properties",
    "src/docs",
    "src/.gitignore",
    "src/CHANGELOG.md",
    "src/Cursos.csproj",
    "src/Cursos.http",
    "src/Program.cs",
    "src/appsettings.json",
    "src/appsettings.Development.json",
    "src/appsettings.example.json"
)

foreach ($path in $oldPaths) {
    if (Test-Path $path) {
        Remove-Item $path -Recurse -Force
        Write-Host "✓ Removed: $path" -ForegroundColor Green
    } else {
        Write-Host "- Not found (already removed?): $path" -ForegroundColor Gray
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Cleanup complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nNew structure:" -ForegroundColor Yellow
Write-Host "  src/Cursos.Domain/"
Write-Host "  src/Cursos.Application/"
Write-Host "  src/Cursos.Infrastructure/"
Write-Host "  src/Cursos.API/"
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. dotnet build Cursos.Architecture.sln"
Write-Host "  2. cd src/Cursos.API"
Write-Host "  3. dotnet run"
Write-Host "`n"
