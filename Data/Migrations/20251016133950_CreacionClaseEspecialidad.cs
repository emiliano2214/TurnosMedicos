using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreacionClaseEspecialidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EspecialidadIdEspecialidad",
                table: "Medico",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Especialidad",
                columns: table => new
                {
                    IdEspecialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidad", x => x.IdEspecialidad);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medico_EspecialidadIdEspecialidad",
                table: "Medico",
                column: "EspecialidadIdEspecialidad");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Especialidad_EspecialidadIdEspecialidad",
                table: "Medico",
                column: "EspecialidadIdEspecialidad",
                principalTable: "Especialidad",
                principalColumn: "IdEspecialidad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Especialidad_EspecialidadIdEspecialidad",
                table: "Medico");

            migrationBuilder.DropTable(
                name: "Especialidad");

            migrationBuilder.DropIndex(
                name: "IX_Medico_EspecialidadIdEspecialidad",
                table: "Medico");

            migrationBuilder.DropColumn(
                name: "EspecialidadIdEspecialidad",
                table: "Medico");
        }
    }
}
