# Roles y Permisos

El sistema maneja autenticación y autorización basada en roles.

## Roles Principales

### Admin (Administrador)
- **Acceso Total**: Puede ver y editar médicos, pacientes, especialidades, consultorios y turnos.
- **Gestión**: Encargado de dar de alta médicos y configurar el sistema.
- **Dashboard**: Ve métricas globales.

### Medico
- **Agenda**: Puede ver su propia agenda de turnos.
- **Pacientes**: Puede acceder a la historia clínica de los pacientes que atiende.
- **Restricciones**: No puede editar datos de otros médicos ni borrar turnos arbitrariamente sin justificación (regla de negocio).

### Paciente
- **Autogestión**: Puede solicitar turnos para sí mismo.
- **Historial**: Puede ver sus turnos pasados y futuros.
- **Restricciones**: No puede ver datos de otros pacientes ni agendas completas de médicos (solo disponibilidad).

### Administrativo (Secretaría)
- **Gestión de Turnos**: Puede dar turnos en nombre de pacientes.
- **Recepción**: Marca la llegada de pacientes (check-in).
