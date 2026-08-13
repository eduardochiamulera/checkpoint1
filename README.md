# Cursos - Clean Architecture

[![CI/CD](https://github.com/eduardochiamulera/checkpoint1/actions/workflows/ci-cd.yml/badge.svg?branch=main)](https://github.com/eduardochiamulera/checkpoint1/actions/workflows/ci-cd.yml)

> API de cursos com pagamentos, construida com Clean Architecture, .NET 9 e MySQL

## 🏗️ Arquitetura

Este projeto utiliza **Clean Architecture** com as seguintes camadas:

```
Cursos.Architecture.sln
├── src/
│   ├── Cursos.Domain/          # Entidades, Value Objects, Interfaces
│   ├── Cursos.Application/     # Use Cases, DTOs, Handlers (MediatR)
│   ├── Cursos.Infrastructure/  # EF Core (MySQL), Repositorios, Gateways
│   └── Cursos.API/             # Controllers, Middleware, Logging
└── tests/                      # Testes automatizados
```

### Regras de Dependencia

```
API → Application → Domain
Infrastructure → Domain
```

- **Domain**: Zero dependencias externas
- **Application**: Depende apenas de Domain + MediatR
- **Infrastructure**: Implementa interfaces de Domain (EF Core/MySQL, JWT, hashing)
- **API**: Orquestra tudo via DI

Detalhes completos: [ARCHITECTURE.md](./ARCHITECTURE.md) | [COMPLIANCE_REPORT.md](./COMPLIANCE_REPORT.md)

## 💳 Pagamentos

O fluxo de pagamento segue o padrao Aggregate Root + Strategy:

- **`Payment`** (agregado): controla o ciclo de vida via `Confirm()`, `Cancel()`, `Refund()`, validando transicoes com `PaymentRules`
- **`Money`** (value object): valor imutavel, valida quantia negativa
- **`IPaymentGateway`** (Strategy/Adapter): permite trocar o provedor de pagamento sem alterar o Domain; implementacao atual: `SimulatedPaymentGateway`
- **Idempotencia**: `ProcessPaymentHandler` verifica pagamento ja confirmado para a mesma enrollment antes de criar um novo

```bash
curl -X POST http://localhost:8080/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "enrollmentId": "00000000-0000-0000-0000-000000000000",
    "amount": 100.00,
    "paymentMethodType": "CreditCard"
  }'
```

## 🧪 Testes

```bash
# Todos os testes
dotnet test tests/Cursos.Tests.csproj

# Com relatorio de resultados
dotnet test tests/Cursos.Tests.csproj --logger "trx;LogFileName=test-results.trx" --results-directory ./TestResults
```

Estrutura de testes (`tests/`):
- `Domain/` - Testes de unidade das regras de negocio (Payment, Money, PaymentRules)
- `Integration/` - Testes de integracao (API, repositorios)
- `Fixtures/` - Builders e dados de teste (padrao AAA: Arrange, Act, Assert)

O pipeline de CI executa os testes automaticamente em todo Pull Request e push para `main`.

## 🚀 Como Executar

### Opcao 1: Docker (recomendado)

```bash
cp .env.example .env
# edite o .env com senhas/segredos reais
docker compose up -d --build
curl http://localhost:8080/health
```

Guia completo: [DOCKER_GUIDE.md](./DOCKER_GUIDE.md)

### Opcao 2: Local (.NET SDK)

**Pre-requisitos**: .NET 9 SDK, MySQL 8.0

```bash
git clone https://github.com/eduardochiamulera/checkpoint1.git
cd checkpoint1
```

Edite `src/Cursos.API/appsettings.json` com sua connection string MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=cursos;User Id=root;Password=yourpassword;"
  }
}
```

```bash
cd src/Cursos.API
dotnet run
```

A API vai aplicar migrations e seedar dados automaticamente, iniciando em `http://localhost:5000`.

### Acesse o Swagger

http://localhost:5000/swagger (local) ou http://localhost:8080/swagger (Docker)

## 📊 Endpoints Disponiveis

### Auth
- `POST /api/auth/register` - Registro de usuario
- `POST /api/auth/login` - Login (retorna JWT)
- `GET /api/auth/me` - Dados do usuario atual (protegido)

### Payments
- `POST /api/payments` - Processar pagamento
- `GET /api/payments/enrollment/{id}` - Buscar pagamento

### Courses
- `GET /api/courses` - Listar cursos
- `GET /api/courses/{id}` - Buscar curso
- `POST /api/courses` - Criar curso

### Students
- `GET /api/students` - Listar estudantes
- `GET /api/students/{id}` - Buscar estudante
- `POST /api/students` - Criar estudante
- `PUT /api/students/{id}` - Atualizar estudante
- `DELETE /api/students/{id}` - Deletar estudante

