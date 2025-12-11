# Estados de Turno

El campo `Estado` en la entidad `Turno` maneja el ciclo de vida de la cita.

## Estados Posibles

### Pendiente
- **Descripción**: Estado inicial por defecto cuando se crea un turno.
- **Significado**: El turno ha sido solicitado pero aún no ha ocurrido ni ha sido cancelado.
- **Valor en BD**: "Pendiente"

### Confirmado
- **Descripción**: El turno ha sido reconfirmado por el paciente o la clínica (si aplica flujo de confirmación).
- **Valor en BD**: "Confirmado" (Asumido, verificar si se usa en la lógica actual).

### Cancelado
- **Descripción**: El turno fue dado de baja por el paciente o el médico.
- **Significado**: El horario queda liberado (o queda registro histórico de la cancelación).
- **Valor en BD**: "Cancelado"

### Realizado / Atendido
- **Descripción**: El paciente asistió y el médico completó la atención.
- **Valor en BD**: "Realizado" (o "Atendido").

### Ausente
- **Descripción**: El paciente no se presentó al turno.
- **Valor en BD**: "Ausente"

> **Nota**: Actualmente el código inicializa el estado en "Pendiente".
