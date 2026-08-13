# Cursos - Clean Architecture

## 🏗️ Estrutura do Projeto

```
Cursos.Architecture/
├── src/
│   ├── Cursos.Domain/          # Entidades, Value Objects, Interfaces
│   ├── Cursos.Application/     # Use Cases, DTOs, Handlers (MediatR)
│   ├── Cursos.Infrastructure/  # EF Core, Repositorios, Gateways
│   └── Cursos.API/             # Controllers, Middleware
└── tests/
    ├── Cursos.Domain.Tests/
    ├── Cursos.Application.Tests/
    └── Cursos.Integration.Tests/
```

## 📐 Regras de Dependencia

```
API → Application → Domain
Infrastructure → Domain
```

- **Domain**: Zero dependencias de outros projetos
- **Application**: Depende apenas de Domain + MediatR
- **Infrastructure**: Implementa interfaces de Domain
- **API**: Orquestra Application + Infrastructure via DI

## 🎯 Padroes Aplicados

| Padrao | Localizacao | Proposito |
|--------|-------------|-----------|
| Repository | Domain/Interfaces | Abstrair persistencia |
| Unit of Work | Domain/Interfaces | Transacoes |
| Strategy | Infrastructure/Gateways | Troca de gateway de pagamento |
| Command/Query | Application | Separacao de leitura/escrita |
| Mediator | Application | Desacoplamento de handlers |
| Aggregate Root | Domain/Payments | Garantir invariantes de dominio |
| Value Object | Domain/Payments | Money como valor imutavel |

## 🚀 Como Executar

```bash
# Build
dotnet build Cursos.Architecture.sln

# Run API
cd src/Cursos.API
dotnet run

# Testes
dotnet test
```

## 📝 Checklist de PR

- [ ] Nova regra de negocio esta em Domain (nao em Controller)
- [ ] Use Case criado em Application com Handler
- [ ] Repository/Gateway implementado em Infrastructure
- [ ] DTOs para entrada/saida (nao expor entidades)
- [ ] Tratamento de erro com ProblemDetails
- [ ] Logs estruturados (correlationId, userId)
- [ ] Testes de unidade para Domain
- [ ] Nullable enable em todos os projetos

## 📊 Decision Log

| Data | Decisao | Motivo |
|------|---------|--------|
| 2026-08-13 | Clean Architecture | Testabilidade, baixo acoplamento |
| 2026-08-13 | MediatR para Use Cases | Separacao clara de responsabilidades |
| 2026-08-13 | Strategy Pattern para gateways | Troca de provider sem mexer no Domain |
| 2026-08-13 | Domain Events | Notificar mudancas de estado sem acoplamento |

## 🔑 Principais Conceitos

### Domain Layer
- **Agregados**: `Payment` é um agregado raiz que garante invariantes
- **Value Objects**: `Money` é imutavel e define igualdade por valor
- **Interfaces**: `IPaymentRepository`, `IPaymentGateway`, `IUnitOfWork`
- **Excecoes de Dominio**: `DomainException` para regras de negocio violadas

### Application Layer
- **Commands**: Operacoes de escrita (ex: `ProcessPaymentCommand`)
- **Queries**: Operacoes de leitura (ex: `GetPaymentByEnrollmentQuery`)
- **Handlers**: Implementacao da logica de aplicacao
- **DTOs**: Objetos de transferencia de dados (nao expoe entidades)

### Infrastructure Layer
- **EF Core**: Mapeamento das entidades para banco de dados
- **Repositorios**: Implementacao das interfaces de Domain
- **Gateways**: Implementacao de servicos externos (pagamento, email, etc)
- **DI**: Configuracao de injecao de dependencia

### API Layer
- **Controllers**: Recebem HTTP requests e chamam MediatR
- **Middleware**: Tratamento global de excecoes, logging, CORS
- **Swagger**: Documentacao automatica da API

## 🛡️ Boas Praticas

1. **Domain Anemico**: Evitar! Regras de negocio ficam em Agregados/Servicos de Dominio
2. **Nao expor entidades**: Usar sempre DTOs em entrada/saida
3. **Interfaces pequenas (ISP)**: Separar contratos por responsabilidade
4. **Idempotencia**: Validar antes de criar pagamentos duplicados
5. **Logs estruturados**: Incluir correlationId, userId, rota, status
6. **Testabilidade**: Testes de unidade no Domain, integracao para Repositorios/API

## 📦 NuGet Packages

- `MediatR` (12.4.1) - Pattern Mediator para Commands/Queries
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.0) - ORM
- `Swashbuckle.AspNetCore` (6.9.0) - Swagger/OpenAPI

## 🔗 Referencias

- [Domain-Driven Design - Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [Clean Architecture - Robert C. Martin](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
