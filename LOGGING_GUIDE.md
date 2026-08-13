# 📝 Guia de Logging Estruturado

## Visao Geral

O logging estruturado foi implementado para fornecer visibilidade completa sobre o comportamento da aplicacao, facilitando troubleshooting, monitoramento e auditoria.

## 🎯 Principios

1. **Dados sensveis NUNCA sao logados** - Senhas, tokens, CPF, cartoes de credito
2. **Mensagens curtas e acionaveis** - Facil de entender e agir
3. **Niveis coerentes** - Info, Warn, Error usados corretamente
4. **Contexto rico** - CorrelationId, userId, rota, status

## 📊 Niveis de Log

| Nivel | Quando Usar | Exemplo |
|-------|-------------|---------|
| **Debug** | Detalhes tecnicos para desenvolvimento | "Payment {Id} created for enrollment {EnrollmentId}" |
| **Information** | Fluxo normal da aplicacao | "Payment {Id} confirmed successfully" |
| **Warning** | Situacoes que requerem atencao mas nao sao erros | "Payment already confirmed for enrollment {Id}" |
| **Error** | Erros que impedem operacao | "Payment gateway failed with error" |

## 🔍 Logging Middleware

O middleware de logging captura todas as requisicoes HTTP e registra:

- **CorrelationId** - Identificador unico da requisicao
- **UserId** - Email do usuario autenticado
- **Method/Path** - Rota acessada
- **StatusCode** - Status da resposta
- **ElapsedMs** - Tempo de processamento

### Exemplo de Log

```
[Info] [a1b2c3d4] POST /api/payments started - User: test@example.com
[Info] [a1b2c3d4] Processing payment for enrollment 123e4567-e89b-12d3-a456-426614174000, amount 100.00
[Info] [a1b2c3d4] Payment 987fcdeb-5123-4567-89ab-cdef01234567 created
[Info] [a1b2c3d4] Payment gateway processed payment with transaction sim_abc123
[Info] [a1b2c3d4] Payment 987fcdeb confirmed successfully, transaction sim_abc123
[Info] [a1b2c3d4] POST /api/payments completed with status 200 in 145ms - User: test@example.com
```

## 📝 Como Ativar Logs

### Desenvolvimento (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information",
      "Cursos": "Debug"
    }
  }
}
```

### Producao (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Cursos": "Information"
    }
  }
}
```

### Niveis Disponiveis

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",  // Trace < Debug < Information < Warning < Error < Critical < None
      "Cursos.Application": "Debug",
      "Cursos.Infrastructure": "Information",
      "Cursos.API": "Information"
    }
  }
}
```

## 🔍 Como Ler Logs

### 1. Console (Desenvolvimento)

Os logs sao exibidos no console onde a API esta rodando:

```bash
cd src/Cursos.API
dotnet run

# Logs aparecem no console:
# [Info] [a1b2c3d4] POST /api/payments started...
```

### 2. Arquivo de Log (Producao)

Para salvar logs em arquivo, adicione ao `Program.cs`:

```csharp
builder.Logging.AddFile("Logs/app-{Date}.log");
```

### 3. Application Insights (Azure)

Para logs em producao na nuvem:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

## 📋 Pontos Criticos de Logging

### Criacao de Pagamento

```csharp
_logger.LogInformation(
    "Processing payment for enrollment {EnrollmentId}, amount {Amount}",
    request.EnrollmentId,
    request.Amount);
```

**Log Gerado:**
```
[Info] Processing payment for enrollment 123e4567-e89b-12d3-a456-426614174000, amount 100.00
```

### Confirmacao de Pagamento

```csharp
_logger.LogInformation(
    "Payment {PaymentId} confirmed successfully for enrollment {EnrollmentId}, transaction {TransactionId}",
    payment.Id,
    request.EnrollmentId,
    gatewayResult.TransactionId);
```

**Log Gerado:**
```
[Info] Payment 987fcdeb-5123-4567-89ab-cdef01234567 confirmed successfully for enrollment 123e4567-e89b-12d3-a456-426614174000, transaction sim_abc123
```

### Falha de Gateway

```csharp
_logger.LogError(
    "Payment gateway failed for payment {PaymentId}, enrollment {EnrollmentId}. Error: {Error}",
    payment.Id,
    request.EnrollmentId,
    gatewayResult.ErrorMessage);
