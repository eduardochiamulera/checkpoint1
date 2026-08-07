Tabela: Students
- Id (INT, PK, AUTO_INCREMENT)
- NomeCompleto (VARCHAR(200), NOT NULL)
- Email (VARCHAR(150), NOT NULL, UNIQUE)
- DataCadastro (DATETIME, NOT NULL)
- UserId (VARCHAR(450), FK → AspNetUsers.Id)

Tabela: Courses
- Id (INT, PK, AUTO_INCREMENT)
- Titulo (VARCHAR(200), NOT NULL)
- Descricao (TEXT)
- Categoria (VARCHAR(100))
- CargaHoraria (INT, NOT NULL)
- DataCriacao (DATETIME, NOT NULL)

Tabela: Enrollments
- Id (INT, PK, AUTO_INCREMENT)
- StudentId (INT, FK → Students.Id)
- CourseId (INT, FK → Courses.Id)
- Status (ENUM: Ativo/Cancelado, DEFAULT Ativo)
- DataMatricula (DATETIME, NOT NULL)
- UNIQUE(StudentId, CourseId)