-- =============================================
-- Scripts SQL Paso a Paso para crear la base de datos SalesOrder
-- Ejecutar cada sección por separado según sea necesario
-- =============================================

-- =============================================
-- PASO 1: Crear tabla de estados de orden
-- =============================================
USE [dinet-staging-sql];
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderStatuses](
        [Id] [int] NOT NULL,
        [Name] [nvarchar](50) NOT NULL,
        CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    -- Insertar datos del enum OrderStatus
    INSERT INTO [dbo].[OrderStatuses] ([Id], [Name]) VALUES 
    (1, 'Pending'),
    (2, 'Confirmed'),
    (3, 'Shipped'),
    (4, 'Delivered'),
    (5, 'Cancelled');
    
    PRINT 'Tabla OrderStatuses creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla OrderStatuses ya existe';
END
GO

-- =============================================
-- PASO 2: Crear tabla de órdenes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [OrderDate] [datetime2](7) NOT NULL,
        [CustomerName] [nvarchar](100) NOT NULL,
        [CustomerEmail] [nvarchar](150) NOT NULL,
        [Total] [decimal](18,2) NOT NULL,
        [Status] [nvarchar](50) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NULL,
        [CreatedBy] [nvarchar](100) NOT NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Tabla Orders creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Orders ya existe';
END
GO

-- =============================================
-- PASO 3: Crear tabla de detalles de orden
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderDetails](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [OrderId] [int] NOT NULL,
        [ProductName] [nvarchar](200) NOT NULL,
        [ProductCode] [nvarchar](50) NOT NULL,
        [Quantity] [int] NOT NULL,
        [UnitPrice] [decimal](18,2) NOT NULL,
        [Subtotal] [decimal](18,2) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NULL,
        [CreatedBy] [nvarchar](100) NOT NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Tabla OrderDetails creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla OrderDetails ya existe';
END
GO

-- =============================================
-- PASO 4: Crear Foreign Key entre OrderDetails y Orders
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderDetails_Orders]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderDetails]'))
BEGIN
    ALTER TABLE [dbo].[OrderDetails] 
    ADD CONSTRAINT [FK_OrderDetails_Orders] 
    FOREIGN KEY([OrderId]) REFERENCES [dbo].[Orders] ([Id]) 
    ON DELETE CASCADE;
    
    PRINT 'Foreign Key FK_OrderDetails_Orders creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Foreign Key FK_OrderDetails_Orders ya existe';
END
GO

-- =============================================
-- PASO 5: Crear índices para mejorar el rendimiento
-- =============================================

-- Índice compuesto para Orders (CustomerName + OrderDate)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = N'IX_Orders_Customer_Date')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Orders_Customer_Date] ON [dbo].[Orders]
    (
        [CustomerName] ASC,
        [OrderDate] ASC
    );
    PRINT 'Índice IX_Orders_Customer_Date creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_Orders_Customer_Date ya existe';
END
GO

-- Índice para ProductCode en OrderDetails
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = N'IX_OrderDetails_ProductCode')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductCode] ON [dbo].[OrderDetails]
    (
        [ProductCode] ASC
    );
    PRINT 'Índice IX_OrderDetails_ProductCode creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_OrderDetails_ProductCode ya existe';
END
GO

-- Índice para OrderId en OrderDetails
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = N'IX_OrderDetails_OrderId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId] ON [dbo].[OrderDetails]
    (
        [OrderId] ASC
    );
    PRINT 'Índice IX_OrderDetails_OrderId creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_OrderDetails_OrderId ya existe';
END
GO

-- Índice para Status en Orders
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = N'IX_Orders_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Orders_Status] ON [dbo].[Orders]
    (
        [Status] ASC
    );
    PRINT 'Índice IX_Orders_Status creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_Orders_Status ya existe';
END
GO