### Enrollments
- `GET /api/enrollments/student/{id}` - Listar por estudante
- `GET /api/enrollments/course/{id}` - Listar por curso
- `POST /api/enrollments` - Criar enrollment
- `POST /api/enrollments/{id}/complete` - Completar
- `POST /api/enrollments/{id}/cancel` - Cancelar

### Health Checks
- `GET /health` - Health check completo (API + MySQL)
- `GET /health/ready` - Apenas database (readiness)

## 📝 Logging Estruturado e Observabilidade

### Ativar Logs

**Desenvolvimento** (`appsettings.Development.json`):
```json
{ "Logging": { "LogLevel": { "Default": "Information", "Cursos": "Debug" } } }
```

**Producao** (`appsettings.json`):
```json
{ "Logging": { "LogLevel": { "Default": "Warning", "Cursos": "Information" } } }
```

### Amostra de Log (fluxo de pagamento)

```
[Info] [a1b2c3d4] POST /api/payments started - User: test@example.com
[Info] [a1b2c3d4] Processing payment for enrollment 123e4567, amount 100.00
[Warn] [a1b2c3d4] Payment already confirmed for enrollment 123e4567, transaction sim_abc123
[Error][a1b2c3d4] Payment gateway failed for payment 987fcdeb. Error: Simulated gateway failure
[Info] [a1b2c3d4] Payment 987fcdeb confirmed successfully, transaction sim_abc123
[Info] [a1b2c3d4] POST /api/payments completed with status 200 in 145ms
```

O `LoggingMiddleware` adiciona automaticamente `X-Correlation-ID` no header de resposta, junto com usuario (email), rota e status.

### Consultar Health

```bash
curl http://localhost:8080/health          # API + MySQL
curl http://localhost:8080/health/ready    # Apenas MySQL (readiness)
```

📖 **Guia completo de logging**: [LOGGING_GUIDE.md](./LOGGING_GUIDE.md)  
📖 **Guia de autenticacao JWT**: [JWT_AUTH_GUIDE.md](./JWT_AUTH_GUIDE.md)

### 🔒 Dados Sensiveis

**Nunca logados**: senhas, tokens JWT completos, numeros de cartao, CPF/CNPJ  
**Sempre logados**: IDs, emails, status de operacoes, timestamps

## 🐳 Container e Pipeline

### Docker

- `Dockerfile` multi-stage: `restore → build → test → publish → runtime`
- `docker-compose.yml`: API + MySQL com healthchecks e volume persistente
- Imagem final minima (`aspnet:9.0`), sem SDK, roda como usuario nao-root

```bash
cp .env.example .env
docker compose up -d --build
```

Guia completo: [DOCKER_GUIDE.md](./DOCKER_GUIDE.md)

### CI/CD (GitHub Actions)

Pipeline (`.github/workflows/ci-cd.yml`) disparado em **Pull Request** e **push para `main`**:

```
restore → build → testes → publicar artefatos → build/push imagem → smoke test
```

| Etapa | O que faz |
|-------|-----------|
| Restore + Build | `dotnet restore` + `dotnet build` da solucao |
| Testes | `dotnet test` com publicacao de resultados (`.trx`) |
| Publicar artefatos | `dotnet publish` da API, disponivel como artifact do workflow |
| Build/Push imagem | Builda e envia imagem Docker para `ghcr.io` (apenas em push para `main`) |
| Smoke test | Sobe `docker compose`, aguarda `/health` responder e testa endpoint critico |

Os segredos usados no pipeline (senhas de smoke test, chaves JWT de teste) ficam configurados em **GitHub Secrets** (Settings → Secrets and variables → Actions), nunca no repositorio.

## ⚠️ Checklist de Erros Comuns

