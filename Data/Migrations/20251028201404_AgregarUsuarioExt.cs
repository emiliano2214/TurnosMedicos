using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUsuarioExt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriaClinica_Medico_MedicoIdMedico",
                table: "HistoriaClinica");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriaClinica_Paciente_PacienteIdPaciente",
                table: "HistoriaClinica");

            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Medico_MedicoIdMedico",
                table: "Turno");

            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Paciente_PacienteIdPaciente",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_MedicoIdMedico",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_PacienteIdPaciente",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_HistoriaClinica_MedicoIdMedico",
                table: "HistoriaClinica");

            migrationBuilder.DropIndex(
                name: "IX_HistoriaClinica_PacienteIdPaciente",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "MedicoIdMedico",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "Turno");

            migrationBuilder.DropColumn(
                name: "MedicoIdMedico",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "HistoriaClinica");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacienteId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turno_IdMedico",
                table: "Turno",
                column: "IdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_IdPaciente",
                table: "Turno",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_IdMedico",
                table: "HistoriaClinica",
                column: "IdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_IdPaciente",
                table: "HistoriaClinica",
                column: "IdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriaClinica_Medico_IdMedico",
                table: "HistoriaClinica",
                column: "IdMedico",
                principalTable: "Medico",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriaClinica_Paciente_IdPaciente",
                table: "HistoriaClinica",
                column: "IdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Medico_IdMedico",
                table: "Turno",
                column: "IdMedico",
                principalTable: "Medico",
                principalColumn: "IdMedico",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Paciente_IdPaciente",
                table: "Turno",
                column: "IdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriaClinica_Medico_IdMedico",
                table: "HistoriaClinica");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoriaClinica_Paciente_IdPaciente",
                table: "HistoriaClinica");

            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Medico_IdMedico",
                table: "Turno");

            migrationBuilder.DropForeignKey(
                name: "FK_Turno_Paciente_IdPaciente",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_IdMedico",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_Turno_IdPaciente",
                table: "Turno");

            migrationBuilder.DropIndex(
                name: "IX_HistoriaClinica_IdMedico",
                table: "HistoriaClinica");

            migrationBuilder.DropIndex(
                name: "IX_HistoriaClinica_IdPaciente",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PacienteId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "MedicoIdMedico",
                table: "Turno",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "Turno",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicoIdMedico",
                table: "HistoriaClinica",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "HistoriaClinica",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turno_MedicoIdMedico",
                table: "Turno",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_PacienteIdPaciente",
                table: "Turno",
                column: "PacienteIdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_MedicoIdMedico",
                table: "HistoriaClinica",
                column: "MedicoIdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_PacienteIdPaciente",
                table: "HistoriaClinica",
                column: "PacienteIdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriaClinica_Medico_MedicoIdMedico",
                table: "HistoriaClinica",
                column: "MedicoIdMedico",
                principalTable: "Medico",
                principalColumn: "IdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriaClinica_Paciente_PacienteIdPaciente",
                table: "HistoriaClinica",
                column: "PacienteIdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Medico_MedicoIdMedico",
                table: "Turno",
                column: "MedicoIdMedico",
                principalTable: "Medico",
                principalColumn: "IdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_Turno_Paciente_PacienteIdPaciente",
                table: "Turno",
                column: "PacienteIdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente");
        }
    }
}
