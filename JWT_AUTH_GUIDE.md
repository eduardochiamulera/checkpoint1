# 🔐 Guia de Autenticacao JWT

## Visao Geral

A autenticacao foi implementada usando **JWT (JSON Web Tokens)** seguindo Clean Architecture, com separacao clara entre Domain, Application e Infrastructure.

## 🏗️ Arquitetura

```
Domain Layer:
├── Entities/User.cs              # Entidade User com dados do usuario
├── Interfaces/IUserRepository.cs # Contrato do repositorio
├── Interfaces/IPasswordHasher.cs # Contrato para hash de senha
└── Interfaces/IJwtTokenGenerator.cs # Contrato para geracao de tokens

Application Layer:
├── Auth/RegisterUser/            # Use case de registro
│   ├── RegisterUserCommand.cs
│   └── RegisterUserHandler.cs
├── Auth/AuthenticateUser/        # Use case de login
│   ├── AuthenticateUserCommand.cs
│   └── AuthenticateUserHandler.cs
└── Auth/AuthResultDto.cs         # DTO de resposta

Infrastructure Layer:
├── Security/PasswordHasher.cs    # Implementacao PBKDF2
├── Security/JwtTokenGenerator.cs # Implementacao JWT
└── Repositories/UserRepository.cs # Repositorio EF Core
```

## 🔑 Como Funciona

### 1. Registro de Usuario

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "SenhaForte123!",
  "name": "Nome do Usuario",
  "phone": "+55 11 99999-9999"
}
```

**Resposta de Sucesso (201 Created):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dGhpcyBpcyBhIHJhbmRvbSByZWZyZXNo...",
  "errorMessage": null,
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "usuario@example.com",
    "name": "Nome do Usuario",
    "roles": ["User"]
  }
}
```

### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@example.com",
  "password": "SenhaForte123!"
}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dGhpcyBpcyBhIHJhbmRvbSByZWZyZXNo...",
  "errorMessage": null,
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "usuario@example.com",
    "name": "Nome do Usuario",
    "roles": ["User"]
  }
}
```

### 3. Usar Token em Endpoints Protegidos

```http
GET /api/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Resposta (200 OK):**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "email": "usuario@example.com",
  "name": "Nome do Usuario",
  "roles": ["User"]
}
```

## 🔒 Seguranca

### Password Hashing

- **Algoritmo**: PBKDF2 com SHA-256
- **Salt**: 16 bytes (128 bits) gerado aleatoriamente
- **Iteracoes**: 10.000
- **Hash Size**: 32 bytes (256 bits)

### JWT Token

- **Algoritmo**: HMAC SHA-256
- **Expiration**: 60 minutos (configuravel)
- **Claims incluidas**:
  - `sub`: User ID
  - `email`: Email do usuario
  - `name`: Nome do usuario
  - `iat`: Timestamp de criacao
  - `role`: Roles do usuario (multiplos)

### Refresh Token

- **Tamanho**: 64 bytes aleatorios
- **Formato**: Base64
- **Uso**: Obter novo access token (implementacao futura)

## ⚙️ Configuracao

### appsettings.json

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
    "Issuer": "CursosAPI",
    "Audience": "CursosUsers",
    "ExpirationMinutes": 60
  }
}
```

### ⚠️ IMPORTANTE: Seguranca em Producao

1. **NUNCA** use a `SecretKey` de exemplo em producao
2. Gere uma chave forte (minimo 32 caracteres)
3. Use variaveis de ambiente ou Azure Key Vault
4. Exemplo de geracao de chave:

```bash
# Linux/Mac
openssl rand -base64 32

# PowerShell
[System.Web.Security.Membership]::GeneratePassword(64, 10)
```

## 🧪 Testando com curl

### Registrar Usuario

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SenhaForte123!",
    "name": "Test User",
    "phone": "+55 11 99999-9999"
  }'
```

### Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "SenhaForte123!"
  }'
```

### Acessar Endpoint Protegido

```bash
# Salve o token da resposta de login
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer $TOKEN"
```

## 📦 Clean Architecture Aplicada

### Domain Layer
- **Entidades ricas**: `User` com metodos de dominio
- **Interfaces**: Contratos definidos no Domain
- **Zero dependencias**: Sem referencias a bibliotecas externas

### Application Layer
- **Use Cases isolados**: `RegisterUser`, `AuthenticateUser`
- **Handlers testaveis**: Cada handler faz uma coisa
- **DTOs**: Dados de transferencia bem definidos

### Infrastructure Layer
- **Implementacoes concretas**: EF Core, JWT, PasswordHash
- **Injecao de dependencia**: Configurado no `DependencyInjection.cs`
- **SDKs confinados**: `System.IdentityModel.Tokens.Jwt` fica aqui

## 🛡️ Boas Praticas Implementadas

✅ **Senha nunca trafega em claro** - HTTPS obrigatorio em producao  
✅ **Hash forte** - PBKDF2 com 10.000 iteracoes  
✅ **Token com expiracao** - 60 minutos (ajustavel)  
✅ **Refresh token** - Para obter novo access token  
✅ **Validacao de email duplicado** - No registro  
✅ **Usuario inativo** - Check no login  
✅ **LastLoginAt** - Auditoria de ultimo acesso  
✅ **Roles** - Autorizacao baseada em papeis  

## 🔄 Fluxo Completo

```
1. Usuario faz POST /api/auth/register
   ↓
2. RegisterUserHandler valida email duplicado
   ↓
3. PasswordHasher.Hash() cria hash da senha
   ↓
4. UserRepository salva User no banco
   ↓
5. JwtTokenGenerator gera access token + refresh token
   ↓
6. Retorna AuthResultDto com tokens e dados do usuario
   ↓
7. Cliente armazena token (localStorage/cookie)
   ↓
8. Cliente usa token em endpoints protegidos
   ↓
9. Quando expirar, usa refresh token para obter novo access token
```

## 📚 Referencias

- [RFC 7519 - JWT](https://tools.ietf.org/html/rfc7519)
- [OWASP Password Storage](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [Microsoft JWT Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt)

---

**Status**: ✅ Producao-Ready  
**Ultima Atualizacao**: Agosto 2026