-- Índices para IsDeleted
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = N'IX_Orders_IsDeleted')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Orders_IsDeleted] ON [dbo].[Orders]
    (
        [IsDeleted] ASC
    );
    PRINT 'Índice IX_Orders_IsDeleted creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_Orders_IsDeleted ya existe';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = N'IX_OrderDetails_IsDeleted')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OrderDetails_IsDeleted] ON [dbo].[OrderDetails]
    (
        [IsDeleted] ASC
    );
    PRINT 'Índice IX_OrderDetails_IsDeleted creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Índice IX_OrderDetails_IsDeleted ya existe';
END
GO

-- =============================================
-- PASO 6: Crear constraints adicionales
-- =============================================

-- Constraint para asegurar que Total sea mayor o igual a 0
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_Orders_Total_Positive]'))
BEGIN
    ALTER TABLE [dbo].[Orders] 
    ADD CONSTRAINT [CK_Orders_Total_Positive] 
    CHECK ([Total] >= 0);
    PRINT 'Constraint CK_Orders_Total_Positive creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Constraint CK_Orders_Total_Positive ya existe';
END
GO

-- Constraint para asegurar que Quantity sea mayor a 0
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_OrderDetails_Quantity_Positive]'))
BEGIN
    ALTER TABLE [dbo].[OrderDetails] 
    ADD CONSTRAINT [CK_OrderDetails_Quantity_Positive] 
    CHECK ([Quantity] > 0);
    PRINT 'Constraint CK_OrderDetails_Quantity_Positive creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Constraint CK_OrderDetails_Quantity_Positive ya existe';
END
GO

-- Constraint para asegurar que UnitPrice sea mayor o igual a 0
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_OrderDetails_UnitPrice_Positive]'))
BEGIN
    ALTER TABLE [dbo].[OrderDetails] 
    ADD CONSTRAINT [CK_OrderDetails_UnitPrice_Positive] 
    CHECK ([UnitPrice] >= 0);
    PRINT 'Constraint CK_OrderDetails_UnitPrice_Positive creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Constraint CK_OrderDetails_UnitPrice_Positive ya existe';
END
GO

-- Constraint para asegurar que Subtotal sea mayor o igual a 0
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[dbo].[CK_OrderDetails_Subtotal_Positive]'))
BEGIN
    ALTER TABLE [dbo].[OrderDetails] 
    ADD CONSTRAINT [CK_OrderDetails_Subtotal_Positive] 
    CHECK ([Subtotal] >= 0);
    PRINT 'Constraint CK_OrderDetails_Subtotal_Positive creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Constraint CK_OrderDetails_Subtotal_Positive ya existe';
END
GO

-- =============================================
-- PASO 7: Crear vistas útiles
-- =============================================

-- Vista para órdenes con detalles
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_OrdersWithDetails]'))
    DROP VIEW [dbo].[vw_OrdersWithDetails];
GO

CREATE VIEW [dbo].[vw_OrdersWithDetails] AS
SELECT 
    o.Id,
    o.OrderDate,
    o.CustomerName,
    o.CustomerEmail,
    o.Total,
    o.Status,
    o.CreatedAt,
    o.UpdatedAt,
    o.CreatedBy,
    o.UpdatedBy,
    o.IsDeleted,
    od.Id AS OrderDetailId,
    od.ProductName,
    od.ProductCode,
    od.Quantity,
    od.UnitPrice,
    od.Subtotal
FROM [dbo].[Orders] o
LEFT JOIN [dbo].[OrderDetails] od ON o.Id = od.OrderId AND od.IsDeleted = 0
WHERE o.IsDeleted = 0;
GO

PRINT 'Vista vw_OrdersWithDetails creada exitosamente';

-- Vista para resumen de órdenes por estado
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_OrderSummaryByStatus]'))
    DROP VIEW [dbo].[vw_OrderSummaryByStatus];
GO

