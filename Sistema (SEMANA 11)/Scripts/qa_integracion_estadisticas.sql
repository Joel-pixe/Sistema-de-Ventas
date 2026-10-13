SET NOCOUNT OFF;

/* ============================================================
   QA — MÓDULO ESTADÍSTICAS: integración BD (7 SPs + 6 índices)
   Nota: NOCOUNT OFF para poder leer "(N rows affected)" y comparar
   cantidades de filas entre granularidades.
   ============================================================ */

PRINT '=== A. EXISTENCIA DE LOS 7 SPs ===';
SELECT name AS SP, modify_date FROM sys.procedures
WHERE name IN ('sp_estadisticas_ventas_periodo','sp_estadisticas_stock_alertas',
               'sp_dashboard_kpis','sp_dashboard_tendencia','sp_dashboard_top_productos',
               'sp_dashboard_categoria_distribucion','sp_dashboard_filtros_vendedores_categorias')
ORDER BY name;

PRINT '=== B. PARÁMETROS DECLARADOS POR SP (vs SqlParameter de DEstadisticas.cs) ===';
SELECT OBJECT_NAME(p.object_id) AS SP, p.name AS Param, t.name AS Tipo,
       CASE WHEN p.has_default_value = 1 THEN 'SI' ELSE 'NO' END AS DefaultValue
FROM sys.parameters p
JOIN sys.types t ON t.user_type_id = p.user_type_id
WHERE OBJECT_NAME(p.object_id) IN ('sp_estadisticas_ventas_periodo','sp_estadisticas_stock_alertas',
               'sp_dashboard_kpis','sp_dashboard_tendencia','sp_dashboard_top_productos',
               'sp_dashboard_categoria_distribucion','sp_dashboard_filtros_vendedores_categorias')
ORDER BY SP, p.parameter_id;

PRINT '=== C. ÍNDICES RECOMENDADOS ===';
SELECT i.name AS Indice, OBJECT_NAME(i.object_id) AS Tabla, i.type_desc, i.is_disabled
FROM sys.indexes i
WHERE i.name IN ('IX_detalle_venta_articulo','IX_ingreso_usuario_fecha','IX_detalle_ingreso_articulo',
                 'IX_articulo_stock_alerta','IX_usuario_rol','IX_categoria_estado')
ORDER BY i.name;

PRINT '=== D. COLLATION ===';
SELECT CAST(DATABASEPROPERTYEX(DB_NAME(),'Collation') AS varchar(100)) AS Collation;

-- T-SQL no admite subconsultas como valor de parámetro de EXEC: se usan variables.
DECLARE @idu INT = (SELECT MIN(idusuario)  FROM dbo.usuario);
DECLARE @idc INT = (SELECT MIN(idcategoria) FROM dbo.categoria);

PRINT '=== E1. sp_dashboard_kpis (sin filtro) ===';
EXEC dbo.sp_dashboard_kpis @fechainicio='2026-07-01', @fechafin='2026-10-12', @idusuario=NULL, @idcategoria=NULL;

PRINT '=== E1b. sp_dashboard_kpis (filtro idusuario + idcategoria) ===';
EXEC dbo.sp_dashboard_kpis @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=@idu, @idcategoria=@idc;

PRINT '=== E1c. sp_dashboard_kpis (rango SIN datos) ===';
EXEC dbo.sp_dashboard_kpis @fechainicio='2019-01-01', @fechafin='2019-01-31', @idusuario=NULL, @idcategoria=NULL;

PRINT '=== E2. sp_dashboard_tendencia DIA (esperada: 104 filas 01/07-12/10) ===';
EXEC dbo.sp_dashboard_tendencia @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='DIA';

PRINT '=== E2b. sp_dashboard_tendencia SEMANA (si da 104 = la SP ignora SEMANA) ===';
EXEC dbo.sp_dashboard_tendencia @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='SEMANA';

PRINT '=== E2c. sp_dashboard_tendencia ANIO ===';
EXEC dbo.sp_dashboard_tendencia @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='ANIO';

PRINT '=== E2d. sp_dashboard_tendencia con datos (DIA) ===';
EXEC dbo.sp_dashboard_tendencia @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='DIA';

PRINT '=== E3. sp_dashboard_top_productos con datos (MONTO) ===';
EXEC dbo.sp_dashboard_top_productos @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @metrica='MONTO', @top=10;

PRINT '=== E3b. sp_dashboard_top_productos metrica INVALIDA (debe degradar a MONTO) ===';
EXEC dbo.sp_dashboard_top_productos @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @metrica='ZZZ', @top=10;