```

**Log Gerado:**
```
[Error] Payment gateway failed for payment 987fcdeb-5123-4567-89ab-cdef01234567, enrollment 123e4567-e89b-12d3-a456-426614174000. Error: Gateway timeout
```

## 🔒 Dados Sensiveis - O QUE NAO LOGAR

❌ **NUNCA LOGAR:**
- Senhas
- Tokens JWT completos
- Numeros de cartao de credito
- CPF/CNPJ
- Chaves de API
- Segredos de conexao

✅ **PODE LOGAR:**
- IDs (PaymentId, UserId, EnrollmentId)
- Emails (parcialmente mascarado se necessario)
- Status de operacoes
- Timestamps
- Valores monetarios (sem dados de cartao)

## 📊 Health Checks

### Endpoints Disponiveis

| Endpoint | Descricao | Tags |
|----------|-----------|------|
| `/health` | Health check completo (API + DB) | - |
| `/health/ready` | Apenas database (para readiness) | database |

### Exemplo de Resposta

```bash
curl http://localhost:5000/health
```

**Saudavel (200 OK):**
```json
{
  "status": "Healthy",
  "totalDuration": 00:00:00.0123456,
  "entries": {
    "self": {
      "status": "Healthy",
      "description": "API is running",
      "duration": "00:00:00.0001234"
    },
    "sql-server": {
      "status": "Healthy",
      "description": "SQL Server is accessible",
      "duration": "00:00:00.0123456"
    }
  }
}
```

**Doente (503 Service Unavailable):**
```json
{
  "status": "Unhealthy",
  "totalDuration": 00:00:00.0500000,
  "entries": {
    "self": {
      "status": "Healthy",
      "duration": "00:00:00.0001234"
    },
    "sql-server": {
      "status": "Unhealthy",
      "description": "Connection timeout",
      "duration": "00:00:00.0500000"
    }
  }
}
```

### Monitoramento Continuo

Para monitorar health checks em producao:

```bash
# Kubernetes liveness probe
livenessProbe:
  httpGet:
    path: /health
    port: 5000
  initialDelaySeconds: 30
  periodSeconds: 10

# Kubernetes readiness probe
readinessProbe:
  httpGet:
    path: /health/ready
    port: 5000
  initialDelaySeconds: 5
  periodSeconds: 5
```

## 🧪 Testando Logs

### 1. Requisicao com CorrelationId

```bash
curl -X POST http://localhost:5000/api/payments \
  -H "Content-Type: application/json" \
  -H "X-Correlation-ID: my-custom-id-123" \
  -d '{
    "enrollmentId": "00000000-0000-0000-0000-000000000000",
    "amount": 100.00,
    "paymentMethodType": "CreditCard"
  }'
```

**Log com CorrelationId customizado:**
```
[Info] [my-custom-id-123] POST /api/payments started - User: test@example.com
```

### 2. Usuario Autenticado

```bash
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer $TOKEN"
```

**Log com userId:**
```
[Info] [a1b2c3d4] GET /api/auth/me started - User: test@example.com
```

## 📚 Boas Praticas

1. **Use interpolacao de strings** - `{EnrollmentId}` em vez de `+ enrollmentId`
2. **Seja especifico** - "Payment confirmed" em vez de "Success"
3. **Inclua contexto** - IDs, usuarios, rotas
4. **Mantenha curto** - Mensagens diretas e objetivas
5. **Use niveis corretos** - Info para fluxo normal, Error para falhas
6. **Nunca logue dados sensiveis** - Senhas, tokens, CPFs

## 🔗 Referencias

- [Microsoft Logging Guidelines](https://learn.microsoft.com/en-us/dotnet/core/logging)
- [Structured Logging Best Practices](https://www.meziant.com/blog/structured-logging-best-practices)
- [Health Checks in .NET](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

---

**Status**: ✅ Producao-Ready  
**Ultima Atualizacao**: Agosto 2026
