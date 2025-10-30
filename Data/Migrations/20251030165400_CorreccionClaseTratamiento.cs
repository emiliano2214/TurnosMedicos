using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionClaseTratamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tratamiento_Paciente_PacienteIdPaciente",
                table: "Tratamiento");

            migrationBuilder.DropIndex(
                name: "IX_Tratamiento_PacienteIdPaciente",
                table: "Tratamiento");

            migrationBuilder.DropColumn(
                name: "PacienteIdPaciente",
                table: "Tratamiento");

            migrationBuilder.CreateIndex(
                name: "IX_Tratamiento_IdPaciente",
                table: "Tratamiento",
                column: "IdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_Tratamiento_Paciente_IdPaciente",
                table: "Tratamiento",
                column: "IdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tratamiento_Paciente_IdPaciente",
                table: "Tratamiento");

            migrationBuilder.DropIndex(
                name: "IX_Tratamiento_IdPaciente",
                table: "Tratamiento");

            migrationBuilder.AddColumn<int>(
                name: "PacienteIdPaciente",
                table: "Tratamiento",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tratamiento_PacienteIdPaciente",
                table: "Tratamiento",
                column: "PacienteIdPaciente");

            migrationBuilder.AddForeignKey(
                name: "FK_Tratamiento_Paciente_PacienteIdPaciente",
                table: "Tratamiento",
                column: "PacienteIdPaciente",
                principalTable: "Paciente",
                principalColumn: "IdPaciente");
        }
    }
}