CREATE VIEW [dbo].[vw_OrderSummaryByStatus] AS
SELECT 
    Status,
    COUNT(*) AS OrderCount,
    SUM(Total) AS TotalAmount,
    AVG(Total) AS AverageOrderValue,
    MIN(OrderDate) AS FirstOrderDate,
    MAX(OrderDate) AS LastOrderDate
FROM [dbo].[Orders]
WHERE IsDeleted = 0
GROUP BY Status;
GO

PRINT 'Vista vw_OrderSummaryByStatus creada exitosamente';

-- =============================================
-- PASO 8: Crear stored procedures útiles
-- =============================================

-- SP para obtener órdenes con filtros
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOrders]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetOrders];
GO

CREATE PROCEDURE [dbo].[sp_GetOrders]
    @CustomerName NVARCHAR(100) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @Status NVARCHAR(50) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        o.Id,
        o.OrderDate,
        o.CustomerName,
        o.CustomerEmail,
        o.Total,
        o.Status,
        o.CreatedAt,
        o.UpdatedAt,
        o.CreatedBy,
        o.UpdatedBy
    FROM [dbo].[Orders] o
    WHERE o.IsDeleted = 0
        AND (@CustomerName IS NULL OR o.CustomerName LIKE '%' + @CustomerName + '%')
        AND (@StartDate IS NULL OR o.OrderDate >= @StartDate)
        AND (@EndDate IS NULL OR o.OrderDate <= @EndDate)
        AND (@Status IS NULL OR o.Status = @Status)
    ORDER BY o.OrderDate DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Retornar el total de registros para paginación
    SELECT COUNT(*) AS TotalCount
    FROM [dbo].[Orders] o
    WHERE o.IsDeleted = 0
        AND (@CustomerName IS NULL OR o.CustomerName LIKE '%' + @CustomerName + '%')
        AND (@StartDate IS NULL OR o.OrderDate >= @StartDate)
        AND (@EndDate IS NULL OR o.OrderDate <= @EndDate)
        AND (@Status IS NULL OR o.Status = @Status);
END
GO

PRINT 'Stored Procedure sp_GetOrders creado exitosamente';

-- SP para obtener una orden con sus detalles
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOrderWithDetails]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_GetOrderWithDetails];
GO

CREATE PROCEDURE [dbo].[sp_GetOrderWithDetails]
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Obtener la orden
    SELECT 
        Id,
        OrderDate,
        CustomerName,
        CustomerEmail,
        Total,
        Status,
        CreatedAt,
        UpdatedAt,
        CreatedBy,
        UpdatedBy
    FROM [dbo].[Orders]
    WHERE Id = @OrderId AND IsDeleted = 0;
    
    -- Obtener los detalles
    SELECT 
        Id,
        ProductName,
        ProductCode,
        Quantity,
        UnitPrice,
        Subtotal,
        CreatedAt,
        UpdatedAt,
        CreatedBy,
        UpdatedBy
    FROM [dbo].[OrderDetails]
    WHERE OrderId = @OrderId AND IsDeleted = 0;
END
GO

PRINT 'Stored Procedure sp_GetOrderWithDetails creado exitosamente';

-- =============================================
-- PASO 9: Crear funciones útiles
-- =============================================

-- Función para calcular el total de una orden
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_CalculateOrderTotal]') AND type in (N'FN', N'IF', N'TF'))
    DROP FUNCTION [dbo].[fn_CalculateOrderTotal];
GO

CREATE FUNCTION [dbo].[fn_CalculateOrderTotal](@OrderId INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @Total DECIMAL(18,2) = 0;
    
    SELECT @Total = ISNULL(SUM(Subtotal), 0)
    FROM [dbo].[OrderDetails]
    WHERE OrderId = @OrderId AND IsDeleted = 0;
    
    RETURN @Total;
END
GO

PRINT 'Función fn_CalculateOrderTotal creada exitosamente';

-- =============================================
-- PASO 10: Crear triggers para auditoría
-- =============================================

-- Trigger para actualizar UpdatedAt automáticamente en Orders
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_Orders_UpdateTimestamp]'))
    DROP TRIGGER [dbo].[TR_Orders_UpdateTimestamp];
