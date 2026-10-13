CREATE PROCEDURE venta_mostrar_comprobante
    @idventa INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.idventa AS IdVenta,
        v.tipo_comprobante AS TipoComprobante,
        v.serie_comprobante AS SerieComprobante,
        v.num_comprobante AS NumComprobante,
        v.fecha AS Fecha,
        v.impuesto AS Impuesto,
        v.total AS Total,
        v.estado AS Estado,
        p.nombre AS Cliente,
        p.tipo_documento AS TipoDocumento,
        p.num_documento AS NumDocumento,
        u.nombre AS Vendedor,
        a.codigo AS Codigo,
        a.nombre AS Articulo,
        d.cantidad AS Cantidad,
        d.precio AS Precio,
        d.descuento AS Descuento,
        ((d.cantidad * d.precio) - d.descuento) AS Importe
    FROM venta v
    INNER JOIN persona p ON v.idcliente = p.idpersona
    INNER JOIN usuario u ON v.idusuario = u.idusuario
    INNER JOIN detalle_venta d ON v.idventa = d.idventa
    INNER JOIN articulo a ON d.idarticulo = a.idarticulo
    WHERE v.idventa = @idventa
    ORDER BY d.iddetalle_venta;
END
GO
