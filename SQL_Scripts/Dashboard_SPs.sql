/* ============================================================================
   SISTEMA SEMANA 11 - Dashboard Analytics (Sprint 1)
   Script de creacion de Stored Procedures para el modulo Dashboard
   Base de datos: mydatabase2
   Ejecutable en SSMS (SQL Server 2016 SP1 o superior)

   Nota sobre el filtro de documentos aceptados:
     - venta.estado  -> se registra con el valor 1 (aceptada) y 'Anulado'
     - ingreso.estado-> se registra con 'Aceptado' y 'Anulado'
     Por eso el filtro de negocio es:  ISNULL(estado,'') <> 'Anulado'
   ==========================================================================*/

USE [mydatabase2];
GO

/* ==========================================================================
   1) sp_dashboard_kpis
   Devuelve 1 sola fila con los 6 KPIs del periodo seleccionado.
   @IdUsuario = NULL  -> todos los usuarios (Administrador)
   @IdCategoria = NULL-> todas las categorias
   ==========================================================================*/
IF OBJECT_ID('dbo.sp_dashboard_kpis', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_dashboard_kpis;
GO
CREATE PROCEDURE dbo.sp_dashboard_kpis
(
    @FechaInicio DATE,
    @FechaFin    DATE,
    @IdUsuario   INT = NULL,
    @IdCategoria INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH VentasPeriodo AS
    (
        SELECT ISNULL(SUM(v.total), 0) AS VentasTotales,
               COUNT(*)                AS NumTransacciones
        FROM venta v
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= @FechaInicio
          AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
          AND (@IdCategoria IS NULL OR EXISTS
               (
                   SELECT 1
                   FROM detalle_venta dv
                   INNER JOIN articulo a ON a.idarticulo = dv.idarticulo
                   WHERE dv.idventa = v.idventa
                     AND a.idcategoria = @IdCategoria
               ))
    ),
    IngresosPeriodo AS
    (
        SELECT ISNULL(SUM(i.total), 0) AS IngresosTotales
        FROM ingreso i
        WHERE ISNULL(i.estado, '') <> 'Anulado'
          AND i.fecha >= @FechaInicio
          AND i.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR i.idusuario = @IdUsuario)
    ),
    VentasNetas AS
    (
        SELECT ISNULL(SUM(dv.cantidad * dv.precio - dv.descuento), 0) AS TotalNeto
        FROM detalle_venta dv
        INNER JOIN venta v ON v.idventa = dv.idventa
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= @FechaInicio
          AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
          AND (@IdCategoria IS NULL OR dv.idarticulo IN
               (SELECT idarticulo FROM articulo WHERE idcategoria = @IdCategoria))
    ),
    CostoPeriodo AS
    (
        -- Costo estimado de lo vendido: cantidad vendida x precio promedio
        -- de compra (ponderado) del articulo segun los ingresos aceptados.
        SELECT ISNULL(SUM(dv.cantidad * ISNULL(c.PrecioPromedio, 0)), 0) AS CostoEstimado
        FROM detalle_venta dv
        INNER JOIN venta v ON v.idventa = dv.idventa
        OUTER APPLY
        (
            SELECT SUM(di.cantidad * di.precio) / NULLIF(SUM(di.cantidad), 0) AS PrecioPromedio
            FROM detalle_ingreso di
            INNER JOIN ingreso i ON i.idingreso = di.idingreso
            WHERE di.idarticulo = dv.idarticulo
              AND ISNULL(i.estado, '') <> 'Anulado'
        ) c
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= @FechaInicio
          AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
          AND (@IdCategoria IS NULL OR dv.idarticulo IN
               (SELECT idarticulo FROM articulo WHERE idcategoria = @IdCategoria))
    ),
    VentasPeriodoAnterior AS
    (
        -- Mismo tamanho de periodo inmediatamente anterior
        SELECT ISNULL(SUM(v.total), 0) AS VentasTotales
        FROM venta v
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= DATEADD(DAY, -DATEDIFF(DAY, @FechaInicio, @FechaFin) - 1, @FechaInicio)
          AND v.fecha <  @FechaInicio
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
          AND (@IdCategoria IS NULL OR EXISTS
               (
                   SELECT 1
                   FROM detalle_venta dv
                   INNER JOIN articulo a ON a.idarticulo = dv.idarticulo
                   WHERE dv.idventa = v.idventa
                     AND a.idcategoria = @IdCategoria
               ))
    )
    SELECT vp.VentasTotales,
           ip.IngresosTotales,
           CAST(CASE WHEN vp.NumTransacciones > 0
                THEN vp.VentasTotales / vp.NumTransacciones
                ELSE 0 END AS DECIMAL(18,2))                 AS TicketPromedio,
           vp.NumTransacciones,
           CAST(CASE WHEN vap.VentasTotales > 0
                THEN (vp.VentasTotales - vap.VentasTotales) * 100.0 / vap.VentasTotales
                ELSE 0 END AS DECIMAL(5,2))                  AS VariacionVsPeriodoAnterior,
           CAST(vn.TotalNeto - cp.CostoEstimado AS DECIMAL(18,2)) AS MargenBruto
    FROM VentasPeriodo vp
    CROSS JOIN IngresosPeriodo ip
    CROSS JOIN VentasNetas vn
    CROSS JOIN CostoPeriodo cp
    CROSS JOIN VentasPeriodoAnterior vap;
END
GO

/* ==========================================================================
   2) sp_dashboard_tendencia
   Serie continua de ventas/ingresos por dia o por mes (sin huecos).
   @Granularidad: 'DIA' | 'MES'
   ==========================================================================*/
IF OBJECT_ID('dbo.sp_dashboard_tendencia', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_dashboard_tendencia;
GO
CREATE PROCEDURE dbo.sp_dashboard_tendencia
(
    @FechaInicio  DATE,
    @FechaFin     DATE,
    @IdUsuario    INT = NULL,
    @IdCategoria  INT = NULL,
    @Granularidad VARCHAR(10) = 'DIA'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Granularidad NOT IN ('DIA', 'MES')
        SET @Granularidad = 'DIA';

    -- Tally 0..99999 para generar la serie de fechas sin recursin
    ;WITH Digitos AS
    (
        SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3
        UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7
        UNION ALL SELECT 8 UNION ALL SELECT 9
    ),
    Tally AS
    (
        SELECT d0.n + d1.n * 10 + d2.n * 100 + d3.n * 1000 + d4.n * 10000 AS n
        FROM Digitos d0
        CROSS JOIN Digitos d1
        CROSS JOIN Digitos d2
        CROSS JOIN Digitos d3
        CROSS JOIN Digitos d4
    ),
    Serie AS
    (
        SELECT CASE
                   WHEN @Granularidad = 'MES'
                       THEN DATEADD(MONTH, n, DATEADD(MONTH, DATEDIFF(MONTH, 0, @FechaInicio), 0))
                   ELSE DATEADD(DAY, n, @FechaInicio)
               END AS Fecha
        FROM Tally
        WHERE n <= CASE
                       WHEN @Granularidad = 'MES'
                           THEN DATEDIFF(MONTH, DATEADD(MONTH, DATEDIFF(MONTH, 0, @FechaInicio), 0),
                                              DATEADD(MONTH, DATEDIFF(MONTH, 0, @FechaFin), 0))
                       ELSE DATEDIFF(DAY, @FechaInicio, @FechaFin)
                   END
    ),
    PeriodoVentas AS
    (
        SELECT CASE
                   WHEN @Granularidad = 'MES' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, v.fecha), 0)
                   ELSE CAST(v.fecha AS DATE)
               END AS Fecha,
               SUM(v.total) AS Ventas
        FROM venta v
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= @FechaInicio
          AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
          AND (@IdCategoria IS NULL OR EXISTS
               (
                   SELECT 1
                   FROM detalle_venta dv
                   INNER JOIN articulo a ON a.idarticulo = dv.idarticulo
                   WHERE dv.idventa = v.idventa
                     AND a.idcategoria = @IdCategoria
               ))
        GROUP BY CASE
                     WHEN @Granularidad = 'MES' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, v.fecha), 0)
                     ELSE CAST(v.fecha AS DATE)
                 END
    ),
    PeriodoIngresos AS
    (
        SELECT CASE
                   WHEN @Granularidad = 'MES' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, i.fecha), 0)
                   ELSE CAST(i.fecha AS DATE)
               END AS Fecha,
               SUM(i.total) AS Ingresos
        FROM ingreso i
        WHERE ISNULL(i.estado, '') <> 'Anulado'
          AND i.fecha >= @FechaInicio
          AND i.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR i.idusuario = @IdUsuario)
        GROUP BY CASE
                     WHEN @Granularidad = 'MES' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, i.fecha), 0)
                     ELSE CAST(i.fecha AS DATE)
                 END
    )
    SELECT s.Fecha,
           CASE
               WHEN @Granularidad = 'MES' THEN FORMAT(s.Fecha, 'MMM yyyy')
               ELSE FORMAT(s.Fecha, 'dd/MMM')
           END AS PeriodoLabel,
           ISNULL(pv.Ventas, 0)    AS Ventas,
           ISNULL(pi.Ingresos, 0)  AS Ingresos
    FROM Serie s
    LEFT JOIN PeriodoVentas pv   ON pv.Fecha = s.Fecha
    LEFT JOIN PeriodoIngresos pi ON pi.Fecha = s.Fecha
    ORDER BY s.Fecha;
