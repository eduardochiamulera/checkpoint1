# Cursos API

API RESTful para gerenciamento de cursos, estudantes e matrículas, construída com .NET 9, ASP.NET Core Identity e JWT.

## Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MySQL 8.0+

## Como rodar localmente

```bash
# 1. Clone o repositório
git clone https://github.com/eduardochiamulera/checkpoint1.git
cd checkpoint1

# 2. Configure os segredos (nunca commitar esses valores)
dotnet user-secrets set "ConnectionStrings:appConnection" "Server=localhost;Port=3306;Database=cursos_db;User=root;Password=sua_senha;"
dotnet user-secrets set "Jwt:Key" "sua-chave-secreta-minimo-32-caracteres"
dotnet user-secrets set "AdminUser:Password" "SenhaAdmin"
dotnet user-secrets set "Jwt:Issuer" "Issuer"
dotnet user-secrets set "Jwt:Audience" "Audience"

# 3. Aplique as migrations (cria o banco e as tabelas)
dotnet ef database update

# 4. Rode a aplicação
dotnet run
```

> **Referência de configuração:** copie `appsettings.example.json` para entender todas as chaves disponíveis. Nunca preencha valores reais no `appsettings.json`.

## Acessando o Swagger

Com a aplicação rodando, acesse:


O Swagger abre na raiz. Para autenticar:

1. Chame `POST /api/v1/auth/login` com email e senha do admin
2. Copie o `accessToken` da resposta
3. Clique no botão **Authorize** (cadeado) no topo do Swagger
4. Cole o token no campo `Value` e clique em **Authorize**
5. Todas as rotas protegidas agora funcionarão com seu token

## Endpoints principais

### Auth
| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | `/api/v1/auth/register` | Registra Admin ou Instructor | Público |
| POST | `/api/v1/auth/login` | Login, retorna JWT | Público |

### Courses
| Método | Rota | Descrição | Roles |
|--------|------|-----------|-------|
| GET | `/api/v1/courses` | Lista cursos paginados | Público |
| GET | `/api/v1/courses/{id}` | Detalhe do curso | Público |
| POST | `/api/v1/courses` | Criar curso | Admin, Instructor |
| PUT | `/api/v1/courses/{id}` | Atualizar curso | Admin, Instructor |
| DELETE | `/api/v1/courses/{id}` | Desativar curso | Admin |

### Students
| Método | Rota | Descrição | Roles |
|--------|------|-----------|-------|
| GET | `/api/v1/students` | Lista estudantes | Admin |
| GET | `/api/v1/students/{id}` | Detalhe do estudante | Admin, próprio |
| POST | `/api/v1/students` | Criar estudante | Admin |
| PUT | `/api/v1/students/{id}` | Atualizar estudante | Admin, próprio |
| DELETE | `/api/v1/students/{id}` | Desativar estudante | Admin |

### Enrollments
| Método | Rota | Descrição | Roles |
|--------|------|-----------|-------|
| POST | `/api/v1/enrollments` | Auto-matrícula | Student |
| POST | `/api/v1/enrollments/admin` | Matricular estudante | Admin |
| GET | `/api/v1/students/{id}/enrollments` | Matrículas do estudante | Admin, próprio |
| DELETE | `/api/v1/enrollments/{id}` | Cancelar matrícula | Admin, próprio |

## Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations pendentes
dotnet ef database update

# Reverter última migration
dotnet ef database update NomeDaMigrationAnterior
```

## Dados iniciais (Seed)

Na primeira execução, o sistema cria automaticamente:
- Roles: `Admin`, `Instructor`, `Student`
- Usuário admin com o email `admin@cursos.com` e a senha definida em `AdminUser:Password`

## Erros comuns

| Erro | Causa | Solução |
|------|-------|---------|
| `AdminUser:Password não configurado` | Secret ausente | Execute o `dotnet user-secrets set` acima |
| `Unable to connect to MySQL` | Banco offline ou connection string errada | Verifique o MySQL e a string em user-secrets |
| `401 Unauthorized` | Token expirado ou ausente | Refaça login e atualize o token no Swagger |
| `403 Forbidden` | Role insuficiente | Verifique a role do usuário autenticado |
| `409 Conflict` | Email duplicado ou matrícula existente | Use outro email ou verifique matrículas ativas |

## Como rodar os testes

```bash
dotnet test
```

> Testes ainda não implementados nesta versão. Ver [CHANGELOG](./CHANGELOG.md).