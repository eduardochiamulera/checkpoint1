# Cursos - Clean Architecture

> API de cursos com pagamentos, construida com Clean Architecture e .NET 9

## 🏗️ Arquitetura

Este projeto utiliza **Clean Architecture** com as seguintes camadas:

```
Cursos.Architecture.sln
├── src/
│   ├── Cursos.Domain/          # Entidades, Value Objects, Interfaces
│   ├── Cursos.Application/     # Use Cases, DTOs, Handlers (MediatR)
│   ├── Cursos.Infrastructure/  # EF Core, Repositorios, Gateways
│   └── Cursos.API/             # Controllers, Middleware, Logging
└── tests/                      # Testes (futuro)
```

### Regras de Dependencia

```
API → Application → Domain
Infrastructure → Domain
```

- **Domain**: Zero dependencias externas
- **Application**: Depende apenas de Domain + MediatR
- **Infrastructure**: Implementa interfaces de Domain
- **API**: Orquestra tudo via DI

## 🚀 Como Executar

### Pre-requisitos
- .NET 9 SDK
- SQL Server (ou use Docker)

### 1. Clone o Repositorio

```bash
git clone https://github.com/eduardochiamulera/checkpoint1.git
cd checkpoint1
```

### 2. Configure o Banco de Dados

Edite `src/Cursos.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Cursos;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Execute a API

```bash
cd src/Cursos.API
dotnet run
```

A API vai:
- ✅ Aplicar migrations automaticamente
- ✅ Seedar dados iniciais (3 cursos, 3 estudantes, 3 enrollments)
- ✅ Iniciar em http://localhost:5000

### 4. Acesse o Swagger

http://localhost:5000/swagger

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
- `GET /health` - Health check completo (API + DB)
- `GET /health/ready` - Apenas database (readiness)

## 🧪 Testando com curl

### Login
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "123456"}'
```

### Criar Curso
```bash
curl -X POST http://localhost:5000/api/courses \
  -H "Content-Type: application/json" \
  -d '{
    "name": "ASP.NET Core",
    "description": "Curso de ASP.NET Core",
    "price": 299.90,
    "instructor": "John Doe",
    "durationHours": 20
  }'
```

### Processar Pagamento
```bash
curl -X POST http://localhost:5000/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "enrollmentId": "00000000-0000-0000-0000-000000000000",
    "amount": 100.00,
    "paymentMethodType": "CreditCard"
  }'
```

### Health Check
```bash
curl http://localhost:5000/health
```

## 📝 Logging Estruturado

### Ativar Logs

**Desenvolvimento** (`appsettings.Development.json`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Cursos": "Debug"
    }
  }
}
```

**Producao** (`appsettings.json`):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Cursos": "Information"
    }
  }
}
```

### Exemplo de Log

```
[Info] [a1b2c3d4] POST /api/payments started - User: test@example.com
[Info] [a1b2c3d4] Processing payment for enrollment 123e4567, amount 100.00
[Info] [a1b2c3d4] Payment 987fcdeb confirmed successfully, transaction sim_abc123
[Info] [a1b2c3d4] POST /api/payments completed with status 200 in 145ms
```

### CorrelationId

O middleware adiciona automaticamente:
- **X-Correlation-ID** no header da resposta
- **UserId** (email) se autenticado
- **Method/Path** da requisicao
- **StatusCode** e **ElapsedMs** da resposta

### 🔒 Seguranca

**Dados sensveis NUNCA sao logados:**
- ❌ Senhas
- ❌ Tokens JWT completos
- ❌ Numeros de cartao de credito
- ❌ CPF/CNPJ

**Sempre logados:**
- ✅ IDs (PaymentId, UserId, EnrollmentId)
- ✅ Emails
- ✅ Status de operacoes
- ✅ Timestamps

📖 **Guia completo**: [LOGGING_GUIDE.md](./LOGGING_GUIDE.md)

## 📦 Padroes e Tecnologias

| Padrao | Tecnologia |
|--------|------------|
| **Clean Architecture** | .NET 9 |
| **Mediator** | MediatR 12.4.1 |
| **ORM** | EF Core 9.0 |
| **Banco** | SQL Server |
| **API** | ASP.NET Core |
| **Autenticacao** | JWT Bearer |
| **Health Checks** | ASP.NET Core Health |
| **Documentacao** | Swagger/OpenAPI |

## 🎯 Padroes de Design Aplicados

- ✅ **Repository** - Abstracao da persistencia
- ✅ **Unit of Work** - Gerenciamento de transacoes
- ✅ **Strategy** - Troca de gateway de pagamento
- ✅ **Mediator** - Desacoplamento de handlers
- ✅ **Aggregate Root** - Payment com invariantes
- ✅ **Value Object** - Money imutavel
- ✅ **Command/Query** - Segregacao CQRS
- ✅ **Middleware** - Logging estruturado

## 📚 Documentacao

- 📄 [README.md](./README.md) - Este arquivo
- 📄 [LOGGING_GUIDE.md](./LOGGING_GUIDE.md) - Guia de logging estruturado
- 📄 [JWT_AUTH_GUIDE.md](./JWT_AUTH_GUIDE.md) - Guia de autenticacao JWT
- 📄 [ARCHITECTURE.md](./ARCHITECTURE.md) - Decision log e arquitetura
- 📄 [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) - Guia de migracao
- 📄 [MIGRATION_COMPLETE.md](./MIGRATION_COMPLETE.md) - Status da migracao
- 📄 [COMPLIANCE_REPORT.md](./COMPLIANCE_REPORT.md) - Relatorio de compliance

## ⚠️ Notas Importantes

1. **Autenticacao**: JWT configurado, mas use HTTPS em producao
2. **Banco**: Configure a connection string no `appsettings.json`
3. **Producao**: Desative auto-migration em producao
4. **Logs**: Configure nivel apropriado para cada ambiente
5. **Seguranca**: Nunca logue dados sensiveis

## 🔧 Scripts Uteis

### Limpar estrutura antiga (apos merge)
```bash
# Windows
powershell -ExecutionPolicy Bypass -File cleanup-old-structure-onetime.ps1

# Linux/Mac
./cleanup-old-structure.sh
```

### Build
```bash
dotnet build Cursos.Architecture.sln
```

### Testes (futuro)
```bash
dotnet test
```

## 📖 Referencias

- [Clean Architecture - Robert C. Martin](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164)
- [Domain-Driven Design - Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Health Checks in .NET](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/logging)

## 📄 License

MIT License - veja [LICENSE](./LICENSE)

---

**Status**: ✅ Producao-Ready  
**Ultima Atualizacao**: Agosto 2026  
**Versao**: 2.1.0 (Logging + Health Checks)
