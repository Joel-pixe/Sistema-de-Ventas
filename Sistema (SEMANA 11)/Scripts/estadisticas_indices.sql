/* ============================================================================
   MÓDULO ESTADÍSTICAS — Índices recomendados (6)
   ----------------------------------------------------------------------------
   Objetivo: sostener los 7 SPs del módulo (Scripts/estadisticas_sps.sql)
   sin rastros (scans) sobre tablas grandes. Cada índice se crea solo si no
   existe, por lo que el script es idempotente.
   Índices ya existentes en la BD (no se recrean):
       IX_venta_fecha_estado, IX_venta_usuario_fecha,
       IX_ingreso_fecha_estado, IX_detalle_venta_venta, IX_articulo_categoria
   ============================================================================ */

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO

/* 1) Top productos / distribución / costo de KPI.
      Los accesos son por articulo (JOIN y GROUP BY) y leen cantidad, precio
      y descuento: cubrirlos en el índice evita lookups al heap del cluster. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_detalle_venta_articulo'
                 AND object_id = OBJECT_ID('dbo.detalle_venta'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_detalle_venta_articulo
        ON dbo.detalle_venta (idarticulo)
        INCLUDE (cantidad, precio, descuento);
END
GO

/* 2) Tendencia y KPIs de ingresos filtrados por vendedor.
      Predicado: estado <> 'Anulado' AND fecha en rango AND idusuario = @x */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_ingreso_usuario_fecha'
                 AND object_id = OBJECT_ID('dbo.ingreso'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ingreso_usuario_fecha
        ON dbo.ingreso (idusuario, fecha)
        INCLUDE (total, estado);
END
GO

/* 3) Costo estimado del KPI (sp_dashboard_kpis -> OUTER APPLY por articulo) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_detalle_ingreso_articulo'
                 AND object_id = OBJECT_ID('dbo.detalle_ingreso'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_detalle_ingreso_articulo
        ON dbo.detalle_ingreso (idarticulo)
        INCLUDE (cantidad, precio);
END
GO

/* 4) Alertas de stock (sp_estadisticas_stock_alertas)
      Índice filtrado: solo artículos activos, ordenado por stock para el
      ORDER BY de criticidad sin sort. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_articulo_stock_alerta'
                 AND object_id = OBJECT_ID('dbo.articulo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_articulo_stock_alerta
        ON dbo.articulo (stock)
        INCLUDE (codigo, nombre, idcategoria)
        WHERE estado = 1;
END
GO

/* 5) Filtro de vendedores (sp_dashboard_filtros_vendedores_categorias)
      Predicado: estado = 1 AND idrol IN (...) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_usuario_rol'
                 AND object_id = OBJECT_ID('dbo.usuario'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_usuario_rol
        ON dbo.usuario (idrol)
        INCLUDE (estado, nombre);
END
GO

/* 6) Categorías activas: filtros globales y distribución por categoría */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_categoria_estado'
                 AND object_id = OBJECT_ID('dbo.categoria'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_categoria_estado
        ON dbo.categoria (estado)
        INCLUDE (nombre);
END
GO
