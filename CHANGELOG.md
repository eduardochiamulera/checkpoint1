# Changelog

Todas as mudanças notáveis neste projeto estão documentadas aqui.
Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/).

## [0.1.0] - 2026-06-25

### Adicionado
- CRUD completo de Cursos com paginação, filtros e ordenação
- CRUD completo de Estudantes com soft delete
- Sistema de Matrículas com controle de duplicatas e cancelamento
- Autenticação e autorização via JWT com ASP.NET Core Identity
- Roles: Admin, Instructor, Student com permissões diferenciadas
- Seed automático de roles e usuário admin na inicialização
- Validações de domínio: título mínimo 3 chars, email único, matrícula única
- Tratamento global de exceções com respostas padronizadas (ProblemDetails)
- Documentação Swagger com Security Scheme Bearer JWT
- XML comments em todos os controllers e models