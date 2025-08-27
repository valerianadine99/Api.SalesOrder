# API SalesOrder - Ejecución Sin Docker

## Descripción
Esta es la API SalesOrder construida en .NET 8 con Clean Architecture, configurada para ejecutarse directamente en el sistema sin necesidad de Docker.

## Requisitos Previos
- .NET 8 SDK instalado
- SQL Server (Azure SQL Database o local)
- Visual Studio 2022, VS Code, o cualquier editor compatible con .NET

## Configuración de la Base de Datos
Antes de ejecutar la aplicación, asegúrate de configurar la base de datos:

1. Ejecuta los scripts SQL en tu base de datos:
   - `DatabaseScripts.sql` (recomendado para instalación completa)
   - `DatabaseScripts_StepByStep.sql` (para instalación paso a paso)

2. Actualiza la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:dinet-staging-sql.database.windows.net,1433;Initial Catalog=dinet-staging-sql;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";"
  }
}
```

## Ejecución de la Aplicación

### Opción 1: Visual Studio 2022
1. Abre la solución `Api.SalesOrder.sln`
2. Selecciona el proyecto `Api.SalesOrder` como proyecto de inicio
3. Presiona F5 o haz clic en "Start Debugging"

### Opción 2: Visual Studio Code
1. Abre la carpeta del proyecto en VS Code
2. Instala la extensión C# si no la tienes
3. Presiona F5 o usa la paleta de comandos (Ctrl+Shift+P) y selecciona "Debug: Start Debugging"

### Opción 3: Línea de Comandos
1. Abre una terminal en la carpeta `Api.SalesOrder`
2. Ejecuta los siguientes comandos:

```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

### Opción 4: Publicar y Ejecutar
```bash
# Publicar la aplicación
dotnet publish -c Release -o ./publish

# Ejecutar desde la carpeta publicada
cd publish
dotnet Api.SalesOrder.dll
```

## Configuración de Puertos
La aplicación se ejecutará en los siguientes puertos por defecto:
- HTTP: http://localhost:5154
- HTTPS: https://localhost:7028

Puedes cambiar estos puertos en `Properties/launchSettings.json` o en `appsettings.json`.

## Acceso a la API
Una vez ejecutada la aplicación:
- **Swagger UI**: https://localhost:7028/swagger
- **API Base URL**: https://localhost:7028/api

## Estructura del Proyecto
```
Api.SalesOrder/
├── Controllers/          # Controladores de la API
├── Models/              # Modelos de respuesta
├── Services/            # Servicios de la aplicación
├── Middleware/          # Middleware personalizado
├── Properties/          # Configuraciones de lanzamiento
├── Program.cs           # Punto de entrada de la aplicación
├── appsettings.json     # Configuración de la aplicación
└── Api.SalesOrder.API.csproj  # Archivo de proyecto
```

## Características de la API
- ✅ **Clean Architecture** con separación de responsabilidades
- ✅ **CQRS Pattern** para operaciones de lectura y escritura
- ✅ **JWT Authentication** para seguridad
- ✅ **Swagger/OpenAPI** para documentación
- ✅ **Entity Framework Core** para acceso a datos
- ✅ **FluentValidation** para validaciones
- ✅ **AutoMapper** para mapeo de objetos
- ✅ **MediatR** para implementación de CQRS

## Endpoints Principales
- `GET /api/orders` - Obtener todas las órdenes
- `GET /api/orders/{id}` - Obtener orden por ID
- `POST /api/orders` - Crear nueva orden
- `PUT /api/orders/{id}` - Actualizar orden existente
- `DELETE /api/orders/{id}` - Eliminar orden (soft delete)

## Solución de Problemas

### Error de Conexión a Base de Datos
- Verifica que la cadena de conexión sea correcta
- Asegúrate de que la base de datos esté accesible
- Verifica que los scripts SQL se hayan ejecutado correctamente

### Error de Compilación
- Ejecuta `dotnet restore` para restaurar dependencias
- Verifica que tengas .NET 8 SDK instalado
- Limpia la solución con `dotnet clean`

### Error de Ejecución
- Verifica que no haya otro proceso usando los puertos configurados
- Revisa los logs de la aplicación
- Verifica la configuración en `appsettings.json`

## Desarrollo
Para desarrollo local, puedes usar:
- **Base de datos local**: SQL Server Express o LocalDB
- **Herramientas**: SQL Server Management Studio, Azure Data Studio
- **Testing**: Postman, Insomnia, o cualquier cliente HTTP

## Notas Adicionales
- La aplicación está configurada para ejecutarse en modo Development por defecto
- Los logs se muestran en la consola durante el desarrollo
- Swagger está habilitado solo en modo Development por seguridad
- La autenticación JWT está configurada pero puede requerir configuración adicional para producción

## Soporte
Si encuentras problemas:
1. Verifica que todos los requisitos previos estén cumplidos
2. Revisa la configuración de la base de datos
3. Verifica los logs de la aplicación
4. Asegúrate de que los scripts SQL se hayan ejecutado correctamente
