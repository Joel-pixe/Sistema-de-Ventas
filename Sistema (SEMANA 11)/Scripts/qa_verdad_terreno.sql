-- QA: verdad de terreno para el modulo Estadisticas (ventana 2026-09-13..2026-10-12)
SET NOCOUNT ON;

PRINT '--- 1) ventas en la ventana de prueba (hoy-29..hoy) ---';
SELECT idventa, fecha, total, estado FROM venta
WHERE fecha >= '2026-09-13' AND fecha < '2026-10-13' ORDER BY fecha;

PRINT '--- 2) verdad cruda KPI (estado<>Anulado) ---';
SELECT ISNULL(SUM(total),0) AS VentasTotales, COUNT(*) AS NumTransacciones
FROM venta
WHERE ISNULL(estado,'') <> 'Anulado'
  AND fecha >= '2026-09-13' AND fecha < '2026-10-13';

PRINT '--- 3) sp_dashboard_kpis misma ventana ---';
EXEC sp_dashboard_kpis @FechaInicio='2026-09-13', @FechaFin='2026-10-12';

PRINT '--- 4) detalle de ventas en la ventana ---';
SELECT dv.idventa, a.nombre AS articulo, dv.cantidad, dv.precio, dv.descuento,
       (dv.cantidad * dv.precio - dv.descuento) AS neto
FROM detalle_venta dv
INNER JOIN venta v ON v.idventa = dv.idventa
INNER JOIN articulo a ON a.idarticulo = dv.idarticulo
WHERE v.fecha >= '2026-09-13' AND v.fecha < '2026-10-13'
ORDER BY dv.idventa;

PRINT '--- 5) sp_dashboard_top_productos misma ventana ---';
EXEC sp_dashboard_top_productos @FechaInicio='2026-09-13', @FechaFin='2026-10-12', @Metrica='MONTO', @Top=10;

PRINT '--- 6) sp_dashboard_tendencia con Granularidad=SEMANA (etiquetas que devuelve) ---';
EXEC sp_dashboard_tendencia @FechaInicio='2026-09-13', @FechaFin='2026-10-12', @Granularidad='SEMANA';

PRINT '--- 7) stock_alertas con parametro correcto (funciona en SQL) ---';
EXEC sp_estadisticas_stock_alertas @StockMinimoUmbral=5;

PRINT '--- 8) estados distintos presentes en venta ---';
SELECT estado, COUNT(*) AS n FROM venta GROUP BY estado;
