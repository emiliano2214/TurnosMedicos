# Ejemplos de Consultas (SQL y LINQ)

Este documento provee ejemplos de cómo consultar la base de datos de TurnosMedicos.

## 1. Turnos por Médico en Rango de Fechas

### SQL
```sql
SELECT m.Nombre, m.Apellido, COUNT(t.IdTurno) as CantidadTurnos
FROM Turno t
JOIN Medico m ON t.IdMedico = m.IdMedico
WHERE t.FechaHora BETWEEN @FechaDesde AND @FechaHasta
GROUP BY m.Nombre, m.Apellido
```

### LINQ
```csharp
var query = context.Turnos
    .Where(t => t.FechaHora >= fechaDesde && t.FechaHora <= fechaHasta)
    .GroupBy(t => t.Medico)
    .Select(g => new { 
        Medico = g.Key.Nombre + " " + g.Key.Apellido, 
        Cantidad = g.Count() 
    });
```

## 2. Pacientes con más Turnos Cancelados

### SQL
```sql
SELECT TOP 10 p.Nombre, p.Apellido, COUNT(t.IdTurno) as Cancelaciones
FROM Turno t
JOIN Paciente p ON t.IdPaciente = p.IdPaciente
WHERE t.Estado = 'Cancelado'
GROUP BY p.Nombre, p.Apellido
ORDER BY Cancelaciones DESC
```

### LINQ
```csharp
var query = context.Turnos
    .Where(t => t.Estado == "Cancelado")
    .GroupBy(t => t.Paciente)
    .Select(g => new {
        Paciente = g.Key.Nombre + " " + g.Key.Apellido,
        Cancelaciones = g.Count()
    })
    .OrderByDescending(x => x.Cancelaciones)
    .Take(10);
```

## 3. Turnos por Especialidad (Último Mes)

### SQL
```sql
SELECT e.Nombre, COUNT(t.IdTurno) as Total
FROM Turno t
JOIN Medico m ON t.IdMedico = m.IdMedico
JOIN Especialidad e ON m.IdEspecialidad = e.IdEspecialidad
WHERE t.FechaHora >= DATEADD(month, -1, GETDATE())
GROUP BY e.Nombre
```

### LINQ
```csharp
var fechaLimite = DateTime.Now.AddMonths(-1);
var query = context.Turnos
    .Where(t => t.FechaHora >= fechaLimite)
    .GroupBy(t => t.Medico.Especialidad.Nombre)
    .Select(g => new {
        Especialidad = g.Key,
        Total = g.Count()
    });
```
