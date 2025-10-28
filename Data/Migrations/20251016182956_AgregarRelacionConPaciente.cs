using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionConPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_ObraSocial_ObraSocialIdObraSocial",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_ObraSocialIdObraSocial",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "ObraSocialIdObraSocial",
                table: "Paciente");

            migrationBuilder.AddColumn<int>(
                name: "IdObraSocial",
                table: "Paciente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_IdObraSocial",
                table: "Paciente",
                column: "IdObraSocial");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_ObraSocial_IdObraSocial",
                table: "Paciente",
                column: "IdObraSocial",
                principalTable: "ObraSocial",
                principalColumn: "IdObraSocial",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_ObraSocial_IdObraSocial",
                table: "Paciente");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_IdObraSocial",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "IdObraSocial",
                table: "Paciente");

            migrationBuilder.AddColumn<int>(
                name: "ObraSocialIdObraSocial",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_ObraSocialIdObraSocial",
                table: "Paciente",
                column: "ObraSocialIdObraSocial");

            migrationBuilder.AddForeignKey(
                name: "FK_Paciente_ObraSocial_ObraSocialIdObraSocial",
                table: "Paciente",
                column: "ObraSocialIdObraSocial",
                principalTable: "ObraSocial",
                principalColumn: "IdObraSocial");
        }
    }
}
