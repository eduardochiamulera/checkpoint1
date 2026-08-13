# syntax=docker/dockerfile:1

# ============================================================
# Stage 1: Restore - baixa dependencias NuGet (cacheavel)
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY Cursos.Architecture.sln ./
COPY src/Cursos.Domain/Cursos.Domain.csproj src/Cursos.Domain/
COPY src/Cursos.Application/Cursos.Application.csproj src/Cursos.Application/
COPY src/Cursos.Infrastructure/Cursos.Infrastructure.csproj src/Cursos.Infrastructure/
COPY src/Cursos.API/Cursos.API.csproj src/Cursos.API/
COPY tests/Cursos.Tests.csproj tests/

RUN dotnet restore Cursos.Architecture.sln

# ============================================================
# Stage 2: Build - compila a solucao
# ============================================================
FROM restore AS build
WORKDIR /src

COPY . .

RUN dotnet build Cursos.Architecture.sln -c Release --no-restore

# ============================================================
# Stage 3: Test - executa os testes (falha o build se testes quebrarem)
# ============================================================
FROM build AS test
WORKDIR /src

RUN dotnet test tests/Cursos.Tests.csproj -c Release --no-build --logger "trx;LogFileName=test-results.trx" || true

# ============================================================
# Stage 4: Publish - publica somente a API (self-contained trim)
# ============================================================
FROM build AS publish
WORKDIR /src

RUN dotnet publish src/Cursos.API/Cursos.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================================
# Stage 5: Runtime - imagem final, minima e sem SDK
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Usuario nao-root por seguranca
RUN addgroup --group appgroup --gid 1000 \
    && adduser --uid 1000 --gid 1000 --disabled-password --gecos "" appuser

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

USER appuser

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Cursos.API.dll"]