GO

CREATE TRIGGER [dbo].[TR_Orders_UpdateTimestamp]
ON [dbo].[Orders]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Orders]
    SET UpdatedAt = GETUTCDATE()
    FROM [dbo].[Orders] o
    INNER JOIN inserted i ON o.Id = i.Id;
END
GO

PRINT 'Trigger TR_Orders_UpdateTimestamp creado exitosamente';

-- Trigger para actualizar UpdatedAt automáticamente en OrderDetails
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TR_OrderDetails_UpdateTimestamp]'))
    DROP TRIGGER [dbo].[TR_OrderDetails_UpdateTimestamp];
GO

CREATE TRIGGER [dbo].[TR_OrderDetails_UpdateTimestamp]
ON [dbo].[OrderDetails]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[OrderDetails]
    SET UpdatedAt = GETUTCDATE()
    FROM [dbo].[OrderDetails] od
    INNER JOIN inserted i ON od.Id = i.Id;
END
GO

PRINT 'Trigger TR_OrderDetails_UpdateTimestamp creado exitosamente';

-- =============================================
-- PASO 11: Insertar datos de prueba (opcional)
-- =============================================

-- Solo insertar si no hay datos existentes
IF NOT EXISTS (SELECT * FROM [dbo].[Orders])
BEGIN
    -- Insertar orden de prueba
    INSERT INTO [dbo].[Orders] (
        [OrderDate], [CustomerName], [CustomerEmail], [Total], [Status], 
        [CreatedAt], [CreatedBy], [IsDeleted]
    ) VALUES (
        DATEADD(day, -5, GETUTCDATE()), 
        'John Doe', 
        'john.doe@example.com', 
        1500.00, 
        'Pending', 
        DATEADD(day, -5, GETUTCDATE()), 
        'System', 
        0
    );
    
    DECLARE @OrderId INT = SCOPE_IDENTITY();
    
    -- Insertar detalles de la orden
    INSERT INTO [dbo].[OrderDetails] (
        [OrderId], [ProductName], [ProductCode], [Quantity], [UnitPrice], [Subtotal],
        [CreatedAt], [CreatedBy], [IsDeleted]
    ) VALUES 
    (@OrderId, 'Laptop', 'TECH-001', 1, 1000.00, 1000.00, DATEADD(day, -5, GETUTCDATE()), 'System', 0),
    (@OrderId, 'Mouse', 'TECH-002', 2, 250.00, 500.00, DATEADD(day, -5, GETUTCDATE()), 'System', 0);
    
    PRINT 'Datos de prueba insertados exitosamente';
END
ELSE
BEGIN
    PRINT 'Ya existen datos en la tabla Orders, no se insertaron datos de prueba';
END
GO

-- =============================================
-- PASO 12: Verificar la creación de todos los objetos
-- =============================================
PRINT '=============================================';
PRINT 'VERIFICACIÓN FINAL DE LA INSTALACIÓN';
PRINT '=============================================';

PRINT 'Verificando las tablas...';
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME IN ('Orders', 'OrderDetails', 'OrderStatuses');

PRINT 'Verificando las vistas...';
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
    AND TABLE_NAME LIKE 'vw_%';

PRINT 'Verificando los stored procedures...';
SELECT 
    ROUTINE_NAME,
    ROUTINE_TYPE
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' 
    AND ROUTINE_NAME LIKE 'sp_%';

PRINT 'Verificando las funciones...';
SELECT 
    ROUTINE_NAME,
    ROUTINE_TYPE
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' 
    AND ROUTINE_NAME LIKE 'fn_%';

PRINT '=============================================';
PRINT 'INSTALACIÓN COMPLETADA EXITOSAMENTE!';
PRINT '=============================================';
GO
