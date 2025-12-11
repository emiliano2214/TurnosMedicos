# Modelo de Datos OLTP

Descripción de las tablas principales del sistema `TurnosMedicos`.

## Tablas Principales

### Turno
Representa la cita médica.
- **PK**: `IdTurno` (int)
- **FK**: `IdPaciente` -> `Paciente`
- **FK**: `IdMedico` -> `Medico`
- **Campos**:
  - `FechaHora` (DateTime): Cuándo es el turno.
  - `Estado` (string): "Pendiente", "Cancelado", etc.

### Medico
Profesional de la salud.
- **PK**: `IdMedico` (int)
- **FK**: `IdEspecialidad` -> `Especialidad`
- **FK**: `IdConsultorio` -> `Consultorio`
- **Campos**:
  - `Nombre`, `Apellido` (string)
  - `Matricula` (string)
  - `UserId` (string): Vinculación con Identity User.

### Paciente
El usuario que recibe la atención.
- **PK**: `IdPaciente` (int)
- **Campos**:
  - `Nombre`, `Apellido`, `DNI`, `Email`, `Telefono`.
  - `ObraSocial` (vinculación).

### Especialidad
Categoría médica (Cardiología, Pediatría, etc.).
- **PK**: `IdEspecialidad`
- **Campos**: `Nombre`.

### Consultorio
Lugar físico de atención.
- **PK**: `IdConsultorio`
- **Campos**: `Nombre` / `Numero`.

### HistoriaClinica
Registro médico del paciente.
- **Relación**: Vincula Paciente y Médico (probablemente).

### Tratamiento
Detalle de intervenciones o recetas.

## Relaciones Clave
- Un **Medico** tiene una **Especialidad**.
- Un **Turno** pertenece a un **Medico** y un **Paciente**.
- Un **Medico** atiende en un **Consultorio** (simplificación actual, podría atender en varios).
