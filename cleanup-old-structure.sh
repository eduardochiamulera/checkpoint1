#!/bin/bash
# Bash script to remove old project structure
# Run this from the repository root after merging feature/clean-architecture to main

echo "Starting cleanup of old project structure..."

# Remove old folders
old_paths=(
    "src/Controllers"
    "src/Data"
    "src/Domains"
    "src/Exceptions"
    "src/Migrations"
    "src/Models"
    "src/Services"
    "src/Properties"
    "src/docs"
    "src/.gitignore"
    "src/CHANGELOG.md"
    "src/Cursos.csproj"
    "src/Cursos.http"
    "src/Program.cs"
    "src/appsettings.json"
    "src/appsettings.Development.json"
    "src/appsettings.example.json"
)

for path in "${old_paths[@]}"; do
    if [ -d "$path" ] || [ -f "$path" ]; then
        rm -rf "$path"
        echo "✓ Removed: $path"
    else
        echo "- Not found (already removed?): $path"
    fi
done

echo ""
echo "Cleanup complete!"
echo "New structure:"
echo "  src/Cursos.Domain/"
echo "  src/Cursos.Application/"
echo "  src/Cursos.Infrastructure/"
echo "  src/Cursos.API/"
echo ""
echo "Run 'dotnet build Cursos.Architecture.sln' to verify everything works."
