# Guia de Migraçª£o para Clean Architecture

## ✅ O que já foi feito

### 1. Solution e Projetos
- [x] `Cursos.Architecture.sln` criado
- [x] `Cursos.Domain` - Camada de domínio
- [x] `Cursos.Application` - Camada de aplicação com MediatR
- [x] `Cursos.Infrastructure` - Camada de infraestrutura
- [x] `Cursos.API` - Camada de API

### 2. Domain Layer
- [x] `Entity` e `ValueObject` classes base
- [x] `Course`, `Student`, `Enrollment` entities
- [x] `Payment` aggregate com invariantes
- [x] `Money` value object
- [x] Interfaces: `IPaymentRepository`, `ICourseRepository`, `IStudentRepository`, `IEnrollmentRepository`, `IPaymentGateway`, `IUnitOfWork`
- [x] Enums: `PaymentStatus`, `PaymentMethodType`, `EnrollmentStatus`

### 3. Application Layer
- [x] MediatR configurado
- [x] `ProcessPaymentCommand` + Handler
- [x] `GetPaymentByEnrollmentQuery` + Handler
- [x] `CreateCourseCommand` + Handler
- [x] `GetAllCoursesQuery` + Handler
- [x] DTOs para Payments e Courses

### 4. Infrastructure Layer
- [x] `AppDbContext` configurado
- [x] Configuraçª£o de `Course` e `Payment`
- [x] `PaymentRepository` implementado
- [x] `CourseRepository` implementado
- [x] `StudentRepository` implementado
- [x] `UnitOfWork` implementado
- [x] `SimulatedPaymentGateway` (Strategy Pattern)
- [x] `DependencyInjection` extension

### 5. API Layer
- [x] `Program.cs` com DI configurada
- [x] `PaymentsController` com endpoints
- [x] `CoursesController` com endpoints
- [x] `StudentsController` (placeholder)
- [x] `EnrollmentsController` (placeholder)
- [x] `GlobalExceptionHandler`
- [x] Swagger configurado

### 6. Documentaçª£o
- [x] `README.md` completo
- [x] `ARCHITECTURE.md` com decision log
- [x] `MIGRATION_GUIDE.md` (este arquivo)

## 🔧 O que falta fazer

### 1. Migrar código existente do projeto antigo

#### Controllers
- [ ] `AuthController` - Migrar para Application handlers
- [ ] `EnrollmentsController` - Implementar endpoints completos
- [ ] `StudentsController` - Implementar endpoints completos

#### Services (mover lógica para Application)
- [ ] `AuthService` - Criar `LoginCommand`, `RegisterCommand`
- [ ] `CoursesService` - Migrar para handlers existentes
- [ ] `EnrollmentService` - Criar handlers de enrollment
- [ ] `StudentsService` - Criar handlers de student
- [ ] `PaymentService` - Já migrado parcialmente

#### Models
- [ ] `CreatePaymentRequest` → `ProcessPaymentCommand` (já´´e feito)
- [ ] `PaymentResponse` → `PaymentDto` (já´´e feito)
- [ ] `CourseRequest` → `CreateCourseCommand` (já´´e feito)
- [ ] `CourseResponse` → `CourseDto` (já´´e feito)
- [ ] `StudentRequest` → Criar `CreateStudentCommand`
- [ ] `StudentResponse` → Criar `StudentDto`
- [ ] `EnrollmentRequest` → Criar `CreateEnrollmentCommand`
- [ ] `EnrollmentResponse` → Criar `EnrollmentDto`

#### Data
- [ ] `SeedData` - Recriar para nova estrutura
- [ ] Migrations existentes → Usar nova migration inicial

#### Domains
- [ ] `PaymentRules` - Revisar e integrar no aggregate
- [ ] `PaymentStatusTransition` - Revisar e integrar no aggregate
- [ ] `PaymentGatewayTransaction` - Avaliar se é necessário
- [ ] `IPaymentUniquenessChecker` - Integrar no repository

### 2. Passos para completar

#### Passo 1: Testar build
```bash
cd checkpoint1
dotnet build Cursos.Architecture.sln
```

#### Passo 2: Rodar API
```bash
cd src/Cursos.API
dotnet run
```

Acesse: http://localhost:5000/swagger

#### Passo 3: Testar endpoint de pagamento
```bash
curl -X POST http://localhost:5000/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "enrollmentId": "00000000-0000-0000-0000-000000000000",
    "amount": 100.00,
    "paymentMethodType": "CreditCard"
  }'
```

#### Passo 4: Migrar endpoints restantes

Para cada controller existente:

1. **Criar Command/Query no Application**
2. **Criar Handler**
3. **Criar DTO**
4. **Atualizar Controller para usar MediatR**

Exemplo para Students:

```csharp
// Application/Students/CreateStudent/CreateStudentCommand.cs
public record CreateStudentCommand(
    string Name,
    string Email,
    string Phone,
    DateTime BirthDate
) : ICommand<StudentDto>;

// Application/Students/CreateStudent/CreateStudentHandler.cs
public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<StudentDto> Handle(...)
    {
        var student = new Student(...);
        await _studentRepository.AddAsync(student, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return student.ToDto();
    }
}

// API/Controllers/StudentsController.cs
[HttpPost]
public async Task<ActionResult<StudentDto>> Create(
    [FromBody] CreateStudentCommand command)
{
    var student = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
}
```

#### Passo 5: Aplicar migration no banco
```bash
cd src/Cursos.API
dotnet ef database update
```

#### Passo 6: Validar tudo funcionando
- [ ] Swagger abre sem erros
- [ ] Endpoint de Payments funciona
- [ ] Endpoint de Courses funciona
- [ ] Endpoint de Students funciona
- [ ] Endpoint de Enrollments funciona

#### Passo 7: Merge para main (aprovado)
```bash
git checkout main
git merge feature/clean-architecture
```

## 📋 Checklist Final

- [ ] Build sem erros
- [ ] Todos endpoints migrados
- [ ] Testes passando
- [ ] Documentaçª£o atualizada
- [ ] Code review aprovado
- [ ] Merge para main

## ⚠️ Pontos de Atençª£o

1. **Nao exponha entidades do Domain** na API - use sempre DTOs
2. **Regra de negócio fica no Domain**, na~o em Services ou Controllers
3. **Interfaces pequenas (ISP)** - separe `CreatePayment` de `RefundPayment`
4. **Idempotencia** - valide antes de criar pagamento duplicado
5. **Logs sem dados sensveis** - nunca logar números de cartao, CPF, etc

## 🆘 Problemas Comuns

### Erro: "The type or namespace name 'MediatR' could not be found"
Soluçª£o: `dotnet restore` ou reinicie o VS Code

### Erro: "Unable to resolve service for type..."
Soluçª£o: Verifique se o serviço foi registrado no `DependencyInjection.cs`

### Erro de migration
Soluçª£o: `dotnet ef database update` ou delete o banco e recrie
