using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtenderHistorialMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "HistoriaClinica",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "HistoriaClinica",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdTurno",
                table: "HistoriaClinica",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tratamiento",
                table: "HistoriaClinica",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriaClinica_IdTurno",
                table: "HistoriaClinica",
                column: "IdTurno");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoriaClinica_Turno_IdTurno",
                table: "HistoriaClinica",
                column: "IdTurno",
                principalTable: "Turno",
                principalColumn: "IdTurno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoriaClinica_Turno_IdTurno",
                table: "HistoriaClinica");

            migrationBuilder.DropIndex(
                name: "IX_HistoriaClinica_IdTurno",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "IdTurno",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Tratamiento",
                table: "HistoriaClinica");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "HistoriaClinica",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
