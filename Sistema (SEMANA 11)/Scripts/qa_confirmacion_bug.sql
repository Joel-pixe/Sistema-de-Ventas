SET NOCOUNT OFF;
PRINT '=== 1. NOMBRES EXACTOS DE PARÁMETROS DEL SP DE STOCK ===';
SELECT name AS Param, LEN(name) AS Largo FROM sys.parameters
WHERE object_id = OBJECT_ID('dbo.sp_estadisticas_stock_alertas') ORDER BY parameter_id;

PRINT '=== 2. LLAMADA CON EL NOMBRE QUE USA DEstadisticas.cs (@stock_minimo_umbral) ===';
EXEC dbo.sp_estadisticas_stock_alertas @stock_minimo_umbral=5, @idcategoria=NULL;

PRINT '=== 3. LLAMADA CON EL NOMBRE REAL (@StockMinimoUmbral) ===';
EXEC dbo.sp_estadisticas_stock_alertas @StockMinimoUmbral=5, @IdCategoria=NULL;

PRINT '=== 4. ESTADOS DEVUELTOS (compara con UcAlertasStock: CRITICO/Critico/BAJO/Bajo) ===';
CREATE TABLE #a (IdArticulo int, Codigo varchar(50), Nombre varchar(100), Categoria varchar(50),
                 StockActual int, StockMinimo int, Estado varchar(7));
INSERT INTO #a EXEC dbo.sp_estadisticas_stock_alertas @StockMinimoUmbral=9999, @IdCategoria=NULL;
SELECT Estado, COUNT(*) AS Filas FROM #a GROUP BY Estado;
DROP TABLE #a;

PRINT '=== 5. ¿LOS SPs TIENEN DEFAULTS? (ejecutar SIN argumentos) ===';
EXEC dbo.sp_dashboard_filtros_vendedores_categorias;

PRINT '=== 6. ¿@FechaInicio mayúsculas/minúsculas importa? (ya probado: no) ===';
SELECT CASE WHEN LOWER(name) = '@fechainicio' THEN name ELSE name END AS NombreReal
FROM sys.parameters WHERE object_id = OBJECT_ID('dbo.sp_dashboard_kpis') ORDER BY parameter_id;

PRINT '=== 7. ÍNDICES QUE DEBERÍAN ESTAR EN VENTA (citados como preexistentes) ===';
SELECT name FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.venta') ORDER BY name;

PRINT '=== 8. FIN ===';
