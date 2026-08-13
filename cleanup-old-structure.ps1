# PowerShell script to remove old project structure
# Run this from the repository root after merging feature/clean-architecture to main

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
        Write-Host "Removed: $path" -ForegroundColor Green
    } else {
        Write-Host "Not found (already removed?): $path" -ForegroundColor Gray
    }
}

# Rename Cursos.API to just be the main project
# The new structure already has src/Cursos.API, src/Cursos.Application, etc.

Write-Host "`nCleanup complete!" -ForegroundColor Green
Write-Host "New structure:" -ForegroundColor Cyan
Write-Host "  src/Cursos.Domain/"
Write-Host "  src/Cursos.Application/"
Write-Host "  src/Cursos.Infrastructure/"
Write-Host "  src/Cursos.API/"
Write-Host "`nRun 'dotnet build Cursos.Architecture.sln' to verify everything works." -ForegroundColor Yellow