| Sintoma | Causa provavel | Solucao rapida |
|---------|-----------------|----------------|
| `Unable to resolve service for type 'IPaymentRepository'` | Servico nao registrado no DI | Verifique `AddInfrastructure()` em `Cursos.Infrastructure/DependencyInjection.cs` |
| `The type or namespace name 'X' could not be found` | Falta `using` ou pacote NuGet | Confirme o pacote no `.csproj` do projeto correto e rode `dotnet restore` |
| Build falha apos editar `Program.cs` de Health Checks | Namespace errado (`Microsoft.AspNetCore.HealthChecks` nao existe) | Use `Microsoft.Extensions.Diagnostics.HealthChecks` (HealthCheckResult/HealthStatus) e `Microsoft.AspNetCore.Diagnostics.HealthChecks` (HealthCheckOptions) |
| `AddSqlServer`/`AddMySql` nao encontrado | Pacote de health check nao instalado ou trocado incorretamente | Confirme `AspNetCore.HealthChecks.MySql` no `Cursos.API.csproj` (o banco deste projeto e MySQL, nao SQL Server) |
| Migrations nao aplicam / tabela ja existe | Banco desatualizado ou migration duplicada | Apague o banco de dev e rode novamente, ou `dotnet ef database update` |
| `MYSQL_PASSWORD is required` no `docker compose up` | Arquivo `.env` nao criado | `cp .env.example .env` e preencha as variaveis obrigatorias |
| API nao conecta no banco dentro do Docker | Connection string usando `localhost` em vez do nome do servico | Use `Server=db;...` (nome do servico no `docker-compose.yml`), nunca `localhost` |
| `401 Unauthorized` em endpoints protegidos | Token expirado ou `JwtSettings:SecretKey` diferente entre geracao/validacao | Gere novo login e confirme que `JWT_SECRET_KEY` e igual em todos os ambientes |
| PowerShell bloqueia `.ps1` ("running scripts is disabled") | Execution Policy do Windows | Rode com `powershell -ExecutionPolicy Bypass -File script.ps1` |

## 🎯 Padroes de Design Aplicados

- ✅ **Repository** - Abstracao da persistencia
- ✅ **Unit of Work** - Gerenciamento de transacoes
- ✅ **Strategy** - Troca de gateway de pagamento
- ✅ **Mediator** - Desacoplamento de handlers
- ✅ **Aggregate Root** - Payment com invariantes
- ✅ **Value Object** - Money imutavel
- ✅ **Command/Query** - Segregacao CQRS
- ✅ **Middleware** - Logging estruturado

## 📦 Padroes e Tecnologias

| Padrao | Tecnologia |
|--------|------------|
| **Clean Architecture** | .NET 9 |
| **Mediator** | MediatR 12.4.1 |
| **ORM** | EF Core 9.0 (Pomelo.EntityFrameworkCore.MySql) |
| **Banco** | MySQL 8.0 |
| **API** | ASP.NET Core |
| **Autenticacao** | JWT Bearer |
| **Health Checks** | ASP.NET Core Health + AspNetCore.HealthChecks.MySql |
| **Documentacao** | Swagger/OpenAPI |
| **Container** | Docker multi-stage + docker-compose |
| **CI/CD** | GitHub Actions |

## 📚 Documentacao

- 📄 [README.md](./README.md) - Este arquivo
- 📄 [DOCKER_GUIDE.md](./DOCKER_GUIDE.md) - Guia de Docker e containers
- 📄 [LOGGING_GUIDE.md](./LOGGING_GUIDE.md) - Guia de logging estruturado
- 📄 [JWT_AUTH_GUIDE.md](./JWT_AUTH_GUIDE.md) - Guia de autenticacao JWT
- 📄 [ARCHITECTURE.md](./ARCHITECTURE.md) - Decision log e arquitetura
- 📄 [CHANGELOG.md](./CHANGELOG.md) - Historico de mudancas por fase
- 📄 [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) - Guia de migracao
- 📄 [MIGRATION_COMPLETE.md](./MIGRATION_COMPLETE.md) - Status da migracao
- 📄 [COMPLIANCE_REPORT.md](./COMPLIANCE_REPORT.md) - Relatorio de compliance

## ⚠️ Notas Importantes

1. **Autenticacao**: JWT configurado, mas use HTTPS em producao
2. **Banco**: MySQL — configure a connection string no `appsettings.json` ou `.env`
3. **Producao**: Desative auto-migration em producao (o Docker/CI ja assume ambiente controlado)
4. **Logs**: Configure nivel apropriado para cada ambiente
5. **Seguranca**: Nunca logue dados sensiveis nem faca commit do `.env`

## 🔧 Scripts Uteis

```bash
# Build local
dotnet build Cursos.Architecture.sln

# Testes
dotnet test tests/Cursos.Tests.csproj

# Docker
cp .env.example .env
docker compose up -d --build

# Limpar estrutura antiga (apos merge, se aplicavel)
powershell -ExecutionPolicy Bypass -File cleanup-old-structure-onetime.ps1   # Windows
./cleanup-old-structure.sh                                                   # Linux/Mac
```

## 📖 Referencias

- [Clean Architecture - Robert C. Martin](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164)
- [Domain-Driven Design - Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Health Checks in .NET](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/logging)
- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)

## 📄 License

MIT License - veja [LICENSE](./LICENSE)

---

**Status**: ✅ Producao-Ready  
**Ultima Atualizacao**: Agosto 2026  
**Versao**: 2.2.0 (Docker + CI/CD + Observabilidade)