PRINT '=== E4. sp_dashboard_categoria_distribucion (SIN @idcategoria por diseño) ===';
EXEC dbo.sp_dashboard_categoria_distribucion @fechainicio='2026-07-01', @fechafin='2026-10-12', @idusuario=NULL;

PRINT '=== E4b. sp_dashboard_categoria_distribucion rango SIN datos (0 filas) ===';
EXEC dbo.sp_dashboard_categoria_distribucion @fechainicio='2019-01-01', @fechafin='2019-01-31', @idusuario=NULL;

PRINT '=== E5. sp_estadisticas_ventas_periodo DIA/MONTO ===';
EXEC dbo.sp_estadisticas_ventas_periodo @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='DIA', @metrica='MONTO';

PRINT '=== E5b. sp_estadisticas_ventas_periodo SEMANA/CANTIDAD ===';
EXEC dbo.sp_estadisticas_ventas_periodo @fechainicio='2026-07-01', @fechafin='2026-10-12',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='SEMANA', @metrica='CANTIDAD';

PRINT '=== E5c. sp_estadisticas_ventas_periodo MES/ANIO + rango invertido (inicio>fin) ===';
EXEC dbo.sp_estadisticas_ventas_periodo @fechainicio='2026-10-12', @fechafin='2026-07-01',
     @idusuario=NULL, @idcategoria=NULL, @granularidad='MES', @metrica='MONTO';

PRINT '=== E6. sp_estadisticas_stock_alertas umbral 5 ===';
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=5, @idcategoria=NULL;

PRINT '=== E6b. sp_estadisticas_stock_alertas umbral 0 (solo stock 0) ===';
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=0, @idcategoria=NULL;

PRINT '=== E6c. sp_estadisticas_stock_alertas umbral 9999 (todos, muestra estados) ===';
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=9999, @idcategoria=NULL;

PRINT '=== E6d. sp_estadisticas_stock_alertas con categoría ===';
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=9999, @idcategoria=@idc;

PRINT '=== E7. sp_dashboard_filtros_vendedores_categorias (result set 1: vendedores) ===';
EXEC dbo.sp_dashboard_filtros_vendedores_categorias @idusuario=NULL;

PRINT '=== E7b. result set 2: categorías (2ª llamada para aislarlo) ===';
EXEC dbo.sp_dashboard_filtros_vendedores_categorias @idusuario=NULL;

PRINT '=== E7c. con @idusuario filtrado ===';
EXEC dbo.sp_dashboard_filtros_vendedores_categorias @idusuario=@idu;

PRINT '=== F. SHAPE vía sys.dm_exec_describe_first_result_set ===';
SELECT 'KPIs' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_dashboard_kpis @fechainicio=''2026-07-01'', @fechafin=''2026-10-12'', @idusuario=NULL, @idcategoria=NULL', NULL, 0);

SELECT 'Tendencia' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_dashboard_tendencia @fechainicio=''2026-07-01'', @fechafin=''2026-10-12'', @idusuario=NULL, @idcategoria=NULL, @granularidad=''DIA''', NULL, 0);

SELECT 'TopProductos' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_dashboard_top_productos @fechainicio=''2026-07-01'', @fechafin=''2026-10-12'', @idusuario=NULL, @idcategoria=NULL, @metrica=''MONTO'', @top=10', NULL, 0);

SELECT 'Distribucion' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_dashboard_categoria_distribucion @fechainicio=''2026-07-01'', @fechafin=''2026-10-12'', @idusuario=NULL', NULL, 0);

SELECT 'VentasPeriodo' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_estadisticas_ventas_periodo @fechainicio=''2026-07-01'', @fechafin=''2026-10-12'', @idusuario=NULL, @idcategoria=NULL, @granularidad=''DIA'', @metrica=''MONTO''', NULL, 0);

SELECT 'StockAlertas' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=5, @idcategoria=NULL', NULL, 0);

SELECT 'FiltrosVendedores' AS SP, name AS Columna, system_type_name AS Tipo, is_nullable AS Anulable
FROM sys.dm_exec_describe_first_result_set(
 N'EXEC dbo.sp_dashboard_filtros_vendedores_categorias @idusuario=NULL', NULL, 0);

PRINT '=== G. VALORES DE ESTADO EN ALERTAS (acento / case) ===';
CREATE TABLE #alertas (
    IdArticulo nvarchar(100), Codigo nvarchar(450), Nombre nvarchar(450),
    Categoria nvarchar(450), StockActual int, StockMinimo int, Estado nvarchar(100));
INSERT INTO #alertas
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=9999, @idcategoria=NULL;
SELECT Estado, COUNT(*) AS Filas FROM #alertas GROUP BY Estado;
DROP TABLE #alertas;

PRINT '=== H. FIN QA ===';
