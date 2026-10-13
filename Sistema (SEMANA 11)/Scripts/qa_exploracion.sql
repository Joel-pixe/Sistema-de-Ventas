SET NOCOUNT ON;
PRINT '=== TABLAS ===';
SELECT name AS Tabla FROM sys.tables ORDER BY name;

PRINT '=== VOLUMEN / RANGO FECHAS ===';
SELECT 'venta' AS T, COUNT(*) AS Filas, MIN(fecha) AS MinFecha, MAX(fecha) AS MaxFecha FROM dbo.venta
UNION ALL SELECT 'ingreso', COUNT(*), MIN(fecha), MAX(fecha) FROM dbo.ingreso
UNION ALL SELECT 'detalle_venta', COUNT(*), NULL, NULL FROM dbo.detalle_venta
UNION ALL SELECT 'detalle_ingreso', COUNT(*), NULL, NULL FROM dbo.detalle_ingreso
UNION ALL SELECT 'articulo', COUNT(*), NULL, NULL FROM dbo.articulo
UNION ALL SELECT 'categoria', COUNT(*), NULL, NULL FROM dbo.categoria
UNION ALL SELECT 'usuario', COUNT(*), NULL, NULL FROM dbo.usuario
UNION ALL SELECT 'persona', COUNT(*), NULL, NULL FROM dbo.persona;

PRINT '=== ESTADOS EN VENTA ===';
SELECT ISNULL(estado,'(NULL)') AS Estado, COUNT(*) AS n FROM dbo.venta GROUP BY estado;

PRINT '=== ROLES ===';
SELECT r.idrol, r.nombre, COUNT(u.idusuario) AS usuarios FROM dbo.rol r LEFT JOIN dbo.usuario u ON u.idrol = r.idrol GROUP BY r.idrol, r.nombre;
