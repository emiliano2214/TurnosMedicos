# TurnosMedicos

Sistema de gestión de turnos médicos desarrollado en ASP.NET Core 8 MVC.

## 📋 Descripción
Aplicación web para la administración de clínicas, permitiendo la gestión de:
- **Pacientes y Médicos**
- **Turnos y Agendas**
- **Historias Clínicas y Tratamientos**
- **Obras Sociales y Especialidades**

El sistema cuenta con roles diferenciados (Admin, Administrativo, Médico, Paciente) para asegurar la privacidad y el correcto flujo de trabajo.

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
