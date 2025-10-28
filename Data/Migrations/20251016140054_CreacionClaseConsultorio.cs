using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseConsultorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsultorioIdConsultorio",
                table: "Medico",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Consultorio",
                columns: table => new
                {
                    IdConsultorio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultorio", x => x.IdConsultorio);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medico_ConsultorioIdConsultorio",
                table: "Medico",
                column: "ConsultorioIdConsultorio");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Consultorio_ConsultorioIdConsultorio",
                table: "Medico",
                column: "ConsultorioIdConsultorio",
                principalTable: "Consultorio",
                principalColumn: "IdConsultorio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Consultorio_ConsultorioIdConsultorio",
                table: "Medico");

            migrationBuilder.DropTable(
                name: "Consultorio");

            migrationBuilder.DropIndex(
                name: "IX_Medico_ConsultorioIdConsultorio",
                table: "Medico");

            migrationBuilder.DropColumn(
                name: "ConsultorioIdConsultorio",
                table: "Medico");
        }
    }
}
