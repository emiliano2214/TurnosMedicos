using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtencionDeHistorialClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Tratamiento",
                table: "HistoriaClinica",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alergias",
                table: "HistoriaClinica",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AlturaM",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Antecedentes",
                table: "HistoriaClinica",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstudiosSolicitados",
                table: "HistoriaClinica",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamenFisico",
                table: "HistoriaClinica",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FrecuenciaCardiaca",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FrecuenciaRespiratoria",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IMC",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Indicaciones",
                table: "HistoriaClinica",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoConsulta",
                table: "HistoriaClinica",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "HistoriaClinica",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PesoKg",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PresionDiastolica",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PresionSistolica",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximoControl",
                table: "HistoriaClinica",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaturacionO2",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sintomas",
                table: "HistoriaClinica",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperatura",
                table: "HistoriaClinica",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alergias",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "AlturaM",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Antecedentes",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "EstudiosSolicitados",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "ExamenFisico",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "FrecuenciaCardiaca",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "FrecuenciaRespiratoria",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "IMC",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Indicaciones",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "MotivoConsulta",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "PesoKg",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "PresionDiastolica",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "PresionSistolica",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "ProximoControl",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "SaturacionO2",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Sintomas",
                table: "HistoriaClinica");

            migrationBuilder.DropColumn(
                name: "Temperatura",
                table: "HistoriaClinica");

            migrationBuilder.AlterColumn<string>(
                name: "Tratamiento",
                table: "HistoriaClinica",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(800)",
                oldMaxLength: 800,
                oldNullable: true);
        }
    }
}
