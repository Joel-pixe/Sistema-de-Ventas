using Sistema.Datos.Interfaces;
using Sistema.Entidad;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Sistema.Datos
{
    public class DEstadisticas : IEstadisticasRepository
    {
        // KPIs del período (pestaña "KPIs")
        public Task<DataTable> ObtenerKpis(FilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_kpis", cmd => AgregarParametrosComunes(cmd, filtro), ct);
        }

        // Serie dual ingresos vs ventas (pestaña "Tendencia")
        public Task<DataTable> ObtenerTendencia(FilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_tendencia", cmd =>
            {
                AgregarParametrosComunes(cmd, filtro);
                AgregarGranularidad(cmd, filtro.Granularidad);
            }, ct);
        }

        // Ranking de productos (pestaña "Top Productos")
        public Task<DataTable> ObtenerTopProductos(FilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_top_productos", cmd =>
            {
                AgregarParametrosComunes(cmd, filtro);
                AgregarMetrica(cmd, filtro.Metrica);
                cmd.Parameters.Add("@top", SqlDbType.Int).Value = 10;
            }, ct);
        }

        // Reparto de ventas por categoría (pestaña "Distribución").
        // El SP no acepta @idcategoria: la distribución siempre compara todas
        // las categorías entre sí para que los porcentajes sumen 100%.
        public Task<DataTable> ObtenerDistribucionCategorias(FilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_categoria_distribucion",
                cmd => AgregarParametrosComunes(cmd, filtro, incluyeCategoria: false), ct);
        }

        // Serie temporal de ventas por período (pestaña "Ventas Período") — SP nuevo
        public Task<DataTable> ObtenerVentasPeriodo(FilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_estadisticas_ventas_periodo", cmd =>
            {
                AgregarParametrosComunes(cmd, filtro);
                AgregarGranularidad(cmd, filtro.Granularidad);
                AgregarMetrica(cmd, filtro.Metrica);
            }, ct);
        }

        // Artículos con stock bajo o crítico (pestaña "Stock Bajo") — SP nuevo
        public Task<DataTable> ObtenerStockBajo(StockFilterDto filtro, CancellationToken ct = default)
        {
            return EjecutarSP("sp_estadisticas_stock_alertas", cmd =>
            {
                cmd.Parameters.Add("@StockMinimoUmbral", SqlDbType.Int).Value = filtro.Umbral;

                SqlParameter pCategoria = cmd.Parameters.Add("@idcategoria", SqlDbType.Int);
                pCategoria.Value = filtro.IdCategoria.HasValue ? (object)filtro.IdCategoria.Value : DBNull.Value;
            }, ct);
        }

        // Opción "Todos" del filtro de vendedores
        public Task<DataTable> ObtenerVendedores(int? idUsuario, CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_filtros_vendedores_categorias", cmd =>
            {
                SqlParameter pUsuario = cmd.Parameters.Add("@idusuario", SqlDbType.Int);
                pUsuario.Value = idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value;
            }, ct, primerResultSet: true);
        }

        // Opción "Todas" del filtro de categorías (2º result set del mismo SP)
        public Task<DataTable> ObtenerCategorias(CancellationToken ct = default)
        {
            return EjecutarSP("sp_dashboard_filtros_vendedores_categorias",
                cmd => { }, ct, primerResultSet: false);
        }

        // Parámetros compartidos por todos los SP con rango de fechas.
        // @incluyeCategoria = false para los SP que no lo aceptan.
        private static void AgregarParametrosComunes(SqlCommand cmd, FilterDto filtro, bool incluyeCategoria = true)
        {
            cmd.Parameters.Add("@fechainicio", SqlDbType.Date).Value = filtro.FechaInicio.Date;
            cmd.Parameters.Add("@fechafin", SqlDbType.Date).Value = filtro.FechaFin.Date;

            SqlParameter pUsuario = cmd.Parameters.Add("@idusuario", SqlDbType.Int);
            pUsuario.Value = filtro.IdUsuario.HasValue ? (object)filtro.IdUsuario.Value : DBNull.Value;

            if (incluyeCategoria)
            {
                SqlParameter pCategoria = cmd.Parameters.Add("@idcategoria", SqlDbType.Int);
                pCategoria.Value = filtro.IdCategoria.HasValue ? (object)filtro.IdCategoria.Value : DBNull.Value;
            }
        }

        private static void AgregarGranularidad(SqlCommand cmd, string granularidad)
        {
            cmd.Parameters.Add("@granularidad", SqlDbType.VarChar, 10).Value =
                string.IsNullOrWhiteSpace(granularidad) ? "DIA" : granularidad.Trim().ToUpperInvariant();
        }

        private static void AgregarMetrica(SqlCommand cmd, string metrica)
        {
            cmd.Parameters.Add("@metrica", SqlDbType.VarChar, 10).Value =
                string.IsNullOrWhiteSpace(metrica) ? "MONTO" : metrica.Trim().ToUpperInvariant();
        }

        private static async Task<DataTable> EjecutarSP(
            string nombreSP,
            Action<SqlCommand> configurarParametros,
            CancellationToken ct,
            bool primerResultSet = true)
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conexion = Conexion.ObtenerInstancia().CrearConexion())
            using (SqlCommand comando = new SqlCommand(nombreSP, conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                configurarParametros(comando);

                await conexion.OpenAsync(ct).ConfigureAwait(false);

                using (SqlDataReader lector = await comando.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (!primerResultSet)
                    {
                        await lector.NextResultAsync(ct).ConfigureAwait(false);
                    }

                    tabla.Load(lector);
                }
            }

            return tabla;
        }
    }
}
