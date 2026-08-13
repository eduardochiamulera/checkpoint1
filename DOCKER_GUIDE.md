# 🐳 Guia de Docker e Containers

## Visao Geral

Este projeto esta pronto para rodar via Docker, com build multi-stage otimizado e `docker-compose` orquestrando API + banco de dados MySQL.

## 📦 Estrutura

```
Dockerfile              # Build multi-stage: restore -> build -> test -> publish -> runtime
docker-compose.yml      # Orquestra API + MySQL
.env.example            # Template de variaveis de ambiente (copie para .env)
.dockerignore           # Arquivos excluidos do contexto de build
```

## 🏗️ Etapas do Dockerfile (Multi-Stage)

| Estagio | Imagem Base | Proposito |
|---------|-------------|-----------|
| **restore** | `dotnet/sdk:9.0` | Restaura pacotes NuGet (cacheavel entre builds) |
| **build** | herda de `restore` | Compila a solucao inteira |
| **test** | herda de `build` | Executa os testes automatizados |
| **publish** | herda de `build` | Publica somente o projeto `Cursos.API` |
| **runtime** | `dotnet/aspnet:9.0` | Imagem final minima, sem SDK, roda como usuario nao-root |

A imagem final (`runtime`) nao contem o SDK do .NET nem codigo-fonte, apenas os binarios publicados — reduzindo tamanho e superficie de ataque.

## 🚀 Como Rodar Localmente

### 1. Configurar variaveis de ambiente

```bash
cp .env.example .env
```

Edite o `.env` e defina senhas fortes:

```bash
MYSQL_PASSWORD=SuaSenhaForte123!
MYSQL_ROOT_PASSWORD=SuaSenhaRootForte123!
JWT_SECRET_KEY=$(openssl rand -base64 32)
```

> ⚠️ **O arquivo `.env` nunca deve ser commitado.** Ele ja esta no `.gitignore`.

### 2. Subir os containers

```bash
docker compose up -d --build
```

Isso vai:
- Construir a imagem da API (multi-stage)
- Subir o MySQL 8.0 com volume persistente
- Aguardar o MySQL ficar saudavel antes de iniciar a API (via `depends_on` + `healthcheck`)

### 3. Verificar status

```bash
docker compose ps
docker compose logs -f api
```

### 4. Testar a API

```bash
curl http://localhost:8080/health
curl http://localhost:8080/swagger
```

### 5. Parar os containers

```bash
# Mantem os dados do banco
docker compose down

# Remove tambem o volume do banco (apaga todos os dados!)
docker compose down -v
```

## 🔍 Comandos Uteis

```bash
# Rebuild apenas a API
docker compose build api

# Ver logs de um servico especifico
docker compose logs -f api
docker compose logs -f db

# Executar comando dentro do container da API
docker compose exec api sh

# Conectar no MySQL via container
docker compose exec db mysql -u root -p

# Ver uso de recursos
docker stats
```

## 🔐 Variaveis de Ambiente (.env)

| Variavel | Descricao | Padrao |
|----------|-----------|--------|
| `API_PORT` | Porta exposta da API no host | `8080` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicacao | `Production` |
| `MYSQL_PORT` | Porta exposta do MySQL no host | `3306` |
| `MYSQL_DATABASE` | Nome do banco | `cursos` |
| `MYSQL_USER` | Usuario da aplicacao | `cursos_app` |
| `MYSQL_PASSWORD` | Senha do usuario da aplicacao | **obrigatorio** |
| `MYSQL_ROOT_PASSWORD` | Senha do root do MySQL | **obrigatorio** |
| `JWT_SECRET_KEY` | Chave secreta do JWT (min 32 chars) | **obrigatorio** |
| `JWT_ISSUER` | Emissor do token | `CursosAPI` |
| `JWT_AUDIENCE` | Audiencia do token | `CursosUsers` |
| `JWT_EXPIRATION_MINUTES` | Expiracao do token em minutos | `60` |
| `PAYMENT_GATEWAY_TYPE` | Gateway de pagamento (`Simulated`/`Stripe`/`PayPal`) | `Simulated` |

## 🏥 Health Checks nos Containers

Ambos os servicos tem `healthcheck` configurado:

- **api**: `wget --spider http://localhost:8080/health`
- **db**: `mysqladmin ping`

Isso garante que a API so inicia depois que o banco estiver realmente pronto (`depends_on: condition: service_healthy`).

## 🏗️ Build Manual (sem compose)

```bash
# Build da imagem
docker build -t cursos-api:local .

# Rodar standalone (precisa de um MySQL acessivel)
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=cursos;User Id=root;Password=changeme;" \
  -e JwtSettings__SecretKey="changeme_super_secret_key_at_least_32_chars" \
  --name cursos-api \
  cursos-api:local
```

## 🔄 CI/CD - Build e Push Automatico

O pipeline (`.github/workflows/ci-cd.yml`) builda e publica a imagem automaticamente em push para `main`:

```
ghcr.io/eduardochiamulera/checkpoint1:latest
ghcr.io/eduardochiamulera/checkpoint1:<sha-curto>
```

Para baixar a imagem publicada:

```bash
docker pull ghcr.io/eduardochiamulera/checkpoint1:latest
```

---

**Status**: ✅ Pronto para uso  
**Ultima Atualizacao**: Agosto 2026
