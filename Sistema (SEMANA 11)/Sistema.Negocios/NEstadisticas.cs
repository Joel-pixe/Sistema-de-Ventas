using Sistema.Datos;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Sistema.Negocios
{
    // Fachada del módulo Estadísticas. Métodos estáticos, igual que el resto de
    // las clases N* del proyecto: validan/normalizan la entrada, invocan la capa
    // Datos y mapean DataTable -> DTO.
    public static class NEstadisticas
    {
        private static readonly string[] GranularidadesValidas = { "DIA", "SEMANA", "MES", "ANIO" };
        private static readonly string[] MetricasValidas = { "MONTO", "CANTIDAD" };

        public static async Task<KpiCardsDto> ObtenerKpisAsync(FilterDto filtro, CancellationToken ct = default)
        {
            filtro = Validar(filtro);
            DataTable tabla = await new DEstadisticas().ObtenerKpis(filtro, ct).ConfigureAwait(false);

            if (tabla.Rows.Count == 0)
            {
                return new KpiCardsDto(0, 0, 0, 0, 0, 0);
            }

            DataRow fila = tabla.Rows[0];
            return new KpiCardsDto(
                LeerDecimal(fila, "VentasTotales"),
                LeerDecimal(fila, "IngresosTotales"),
                LeerDecimal(fila, "TicketPromedio"),
                LeerEntero(fila, "NumTransacciones"),
                LeerDecimal(fila, "VariacionVsPeriodoAnterior"),
                LeerDecimal(fila, "MargenBruto"));
        }

        public static async Task<TrendDto> ObtenerTendenciaAsync(FilterDto filtro, CancellationToken ct = default)
        {
            filtro = Validar(filtro);
            DataTable tabla = await new DEstadisticas().ObtenerTendencia(filtro, ct).ConfigureAwait(false);

            List<TrendPointDto> puntos = new List<TrendPointDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                puntos.Add(new TrendPointDto(
                    LeerFecha(fila, "Fecha"),
                    LeerTexto(fila, "PeriodoLabel"),
                    LeerDecimal(fila, "Ventas"),
                    LeerDecimal(fila, "Ingresos")));
            }

            return new TrendDto(puntos);
        }

        public static async Task<TopProductsDto> ObtenerTopProductosAsync(FilterDto filtro, CancellationToken ct = default)
        {
            filtro = Validar(filtro);
            DataTable tabla = await new DEstadisticas().ObtenerTopProductos(filtro, ct).ConfigureAwait(false);

            List<TopProductDto> productos = new List<TopProductDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                productos.Add(new TopProductDto(
                    LeerEntero(fila, "IdArticulo"),
                    LeerTexto(fila, "Codigo"),
                    LeerTexto(fila, "Nombre"),
                    LeerTexto(fila, "Categoria"),
                    LeerEntero(fila, "TotalCantidad"),
                    LeerDecimal(fila, "TotalMonto")));
            }

            return new TopProductsDto(productos);
        }

        public static async Task<CategoryDistributionDto> ObtenerDistribucionCategoriasAsync(FilterDto filtro, CancellationToken ct = default)
        {
            filtro = Validar(filtro);
            DataTable tabla = await new DEstadisticas().ObtenerDistribucionCategorias(filtro, ct).ConfigureAwait(false);

            List<CategoryDto> categorias = new List<CategoryDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                categorias.Add(new CategoryDto(
                    LeerEntero(fila, "IdCategoria"),
                    LeerTexto(fila, "Nombre"),
                    LeerDecimal(fila, "TotalIngresos"),
                    LeerEntero(fila, "TotalCantidad"),
                    LeerDecimal(fila, "Porcentaje")));
            }

            return new CategoryDistributionDto(categorias);
        }

        public static async Task<VentasPeriodoDto> ObtenerVentasPeriodoAsync(FilterDto filtro, CancellationToken ct = default)
        {
            filtro = Validar(filtro);
            DataTable tabla = await new DEstadisticas().ObtenerVentasPeriodo(filtro, ct).ConfigureAwait(false);

            bool porCantidad = filtro.Metrica == "CANTIDAD";
            List<VentasPeriodoPointDto> puntos = new List<VentasPeriodoPointDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                decimal ventas = LeerDecimal(fila, "Ventas");
                int cantidad = LeerEntero(fila, "Cantidad");
                decimal valor = LeerDecimal(fila, "Valor");

                // Si el SP no devolvió Valor calculado se deriva de la métrica.
                if (valor == 0 && (porCantidad ? cantidad != 0 : ventas != 0))
                {
                    valor = porCantidad ? cantidad : ventas;
                }

                puntos.Add(new VentasPeriodoPointDto(
                    LeerFecha(fila, "Fecha"),
                    LeerTexto(fila, "PeriodoLabel"),
                    ventas,
                    cantidad,
                    valor));
            }

            return new VentasPeriodoDto(filtro.Granularidad, filtro.Metrica, puntos);
        }

        public static async Task<StockBajoDto> ObtenerStockBajoAsync(StockFilterDto filtro, CancellationToken ct = default)
        {
            if (filtro == null)
            {
                throw new ArgumentNullException(nameof(filtro));
            }

            // Un umbral negativo no tiene sentido: se fija el valor por defecto (5)
            // y una categoría 0 se interpreta como "sin filtro".
            StockFilterDto normalizado = new StockFilterDto(
                filtro.Umbral < 0 ? 5 : filtro.Umbral,
                filtro.IdCategoria.HasValue && filtro.IdCategoria.Value > 0 ? filtro.IdCategoria : null);

            DataTable tabla = await new DEstadisticas().ObtenerStockBajo(normalizado, ct).ConfigureAwait(false);

            List<StockBajoItemDto> articulos = new List<StockBajoItemDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                articulos.Add(new StockBajoItemDto(
                    LeerEntero(fila, "IdArticulo"),
                    LeerTexto(fila, "Codigo"),
                    LeerTexto(fila, "Nombre"),
                    LeerTexto(fila, "Categoria"),
                    LeerEntero(fila, "StockActual"),
                    LeerEntero(fila, "StockMinimo"),
                    LeerTexto(fila, "Estado")));
            }

            return new StockBajoDto(articulos);
        }

        public static async Task<List<VendedorDto>> ObtenerVendedoresAsync(int? idUsuario, CancellationToken ct = default)
        {
            DataTable tabla = await new DEstadisticas().ObtenerVendedores(idUsuario, ct).ConfigureAwait(false);

            List<VendedorDto> vendedores = new List<VendedorDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                vendedores.Add(new VendedorDto(
                    LeerEntero(fila, "Id"),
                    LeerTexto(fila, "NombreCompleto")));
            }

            return vendedores;
        }

        public static async Task<List<CategoriaDto>> ObtenerCategoriasAsync(CancellationToken ct = default)
        {
            DataTable tabla = await new DEstadisticas().ObtenerCategorias(ct).ConfigureAwait(false);

            List<CategoriaDto> categorias = new List<CategoriaDto>();
            foreach (DataRow fila in tabla.Rows)
            {
                categorias.Add(new CategoriaDto(
                    LeerEntero(fila, "Id"),
                    LeerTexto(fila, "Nombre")));
            }

            return categorias;
        }

        // Carga las dos listas del panel de filtros en paralelo.
        public static async Task<FiltrosDisponiblesDto> ObtenerFiltrosAsync(int? idUsuario, CancellationToken ct = default)
        {
            Task<List<VendedorDto>> vendedores = ObtenerVendedoresAsync(idUsuario, ct);
            Task<List<CategoriaDto>> categorias = ObtenerCategoriasAsync(ct);
            await Task.WhenAll(vendedores, categorias).ConfigureAwait(false);

            return new FiltrosDisponiblesDto(vendedores.Result, categorias.Result);
        }

        // Normaliza la entrada: lanza si el filtro es nulo, invierte el rango de
        // fechas si viene al revés y fuerza granularidad/métrica a valores válidos.
        private static FilterDto Validar(FilterDto filtro)
        {
            if (filtro == null)
            {
                throw new ArgumentNullException(nameof(filtro));
            }

            DateTime inicio = filtro.FechaInicio.Date;
            DateTime fin = filtro.FechaFin.Date;
            if (fin < inicio)
            {
                DateTime temporal = inicio;
                inicio = fin;
                fin = temporal;
            }

            return new FilterDto(
                inicio,
                fin,
                filtro.IdUsuario,
                filtro.IdCategoria,
                Normalizar(filtro.Granularidad, GranularidadesValidas, "DIA"),
                Normalizar(filtro.Metrica, MetricasValidas, "MONTO"));
        }

        private static string Normalizar(string valor, string[] permitidos, string porDefecto)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return porDefecto;
            }

            string limpio = valor.Trim().ToUpperInvariant();
            return Array.IndexOf(permitidos, limpio) >= 0 ? limpio : porDefecto;
        }

        private static string LeerTexto(DataRow fila, string columna)
        {
            return fila.Table.Columns.Contains(columna) && !Convert.IsDBNull(fila[columna])
                ? Convert.ToString(fila[columna])
                : string.Empty;
        }

        private static decimal LeerDecimal(DataRow fila, string columna)
        {
            return fila.Table.Columns.Contains(columna) && !Convert.IsDBNull(fila[columna])
                ? Convert.ToDecimal(fila[columna])
                : 0m;
        }

        private static int LeerEntero(DataRow fila, string columna)
        {
            return fila.Table.Columns.Contains(columna) && !Convert.IsDBNull(fila[columna])
                ? Convert.ToInt32(fila[columna])
                : 0;
        }

        private static DateTime LeerFecha(DataRow fila, string columna)
        {
            return fila.Table.Columns.Contains(columna) && !Convert.IsDBNull(fila[columna])
                ? Convert.ToDateTime(fila[columna])
                : DateTime.MinValue;
        }
    }
}
