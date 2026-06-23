# Cursos API

Uma API funcional com CRUDs, autenticação, autorização e documentação completa.

## 🚀 Getting Started

```bash
git clone https://github.com
cd project-repo
dotnet build
dotnet run
```

## Requisitos
.NET8
MySQL

## Endpoints de Autenticação

### POST /api/auth/register
Registra um novo usuário. Recebe email e senha. Retorna 201 em caso de sucesso.

### POST /api/auth/login
Autentica o usuário com email e senha. Retorna um JWT de acesso e um refresh token.

### POST /api/auth/refresh
Recebe o refresh token e retorna um novo JWT de acesso.