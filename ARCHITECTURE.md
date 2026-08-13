# Decision Log de Arquitetura

## 1. Separacao em 4 Camadas

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
Projeto monolitico com tudo em `src/` estava dificil de testar e manter.

### Decisao
Separar em Domain, Application, Infrastructure, API.

### Consequencias
- ✅ Testabilidade aumentada
- ✅ Baixo acoplamento
- ⚠️ Mais arquivos para navegar
- ⚠️ Curva de aprendizado para novos devs

## 2. MediatR para Use Cases

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
Controllers chamavam Services diretamente, criando acoplamento.

### Decisao
Usar MediatR para Command/Query handlers.

### Consequencias
- ✅ Handlers testaveis isoladamente
- ✅ Pipeline behaviors para logging/validacao
- ⚠️ Mais boilerplate (Command + Handler + Response)

## 3. Strategy Pattern para Gateways

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
SimulatedPaymentGateway estava acoplado no Domain.

### Decisao
Interface `IPaymentGateway` no Domain, implementacoes em Infrastructure.

### Consequencias
- ✅ Troca de gateway via configuracao
- ✅ Testes com mock do gateway
- ✅ Domain nao conhece detalhes de SDKs

## 4. Agregados Ricos

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
Entidades eram apenas getters/setters (anemic domain).

### Decisao
Payment como agregado raiz com metodos que garantem invariantes:
- `Confirm()` - so pode confirmar se estiver Pending
- `Cancel()` - so pode cancelar se estiver Pending
- `Refund()` - so pode estornar se estiver Confirmed

### Consequencias
- ✅ Regras de negocio centralizadas
- ✅ Estado sempre valido
- ⚠️ Mais complexidade inicial

## 5. Value Objects

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
Money era apenas um decimal solto, sem validacao.

### Decisao
Criar `Money` como Value Object imutavel com:
- Validacao de valor negativo no construtor
- Igualdade por valor ( Amount + Currency)
- Metodos de dominio (`Add()`, `Zero()`)

### Consequencias
- ✅ Type safety
- ✅ Validacao centralizada
- ✅ Intencionalidade explicita no codigo

## 6. Repositorios por Agregado

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
DbContext sendo usado diretamente nos services.

### Decisao
Criar interfaces `IPaymentRepository`, `ICourseRepository`, etc.

### Consequencias
- ✅ Testabilidade (mock de repositorios)
- ✅ Desacoplamento do EF Core
- ✅ Troca de persistencia sem mexer no Domain

## 7. Tratamento Global de Excecoes

**Status**: Aprovado  
**Data**: 2026-08-13

### Contexto
Cada controller tinha seu proprio try/catch.

### Decisao
Usar `IExceptionHandler` do ASP.NET Core para tratamento centralizado.

### Consequencias
- ✅ Controllers mais limpos
- ✅ Respostas de erro padronizadas (ProblemDetails)
- ✅ Logs centralizados de erros
