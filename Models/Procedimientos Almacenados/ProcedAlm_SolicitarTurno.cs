using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosMedicos.Migrations
{
    public partial class ProcedAlm_SolicitarTurno : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.sp_SolicitarTurno
    @IdPaciente     INT,
    @IdEspecialidad INT,
    @FechaAtencion  DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSDATETIME();
    DECLARE @IdMedico INT;

    -- 1) Buscar médico disponible para la especialidad pedida
    SELECT TOP (1) @IdMedico = m.IdMedico
    FROM Medicos m
    WHERE m.IdEspecialidad = @IdEspecialidad
    ORDER BY (
        SELECT COUNT(*) 
        FROM Turnos t 
        WHERE t.IdMedico = m.IdMedico 
          AND CAST(t.FechaHora AS DATE) = CAST(@FechaAtencion AS DATE)
    ), m.IdMedico;

    IF @IdMedico IS NULL
    BEGIN
        RAISERROR ('No hay médicos disponibles para la especialidad solicitada.', 16, 1);
        RETURN;
    END;

    -- 2) Verificar disponibilidad exacta (fecha/hora)
    IF EXISTS (
        SELECT 1 
        FROM Turnos t
        WHERE t.IdMedico = @IdMedico
          AND t.Estado <> 'Cancelado'
          AND t.FechaHora = @FechaAtencion
    )
    BEGIN
        RAISERROR ('El médico no tiene disponibilidad en la fecha/hora solicitada.', 16, 1);
        RETURN;
    END;

    -- 3) Insertar el turno
    INSERT INTO Turnos (IdPaciente, IdMedico, FechaHora, Estado)
    VALUES (@IdPaciente, @IdMedico, @FechaAtencion, 'Pendiente');

    DECLARE @IdTurno INT = SCOPE_IDENTITY();

    -- 4) Devolver comprobante del turno
    SELECT 
        t.IdTurno,
        CONCAT(p.Apellido, ', ', p.Nombre) AS Paciente,
        CONCAT(m.Apellido, ', ', m.Nombre) AS Medico,
        e.Nombre AS Especialista,
        @Now AS FechaEmisionTurno,
        t.FechaHora AS FechaAtencion,
        (SELECT TOP (1) h.Descripcion
         FROM HistoriaClinica h
         WHERE h.IdPaciente = p.IdPaciente AND h.IdMedico = m.IdMedico
         ORDER BY h.FechaRegistro DESC) AS Diagnostico,
        (SELECT TOP (1) tr.Descripcion
         FROM Tratamientos tr
         WHERE tr.IdPaciente = p.IdPaciente
         ORDER BY tr.FechaInicio DESC, tr.IdTratamiento DESC) AS Tratamiento
    FROM Turnos t
    JOIN Pacientes p ON p.IdPaciente = t.IdPaciente
    JOIN Medicos m ON m.IdMedico = t.IdMedico
    JOIN Especialidades e ON e.IdEspecialidad = m.IdEspecialidad
    WHERE t.IdTurno = @IdTurno;
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_SolicitarTurno;");
        }
    }
}
