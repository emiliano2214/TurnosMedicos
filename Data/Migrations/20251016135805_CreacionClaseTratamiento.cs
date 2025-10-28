using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseTratamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdConsultorio",
                table: "Medico",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tratamiento",
                columns: table => new
                {
                    IdTratamiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "int", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratamiento", x => x.IdTratamiento);
                    table.ForeignKey(
                        name: "FK_Tratamiento_Paciente_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Paciente",
                        principalColumn: "IdPaciente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tratamiento_PacienteIdPaciente",
                table: "Tratamiento",
                column: "PacienteIdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tratamiento");

            migrationBuilder.DropColumn(
                name: "IdConsultorio",
                table: "Medico");
        }
    }
}
