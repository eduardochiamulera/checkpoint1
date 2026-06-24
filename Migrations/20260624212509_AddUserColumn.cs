using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cursos.Migrations
{
    /// <inheritdoc />
    public partial class AddUserColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlteracao",
                table: "Courses",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAlteracao",
                table: "Courses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCriacao",
                table: "Courses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAlteracao",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UsuarioAlteracao",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UsuarioCriacao",
                table: "Courses");
        }
    }
}
