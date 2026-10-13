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
    public class DIngresos
    {
        public class DIngreso
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
                    SqlCommand Comando = new SqlCommand("ingreso_listar", sqlConnection);
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
                    SqlCommand Comando = new SqlCommand("ingreso_buscar", sqlConnection);
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

            public async Task<DataTable> ListarDetalle(int idIngreso)
            {
                SqlDataReader Resultado;
                DataTable Tabla = new DataTable();
                SqlConnection sqlConnection = new SqlConnection();

                try
                {
                    sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                    SqlCommand Comando = new SqlCommand("ingreso_listar_detalle", sqlConnection);
                    Comando.CommandType = CommandType.StoredProcedure;
                    Comando.Parameters.Add("@idingreso", SqlDbType.Int).Value = idIngreso;
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

            public async Task<string> Insertar(Ingreso ingreso, DataTable detalle)
            {
                string Respuesta = ""; //esta es la variable que vamos a retornar 
                SqlConnection sqlConnection = new SqlConnection();
                try
                {
                    sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                    SqlCommand Comando = new SqlCommand("ingreso_insertar", sqlConnection); 
                    Comando.CommandType = CommandType.StoredProcedure;
                    Comando.Parameters.AddWithValue("@idusuario", ingreso.IdUsuario);
                    Comando.Parameters.AddWithValue("@idproveedor", ingreso.IdProveedor);
                    Comando.Parameters.AddWithValue("@tipo_comprobante", ingreso.TipoComprobante);
                    Comando.Parameters.AddWithValue("@serie_comprobante", ingreso.SerieComprobante);
                    Comando.Parameters.AddWithValue("@num_comprobante", ingreso.NumeroComprobante);
                    Comando.Parameters.AddWithValue("@impuesto", ingreso.Impuesto);
                    Comando.Parameters.AddWithValue("@total", ingreso.Total);
                    SqlParameter parametroDetalle = Comando.Parameters.AddWithValue("@detalle", detalle);
                    parametroDetalle.SqlDbType = SqlDbType.Structured;
                    parametroDetalle.TypeName = "dbo.types_detalle_ingreso";
                    await sqlConnection.OpenAsync();
                    int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                    Respuesta = filasAfectadas > 0 ? "OK" : "No se pudo insertar el Registro";
                }
                catch (Exception ex)
                {

                    Respuesta = ex.Message; // convertimos excepcion a un texto
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

            public async Task<string> Anular(int id)
            {
                string Respuesta = " ";
                SqlConnection sqlConnection = new SqlConnection();

                try
                {
                    sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                    SqlCommand Comando = new SqlCommand("ingreso_anular", sqlConnection);
                    Comando.CommandType = CommandType.StoredProcedure;
                    Comando.Parameters.Add("@idingreso", SqlDbType.Int).Value = id;
                    await sqlConnection.OpenAsync();
                    int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                    Respuesta = filasAfectadas > 0 ? "Ok" : "No se pudo anular el registro";

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
}
