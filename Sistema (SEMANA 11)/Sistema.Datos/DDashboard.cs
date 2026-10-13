using Sistema.Datos.Interfaces;
using Sistema.Entidad;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Sistema.Datos
{
    public class DDashboard : IDashboardRepository
    {
        // Devuelve 1 fila con los 6 KPIs del periodo
        public async Task<DataTable> ObtenerKpis(FilterDto filtro)
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("sp_dashboard_kpis", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                AgregarParametrosComunes(Comando, filtro);
                await sqlConnection.OpenAsync();
                Resultado = await Comando.ExecuteReaderAsync();
                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        // Serie temporal continua de ventas/ingresos (por dia o por mes)
        public async Task<DataTable> ObtenerTendencia(FilterDto filtro)
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("sp_dashboard_tendencia", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                AgregarParametrosComunes(Comando, filtro);
                Comando.Parameters.Add("@granularidad", SqlDbType.VarChar, 10).Value =
                    string.IsNullOrEmpty(filtro.Granularidad) ? "DIA" : filtro.Granularidad;
                await sqlConnection.OpenAsync();
                Resultado = await Comando.ExecuteReaderAsync();
                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        // Top articulos mas vendidos por cantidad o por monto
        public async Task<DataTable> ObtenerTopProductos(FilterDto filtro)
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("sp_dashboard_top_productos", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                AgregarParametrosComunes(Comando, filtro);
                Comando.Parameters.Add("@metrica", SqlDbType.VarChar, 10).Value =
                    string.IsNullOrEmpty(filtro.Metrica) ? "MONTO" : filtro.Metrica;
                await sqlConnection.OpenAsync();
                Resultado = await Comando.ExecuteReaderAsync();
                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        // Distribucion de ingresos por categoria con su porcentaje
        public async Task<DataTable> ObtenerDistribucionCategorias(FilterDto filtro)
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("sp_dashboard_categoria_distribucion", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                // Este sp no recibe filtro de categoria: la distribucion
                // siempre compara todas las categorias entre si.
                AgregarParametrosComunes(Comando, filtro, false);
                await sqlConnection.OpenAsync();
                Resultado = await Comando.ExecuteReaderAsync();
                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        // Vendedores del filtro (segun el rol: NULL = todos)
        public async Task<DataTable> ObtenerVendedores(int? idUsuario)
        {
            return await EjecutarFiltros(idUsuario, true);
        }

        // Categorias activas del filtro
        public async Task<DataTable> ObtenerCategorias()
        {
            return await EjecutarFiltros(null, false);
        }

        // El sp de filtros devuelve 2 result sets (vendedores y categorias).
        // Cada metodo hace su propia consulta y se posiciona en el result set
        // que le corresponde antes de cargar la tabla.
        private async Task<DataTable> EjecutarFiltros(int? idUsuario, bool primeraTabla)
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("sp_dashboard_filtros_vendedores_categorias", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                SqlParameter Parametro = Comando.Parameters.Add("@idusuario", SqlDbType.Int);
                Parametro.Value = idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value;
                await sqlConnection.OpenAsync();
                SqlDataReader Resultado = await Comando.ExecuteReaderAsync();

                if (!primeraTabla)
                {
                    await Resultado.NextResultAsync();
                }

                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        private static void AgregarParametrosComunes(SqlCommand Comando, FilterDto filtro, bool incluyeCategoria = true)
        {
            Comando.Parameters.Add("@fechainicio", SqlDbType.Date).Value = filtro.FechaInicio.Date;
            Comando.Parameters.Add("@fechafin", SqlDbType.Date).Value = filtro.FechaFin.Date;

            SqlParameter ParametroUsuario = Comando.Parameters.Add("@idusuario", SqlDbType.Int);
            ParametroUsuario.Value = filtro.IdUsuario.HasValue ? (object)filtro.IdUsuario.Value : DBNull.Value;

            if (incluyeCategoria)
            {
                SqlParameter ParametroCategoria = Comando.Parameters.Add("@idcategoria", SqlDbType.Int);
                ParametroCategoria.Value = filtro.IdCategoria.HasValue ? (object)filtro.IdCategoria.Value : DBNull.Value;
            }
        }
    }
}
