using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cursos.Migrations
{
    /// <inheritdoc />
    public partial class AddUserColumnOnStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAlteracao",
                table: "Students",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioAlteracao",
                table: "Students",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCriacao",
                table: "Students",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAlteracao",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UsuarioAlteracao",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UsuarioCriacao",
                table: "Students");
        }
    }
}
