using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseHistoriaClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoriaClinica",
                columns: table => new
                {
                    IdHistoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    PacienteIdPaciente = table.Column<int>(type: "int", nullable: true),
                    IdMedico = table.Column<int>(type: "int", nullable: false),
                    MedicoIdMedico = table.Column<int>(type: "int", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriaClinica", x => x.IdHistoria);
                    table.ForeignKey(
                        name: "FK_HistoriaClinica_Medico_MedicoIdMedico",
                        column: x => x.MedicoIdMedico,
                        principalTable: "Medico",
                        principalColumn: "IdMedico");
                    table.ForeignKey(
                        name: "FK_HistoriaClinica_Paciente_PacienteIdPaciente",
                        column: x => x.PacienteIdPaciente,
                        principalTable: "Paciente",
                        principalColumn: "IdPaciente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_MedicoIdMedico",
                table: "HistoriaClinica",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_PacienteIdPaciente",
                table: "HistoriaClinica",
                column: "PacienteIdPaciente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriaClinica");
        }
    }
}
