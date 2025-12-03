# TurnosMedicos

Sistema de gestión de turnos médicos desarrollado en ASP.NET Core 8 MVC.

## Descripción
Aplicación web para la administración de clínicas, permitiendo la gestión de:
- **Pacientes y Médicos**
- **Turnos y Agendas**
- **Historias Clínicas y Tratamientos**
- **Obras Sociales y Especialidades**

El sistema cuenta con roles diferenciados para asegurar la privacidad y el correcto flujo de trabajo.

## Roles y Alcance
El sistema define 4 roles principales con permisos específicos:

### 1. Admin (Administrador)
**Alcance:** Total.
- Tiene control absoluto del sistema.
- **Gestión:** Puede crear, editar y eliminar cualquier entidad (Médicos, Pacientes, Especialidades, Obras Sociales, Consultorios).
- **Usuarios:** Puede gestionar usuarios y asignar roles.
- **Turnos:** Puede ver y administrar la agenda completa de todos los médicos.

### 2. Administrativo
**Alcance:** Operativo / Gestión diaria.
- Diseñado para el personal de recepción o secretaría.
- **Gestión:** Puede registrar y editar Pacientes y Médicos.
- **Turnos:** Puede asignar, modificar y cancelar turnos para cualquier paciente y médico.
- **Restricción:** No puede gestionar usuarios del sistema ni configuraciones sensibles (como borrar Obras Sociales).

### 3. Medico
**Alcance:** Personal / Agenda propia.
- **Agenda:** Visualiza únicamente sus propios turnos asignados.
- **Gestión de Turnos:** Puede cambiar el estado de sus turnos (ej. "Atendido", "Cancelado").
- **Historias Clínicas:** Puede ver y registrar diagnósticos y tratamientos para los pacientes que atiende.
- **Privacidad:** No tiene acceso a las agendas de otros colegas.

### 4. Paciente
**Alcance:** Personal / Autogestión.
- **Mis Turnos:** Visualiza únicamente su propio historial de turnos.
- **Solicitar Turno:** Acceso a la función de "Solicitud Inteligente" que busca automáticamente el médico con mayor disponibilidad para una especialidad y fecha dada.
- **Datos:** Puede ver sus propios datos de contacto.

## Tecnologías
- **Framework:** .NET 8 (ASP.NET Core MVC)
- **ORM:** Entity Framework Core (Code First)
- **Base de Datos:** SQL Server
- **Frontend:** Bootstrap 5, SweetAlert2, DataTables
- **Autenticación:** ASP.NET Core Identity

## Instalación y Ejecución

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repo>
   cd TurnosMedicos
   ```

2. **Configurar Base de Datos**
   Asegúrate de tener SQL Server corriendo. La cadena de conexión por defecto apunta a `(localdb)\mssqllocaldb`. Puedes cambiarla en `appsettings.json`.

3. **Aplicar Migraciones y Seed**
   Al ejecutar la aplicación por primera vez, el sistema aplicará automáticamente las migraciones y cargará los datos de prueba (Seed).

4. **Ejecutar**
   ```bash
   dotnet run
   ```
   O abrir la solución `TurnosMedicos.sln` en Visual Studio y presionar F5.

## Usuarios de Prueba (Seed)
- **Admin:** `admin@turnos.com` / `Admin123!`

## Funcionalidades Principales
- **Gestión de Turnos:** Creación, edición y cancelación.
- **Solicitud Inteligente:** Procedimiento almacenado para asignar turnos automáticamente según disponibilidad.
- **Historias Clínicas:** Registro de diagnósticos y tratamientos.
- **Seguridad:** Control de acceso basado en roles (RBAC).
- **Reportes:** Visualización de datos en tablas interactivas.
