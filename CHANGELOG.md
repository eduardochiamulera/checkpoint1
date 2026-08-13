# Changelog

Todas as mudancas notaveis deste projeto sao documentadas neste arquivo.

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/).

## [2.2.0] - Fase 2 - 2026-08-13

### Adicionado

#### Containerizacao
- `Dockerfile` multi-stage (restore → build → test → publish → runtime) com imagem final minima baseada em `aspnet:9.0` e usuario nao-root
- `docker-compose.yml` com servicos `api` (Cursos API) e `db` (MySQL 8.0), healthchecks e volume persistente
- `.env.example` com todas as variaveis necessarias documentadas (sem segredos reais)
- `.dockerignore` para reduzir contexto de build e evitar vazamento de arquivos sensiveis

#### CI/CD
- Pipeline `ci-cd.yml` (GitHub Actions) com gatilhos em Pull Request e push para `main`
- Etapas: `restore → build → testes → publicar artefatos → build/push imagem → smoke test`
- Publicacao de artefatos de teste (`.trx`) e artefato publicado da API
- Build e push de imagem Docker para GitHub Container Registry (`ghcr.io`) apenas em push para `main`
- Smoke test automatizado subindo `docker compose` e validando `/health` e `/health/ready`
- Segredos do pipeline configurados via GitHub Secrets (nunca no repositorio)

#### Observabilidade
- Logging estruturado com correlationId, usuario, rota e status em pontos criticos (criacao/confirmacao de pagamento, falhas de gateway)
- Health checks expostos em `/health` (API + MySQL) e `/health/ready` (apenas banco)

#### Autenticacao
- Autenticacao JWT completa (registro, login, hash de senha PBKDF2, geracao de token)

### Corrigido
- Health check migrado de SQL Server para MySQL (`AddMySql`)
- Usings ausentes que quebravam o build: `System.IdentityModel.Tokens.Jwt` no Infrastructure, `MediatR` e `Cursos.Application.Auth` nos handlers/commands/controller de autenticacao
- Namespaces corretos de Health Checks no `Program.cs` (`Microsoft.Extensions.Diagnostics.HealthChecks` e `Microsoft.AspNetCore.Diagnostics.HealthChecks`)

### Documentacao
- `README.md` atualizado com secoes de pagamentos, testes, arquitetura, observabilidade, container e pipeline
- `LOGGING_GUIDE.md` com exemplos de log e boas praticas
- `JWT_AUTH_GUIDE.md` com fluxo completo de autenticacao
- `DOCKER_GUIDE.md` com instrucoes de uso local via Docker
- Badge de status do pipeline no README
- Checklist de erros comuns (build/test/migrations/variaveis) com solucoes rapidas

---

## [2.1.0] - 2026-08-13

### Adicionado
- Logging estruturado (`LoggingMiddleware`) com correlationId
- Health checks basicos

## [2.0.0] - 2026-08-13

### Adicionado
- Migracao completa para Clean Architecture (Domain, Application, Infrastructure, API)
- MediatR para Use Cases (Commands/Queries)
- Repository Pattern, Unit of Work, Strategy Pattern (gateway de pagamento)
- Autenticacao JWT (User entity, PasswordHasher, JwtTokenGenerator)

### Removido
- Estrutura monolitica antiga (`Controllers`, `Services`, `Models`, `Data`, `Domains` na raiz de `src/`)
