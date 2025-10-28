using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    IdTurno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "int", nullable: true),
                    IdMedico = table.Column<int>(type: "int", nullable: false),
                    MedicoIdMedico = table.Column<int>(type: "int", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.IdTurno);
                    table.ForeignKey(
                        name: "FK_Turno_Medico_MedicoIdMedico",
                        column: x => x.MedicoIdMedico,
                        principalTable: "Medico",
                        principalColumn: "IdMedico");
                    table.ForeignKey(
                        name: "FK_Turno_Paciente_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Paciente",
                        principalColumn: "IdPaciente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turno_MedicoIdMedico",
                table: "Turno",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_PacienteIdPaciente",
                table: "Turno",
                column: "PacienteIdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turno");
        }
    }
}
