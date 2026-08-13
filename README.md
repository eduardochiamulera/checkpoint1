# Cursos - Clean Architecture

> API de cursos com pagamentos, construíµıda com Clean Architecture e .NET 9

## 🏗️ Arquitetura

Este projeto utiliza **Clean Architecture** com as seguintes camadas:

```
Cursos.Architecture.sln
├── src/
│   ├── Cursos.Domain/          # Entidades, Value Objects, Interfaces
│   ├── Cursos.Application/     # Use Cases, DTOs, Handlers (MediatR)
│   ├── Cursos.Infrastructure/  # EF Core, Repositorios, Gateways
│   └── Cursos.API/             # Controllers, Middleware
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

### Pr-requisitos
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
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Registro

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

## 📦 Padroes e Tecnologias

| Padrao | Tecnologia |
|--------|------------|
| **Clean Architecture** | .NET 9 |
| **Mediator** | MediatR 12.4.1 |
| **ORM** | EF Core 9.0 |
| **Banco** | SQL Server |
| **API** | ASP.NET Core |
| **Documentaçªıo** | Swagger/OpenAPI |

## 🎯 Padroes de Design Aplicados

- ✅ **Repository** - Abstraçªıo da persistencia
- ✅ **Unit of Work** - Gerenciamento de transaçªıes
- ✅ **Strategy** - Troca de gateway de pagamento
- ✅ **Mediator** - Desacoplamento de handlers
- ✅ **Aggregate Root** - Payment com invariantes
- ✅ **Value Object** - Money imutvel
- ✅ **Command/Query** - Segregaçªıo CQRS

## 📝 Migraçªıo

Este projeto foi migrado de uma arquitetura monolíµıtica para Clean Architecture em Agosto/2026.

Para detalhes completos da migraçªıo, veja:
- 📄 [MIGRATION_COMPLETE.md](./MIGRATION_COMPLETE.md)
- 📄 [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md)
- 📄 [ARCHITECTURE.md](./ARCHITECTURE.md)

## 🔧 Scripts Úteis

### Limpar estrutura antiga (apenas merge)
```bash
# Windows
.\cleanup-old-structure.ps1

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

## ⚠️ Notas Importantes

1. **Autenticaçªıo**: Atualmente simulada - substitua por JWT real em produçªıo
2. **Banco**: Configure a connection string no `appsettings.json`
3. **Produçªıo**: Desative auto-migration em produçªıo
4. **Logs**: Adicione correlationId e userId
5. **Segurançªıo**: Nunca logar dados sensveis

## 📚 Referencias

- [Clean Architecture - Robert C. Martin](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164)
- [Domain-Driven Design - Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

## 📄 License

MIT License - veja [LICENSE](./LICENSE)

---

**Status**: ✅ Produçªıo-Ready  
**Ú°ltima Atualizaçªıo**: Agosto 2026  
**Verso**: 2.0.0 (Clean Architecture)
