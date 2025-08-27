# Configuración de Base de Datos - API SalesOrder

## Descripción
Este documento contiene las instrucciones para configurar la base de datos SQL Server para la API SalesOrder construida en .NET 8 con Clean Architecture.

## Requisitos Previos
- SQL Server (Azure SQL Database o local)
- Permisos de administrador en la base de datos
- Herramienta para ejecutar scripts SQL (SQL Server Management Studio, Azure Data Studio, etc.)

## Cadena de Conexión
```
Server=tcp:dinet-staging-sql.database.windows.net,1433;
Initial Catalog=dinet-staging-sql;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
Authentication="Active Directory Default";
```

## Estructura de la Base de Datos

### Tablas Principales

#### 1. Orders
- **Id**: Clave primaria auto-incremental
- **OrderDate**: Fecha de la orden
- **CustomerName**: Nombre del cliente (máx. 100 caracteres)
- **CustomerEmail**: Email del cliente (máx. 150 caracteres)
- **Total**: Total de la orden (decimal 18,2)
- **Status**: Estado de la orden (Pending, Confirmed, Shipped, Delivered, Cancelled)
- **CreatedAt**: Fecha de creación
- **UpdatedAt**: Fecha de última actualización
- **CreatedBy**: Usuario que creó el registro
- **UpdatedBy**: Usuario que actualizó por última vez
- **IsDeleted**: Flag de eliminación lógica

#### 2. OrderDetails
- **Id**: Clave primaria auto-incremental
- **OrderId**: Clave foránea a Orders
- **ProductName**: Nombre del producto (máx. 200 caracteres)
- **ProductCode**: Código del producto (máx. 50 caracteres)
- **Quantity**: Cantidad del producto
- **UnitPrice**: Precio unitario (decimal 18,2)
- **Subtotal**: Subtotal del detalle (decimal 18,2)
- **CreatedAt**: Fecha de creación
- **UpdatedAt**: Fecha de última actualización
- **CreatedBy**: Usuario que creó el registro
- **UpdatedBy**: Usuario que actualizó por última vez
- **IsDeleted**: Flag de eliminación lógica

#### 3. OrderStatuses
- **Id**: Identificador del estado
- **Name**: Nombre del estado

## Instrucciones de Instalación

### Opción 1: Ejecución Completa (Recomendado)
1. Conectarse a la base de datos usando la cadena de conexión
2. Ejecutar el archivo `DatabaseScripts.sql` completo
3. Verificar que todas las tablas, vistas, stored procedures y funciones se hayan creado correctamente

### Opción 2: Ejecución Paso a Paso
Si prefieres ejecutar los scripts por separado, puedes usar el archivo `DatabaseScripts_StepByStep.sql`

## Características Implementadas

### Índices de Rendimiento
- Índice compuesto en Orders (CustomerName + OrderDate)
- Índice en ProductCode de OrderDetails
- Índice en OrderId de OrderDetails
- Índice en Status de Orders
- Índices en IsDeleted para ambas tablas

### Constraints de Integridad
- Foreign Key entre OrderDetails y Orders con eliminación en cascada
- Check constraints para valores positivos en Total, Quantity, UnitPrice y Subtotal
- Validación de que Quantity sea mayor a 0

### Vistas Útiles
- **vw_OrdersWithDetails**: Vista que combina órdenes con sus detalles
- **vw_OrderSummaryByStatus**: Resumen de órdenes agrupadas por estado

### Stored Procedures
- **sp_GetOrders**: Obtiene órdenes con filtros y paginación
- **sp_GetOrderWithDetails**: Obtiene una orden específica con sus detalles

### Funciones
- **fn_CalculateOrderTotal**: Calcula el total de una orden basado en sus detalles

### Triggers
- **TR_Orders_UpdateTimestamp**: Actualiza automáticamente UpdatedAt en Orders
- **TR_OrderDetails_UpdateTimestamp**: Actualiza automáticamente UpdatedAt en OrderDetails

## Datos de Prueba
El script incluye la inserción automática de datos de prueba:
- Una orden de ejemplo para "John Doe"
- Dos productos: Laptop y Mouse
- Total de la orden: $1,500.00

## Verificación de la Instalación
Después de ejecutar los scripts, puedes verificar que todo se haya creado correctamente ejecutando:

```sql
-- Verificar tablas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME IN ('Orders', 'OrderDetails', 'OrderStatuses');

-- Verificar vistas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME LIKE 'vw_%';

-- Verificar stored procedures
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME LIKE 'sp_%';

-- Verificar funciones
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' AND ROUTINE_NAME LIKE 'fn_%';
```

## Consideraciones de Seguridad
- Los scripts incluyen validaciones para evitar la inserción de datos duplicados
- Se implementa eliminación lógica (soft delete) en lugar de eliminación física
- Los triggers mantienen automáticamente los timestamps de auditoría
- Se incluyen constraints para mantener la integridad de los datos

## Soporte
Si encuentras algún problema durante la instalación, verifica:
1. Que tengas permisos de administrador en la base de datos
2. Que la base de datos esté accesible desde tu ubicación
3. Que no haya conflictos con objetos existentes (el script verifica esto automáticamente)

## Notas Adicionales
- El script es idempotente: puede ejecutarse múltiples veces sin errores
- Se incluyen validaciones para evitar la creación duplicada de objetos
- Los índices están optimizados para las consultas más comunes de la aplicación
- Se implementa auditoría completa con timestamps y usuarios
