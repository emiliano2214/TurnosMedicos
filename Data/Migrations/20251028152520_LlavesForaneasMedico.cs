using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class LlavesForaneasMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Consultorio_ConsultorioIdConsultorio",
                table: "Medico");

            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Especialidad_EspecialidadIdEspecialidad",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_ConsultorioIdConsultorio",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_EspecialidadIdEspecialidad",
                table: "Medico");

            migrationBuilder.DropColumn(
                name: "ConsultorioIdConsultorio",
                table: "Medico");

            migrationBuilder.DropColumn(
                name: "EspecialidadIdEspecialidad",
                table: "Medico");

            migrationBuilder.CreateIndex(
                name: "IX_Medico_IdConsultorio",
                table: "Medico",
                column: "IdConsultorio");

            migrationBuilder.CreateIndex(
                name: "IX_Medico_IdEspecialidad",
                table: "Medico",
                column: "IdEspecialidad");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Consultorio_IdConsultorio",
                table: "Medico",
                column: "IdConsultorio",
                principalTable: "Consultorio",
                principalColumn: "IdConsultorio",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Especialidad_IdEspecialidad",
                table: "Medico",
                column: "IdEspecialidad",
                principalTable: "Especialidad",
                principalColumn: "IdEspecialidad",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Consultorio_IdConsultorio",
                table: "Medico");

            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Especialidad_IdEspecialidad",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_IdConsultorio",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_IdEspecialidad",
                table: "Medico");

            migrationBuilder.AddColumn<int>(
                name: "ConsultorioIdConsultorio",
                table: "Medico",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EspecialidadIdEspecialidad",
                table: "Medico",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medico_ConsultorioIdConsultorio",
                table: "Medico",
                column: "ConsultorioIdConsultorio");

            migrationBuilder.CreateIndex(
                name: "IX_Medico_EspecialidadIdEspecialidad",
                table: "Medico",
                column: "EspecialidadIdEspecialidad");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Consultorio_ConsultorioIdConsultorio",
                table: "Medico",
                column: "ConsultorioIdConsultorio",
                principalTable: "Consultorio",
                principalColumn: "IdConsultorio");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Especialidad_EspecialidadIdEspecialidad",
                table: "Medico",
                column: "EspecialidadIdEspecialidad",
                principalTable: "Especialidad",
                principalColumn: "IdEspecialidad");
        }
    }
}
