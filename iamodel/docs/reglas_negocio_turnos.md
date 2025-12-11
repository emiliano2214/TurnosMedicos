# Reglas de Negocio: Turnos

## General
- Los turnos son la unidad central de la atención médica en el sistema.
- Un turno vincula a un **Paciente** con un **Médico** en una **Fecha y Hora** específica.

## Creación de Turnos
- Los pacientes pueden solicitar turnos para una especialidad o médico específico.
- Los administrativos pueden asignar turnos a pacientes.
- No se pueden crear turnos en horarios pasados.
- El sistema debe validar que el médico tenga disponibilidad en el horario solicitado (aunque esta validación puede estar en proceso de mejora).

## Restricciones
- Un médico no puede tener dos turnos a la misma hora.
- Un paciente no puede tener dos turnos a la misma hora.

## Duración
- La duración estándar de un turno no está explícita en el modelo actual, pero se asume que se gestiona por slots de tiempo definidos por la agenda del médico.
