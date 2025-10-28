using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseObraSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObraSocialIdObraSocial",
                table: "Paciente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ObraSocial",
                columns: table => new
                {
                    IdObraSocial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObraSocial", x => x.IdObraSocial);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paciente_ObraSocial_ObraSocialIdObraSocial",
                table: "Paciente");

            migrationBuilder.DropTable(
                name: "ObraSocial");

            migrationBuilder.DropIndex(
                name: "IX_Paciente_ObraSocialIdObraSocial",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "ObraSocialIdObraSocial",
                table: "Paciente");
        }
    }
}
