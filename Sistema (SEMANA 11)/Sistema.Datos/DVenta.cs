using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Datos
{
    public class DVenta
    {
        public async Task<DataTable> Listar()
        {
            //como devolvemos un DataTable, vamos a crear un objeto
            DataTable Tabla = new DataTable();
            //necesitamos almacenar la cadena de conexion
            SqlConnection sqlConnection = new SqlConnection();
            //necesito un SqlDataReader que almacene el resultado de la consulta SQL
            SqlDataReader Resultado;

            try
            {
                // crea la cadena de conexion
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                // crear un comando para enviar a SQL
                SqlCommand Comando = new SqlCommand("venta_listar", sqlConnection);
                // debemos decirle que el primer argumento es un procedimiento almacenado
                Comando.CommandType = CommandType.StoredProcedure;
                // abrir la conexion
                await sqlConnection.OpenAsync();
                //ejecutamos el comando
                Resultado = await Comando.ExecuteReaderAsync();
                // no podemos manipular un SQLDataReader, lo convertiremos a un DataTable
                Tabla.Load(Resultado);
                return Tabla;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //siempre debemos cerrar la conexión
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<DataTable> Buscar(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("venta_buscar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                // el procedimiento almacenado recibe un parametro @valor, debemos enviarlo
                Comando.Parameters.Add("@valor", SqlDbType.VarChar).Value = valor;
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
                //siempre debemos cerrar la conexión
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<DataTable> ListarDetalle(int idVenta)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("venta_listar_detalle", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idventa", SqlDbType.Int).Value = idVenta;
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
                //siempre debemos cerrar la conexión
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<DataTable> MostrarComprobante(int idVenta)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("venta_mostrar_comprobante", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idventa", SqlDbType.Int).Value = idVenta;
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
                //siempre debemos cerrar la conexión
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<string> Insertar(Venta venta, DataTable detalle)
        {
            string Respuesta = "";
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();

                // 1. Apuntamos al procedimiento correcto de ventas
                SqlCommand Comando = new SqlCommand("venta_insertar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;

                // 2. Mapeamos los datos de la cabecera de la venta
                Comando.Parameters.AddWithValue("@idusuario", venta.IdUsuario);
                Comando.Parameters.AddWithValue("@idcliente", venta.IdCliente); // Cambiado a Cliente
                Comando.Parameters.AddWithValue("@tipo_comprobante", venta.TipoComprobante);
                Comando.Parameters.AddWithValue("@serie_comprobante", venta.SerieComprobante);
                Comando.Parameters.AddWithValue("@num_comprobante", venta.NumComprobante);
                Comando.Parameters.AddWithValue("@fecha", venta.Fecha);
                Comando.Parameters.AddWithValue("@impuesto", venta.Impuesto);
                Comando.Parameters.AddWithValue("@total", venta.Total);

                // 3. Pasamos el Table-Valued Parameter (TVP) con el detalle de la venta
                // Nota: Asegúrate de que en SQL Server creaste el tipo "types_detalle_venta"
                SqlParameter parametroDetalle = Comando.Parameters.AddWithValue("@detalle", detalle);
                parametroDetalle.SqlDbType = SqlDbType.Structured;
                parametroDetalle.TypeName = "dbo.type_detalle_venta";

                await sqlConnection.OpenAsync();

                // Ejecutamos y validamos si se insertó correctamente
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas > 0 ? "OK" : "No se pudo insertar el Registro de la Venta";
            }
            catch (Exception ex)
            {
                Respuesta = ex.Message;
            }
            finally
            {
                // Bloque de seguridad para cerrar la conexión pase lo que pase
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }

            return Respuesta;
        }

        public async Task<string> Anular(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("venta_anular", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idventa", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas > 0 ? "Ok" : "No se pudo anular la venta";

            }
            catch (Exception ex)
            {

                Respuesta = ex.Message;
            }
            finally
            {
                //siempre debemos cerrar la conexión
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }

            return Respuesta;
        }

    }
}