END
GO

/* ==========================================================================
   3) sp_dashboard_top_productos
   Top 10 articulos mas vendidos por cantidad o por monto.
   @Metrica: 'MONTO' | 'CANTIDAD'
   ==========================================================================*/
IF OBJECT_ID('dbo.sp_dashboard_top_productos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_dashboard_top_productos;
GO
CREATE PROCEDURE dbo.sp_dashboard_top_productos
(
    @FechaInicio DATE,
    @FechaFin    DATE,
    @IdUsuario   INT = NULL,
    @IdCategoria INT = NULL,
    @Metrica     VARCHAR(10) = 'MONTO',
    @Top         INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Metrica NOT IN ('MONTO', 'CANTIDAD')
        SET @Metrica = 'MONTO';

    SELECT TOP (@Top)
           a.idarticulo                             AS IdArticulo,
           a.codigo                                 AS Codigo,
           a.nombre                                 AS Nombre,
           c.nombre                                 AS Categoria,
           SUM(dv.cantidad)                         AS TotalCantidad,
           SUM(dv.cantidad * dv.precio - dv.descuento) AS TotalMonto
    FROM detalle_venta dv
    INNER JOIN venta v     ON v.idventa = dv.idventa
    INNER JOIN articulo a  ON a.idarticulo = dv.idarticulo
    INNER JOIN categoria c ON c.idcategoria = a.idcategoria
    WHERE ISNULL(v.estado, '') <> 'Anulado'
      AND v.fecha >= @FechaInicio
      AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
      AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
      AND (@IdCategoria IS NULL OR a.idcategoria = @IdCategoria)
    GROUP BY a.idarticulo, a.codigo, a.nombre, c.nombre
    ORDER BY CASE
                 WHEN @Metrica = 'MONTO'     THEN SUM(dv.cantidad * dv.precio - dv.descuento)
                 ELSE SUM(dv.cantidad) * 1.0
             END DESC,
             SUM(dv.cantidad) DESC;
END
GO

/* ==========================================================================
   4) sp_dashboard_categoria_distribucion
   Distribucion por categoria (% sobre los ingresos por ventas del periodo).
   ==========================================================================*/
IF OBJECT_ID('dbo.sp_dashboard_categoria_distribucion', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_dashboard_categoria_distribucion;
GO
CREATE PROCEDURE dbo.sp_dashboard_categoria_distribucion
(
    @FechaInicio DATE,
    @FechaFin    DATE,
    @IdUsuario   INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH Base AS
    (
        SELECT a.idcategoria,
               dv.cantidad,
               dv.cantidad * dv.precio - dv.descuento AS Monto
        FROM detalle_venta dv
        INNER JOIN venta v     ON v.idventa = dv.idventa
        INNER JOIN articulo a  ON a.idarticulo = dv.idarticulo
        WHERE ISNULL(v.estado, '') <> 'Anulado'
          AND v.fecha >= @FechaInicio
          AND v.fecha <  DATEADD(DAY, 1, @FechaFin)
          AND (@IdUsuario IS NULL OR v.idusuario = @IdUsuario)
    ),
    Total AS
    (
        SELECT ISNULL(SUM(Monto), 0) AS TotalGeneral
        FROM Base
    )
    SELECT c.idcategoria                   AS IdCategoria,
           c.nombre                        AS Nombre,
           ISNULL(SUM(b.Monto), 0)         AS TotalIngresos,
           ISNULL(SUM(b.cantidad), 0)      AS TotalCantidad,
           CAST(ISNULL(SUM(b.Monto), 0) * 100.0
                / NULLIF(t.TotalGeneral, 0) AS DECIMAL(5,2)) AS Porcentaje
    FROM categoria c
    INNER JOIN Base b ON b.idcategoria = c.idcategoria
    CROSS JOIN Total t
    WHERE c.estado = 1
    GROUP BY c.idcategoria, c.nombre, t.TotalGeneral
    ORDER BY TotalIngresos DESC;
END
GO

/* ==========================================================================
   5) sp_dashboard_filtros_vendedores_categorias
   Devuelve 2 result sets:
     ResultSet 0 = vendedores (seguros por rol: @IdUsuario NULL = todos)
     ResultSet 1 = categorias activas
   ==========================================================================*/
IF OBJECT_ID('dbo.sp_dashboard_filtros_vendedores_categorias', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_dashboard_filtros_vendedores_categorias;
GO
CREATE PROCEDURE dbo.sp_dashboard_filtros_vendedores_categorias
(
    @IdUsuario INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT u.idusuario                AS Id,
           LTRIM(RTRIM(u.nombre))     AS NombreCompleto
    FROM usuario u
    INNER JOIN rol r ON r.idrol = u.idrol
    WHERE u.estado = 1
      AND r.nombre IN ('Administrador', 'Vendedor')
      AND (@IdUsuario IS NULL OR u.idusuario = @IdUsuario)
    ORDER BY u.nombre;

    SELECT c.idcategoria AS Id,
           c.nombre      AS Nombre
    FROM categoria c
    WHERE c.estado = 1
    ORDER BY c.nombre;
END
GO

/* ==========================================================================
   Indices sugeridos para el dashboard (idempotentes)
   ==========================================================================*/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_venta_fecha_estado' AND object_id = OBJECT_ID('dbo.venta'))
    CREATE NONCLUSTERED INDEX IX_venta_fecha_estado
        ON dbo.venta (estado, fecha) INCLUDE (idusuario, total);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_venta_usuario_fecha' AND object_id = OBJECT_ID('dbo.venta'))
    CREATE NONCLUSTERED INDEX IX_venta_usuario_fecha
        ON dbo.venta (idusuario, fecha) INCLUDE (estado, total);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ingreso_fecha_estado' AND object_id = OBJECT_ID('dbo.ingreso'))
    CREATE NONCLUSTERED INDEX IX_ingreso_fecha_estado
        ON dbo.ingreso (estado, fecha) INCLUDE (idusuario, total);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_detalle_venta_venta' AND object_id = OBJECT_ID('dbo.detalle_venta'))
    CREATE NONCLUSTERED INDEX IX_detalle_venta_venta
        ON dbo.detalle_venta (idventa) INCLUDE (idarticulo, cantidad, precio, descuento);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_detalle_ingreso_articulo' AND object_id = OBJECT_ID('dbo.detalle_ingreso'))
    CREATE NONCLUSTERED INDEX IX_detalle_ingreso_articulo
        ON dbo.detalle_ingreso (idarticulo) INCLUDE (idingreso, cantidad, precio);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_articulo_categoria' AND object_id = OBJECT_ID('dbo.articulo'))
    CREATE NONCLUSTERED INDEX IX_articulo_categoria
        ON dbo.articulo (idcategoria) INCLUDE (codigo, nombre);
GO
