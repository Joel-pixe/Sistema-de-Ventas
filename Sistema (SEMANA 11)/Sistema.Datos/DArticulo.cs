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
    public class DArticulo
    {
        //Listar los Articulos
        public async Task<DataTable> Listar()
        {
            //Como devolvemos un DataTable, vamos a crear un objeto
            DataTable Tabla = new DataTable();
            //Necesitamos almacenar la cadena de conexion
            SqlConnection sqlConnection = new SqlConnection();
            //Necesito un SqlDataReader que almacene el resultado de la consulta SQL
            SqlDataReader Resultado;

            try
            {
                // Crea la cadena de conexion
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                // Crear un comando para enviar a SQL
                SqlCommand Comando = new SqlCommand("articulo_listar", sqlConnection);
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
        }// fin del metodo Listar

        public async Task<DataTable> Buscar(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_buscar", sqlConnection);
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
            } //fin del metodo Buscar


        }

        public async Task<DataTable> BuscarCodigo(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_buscar_codigo", sqlConnection);
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

        public async Task<string> Insertar(Articulo articulo)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_insertar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idcategoria", articulo.IdCategoria);
                Comando.Parameters.AddWithValue("@codigo", articulo.Codigo);
                Comando.Parameters.AddWithValue("@nombre", articulo.Nombre);
                Comando.Parameters.AddWithValue("@precio_venta", articulo.PrecioVenta);
                Comando.Parameters.AddWithValue("@stock", articulo.Stock);
                Comando.Parameters.AddWithValue("@descripcion", articulo.Descripcion);
                Comando.Parameters.AddWithValue("@imagen", articulo.Imagen);

                await sqlConnection.OpenAsync();

                //en este caso, no vamos a traer registro, por ende ejecutaremos una insercion a la bd
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "OK" : "No se pudo insertar el Registro";
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
        }// fin del metodo insertar

        public async Task<string> Actualizar(Articulo articulo)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_actualizar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idarticulo", articulo.IdArticulo);
                Comando.Parameters.AddWithValue("@idcategoria", articulo.IdCategoria);
                Comando.Parameters.AddWithValue("@codigo", articulo.Codigo);
                Comando.Parameters.AddWithValue("@nombre", articulo.Nombre);
                Comando.Parameters.AddWithValue("@precio_venta", articulo.PrecioVenta);
                Comando.Parameters.AddWithValue("@stock", articulo.Stock);
                Comando.Parameters.AddWithValue("@descripcion", articulo.Descripcion);
                Comando.Parameters.AddWithValue("@imagen", articulo.Imagen);

                await sqlConnection.OpenAsync();
                //en este caso, no vamos a traer registro, por ende ejecutaremos una insercion a la bd
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "OK" : "No se pudo actualizar el Registro";
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
        }// fin del metodo actualizar

        public async Task<string> Eliminar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_eliminar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idarticulo", SqlDbType.Int).Value = id;

                await sqlConnection.OpenAsync();

                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se pudo eliminar el registro";

            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // Error 547 = violación de llave foránea (FK)
                Respuesta = "No se puede eliminar este artículo porque ya tiene ventas o ingresos registrados. Puedes desactivarlo en su lugar.";
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

        public async Task<string> Activar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_activar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idarticulo", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se puedo activar el registro";

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

        public async Task<string> Desactivar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_desactivar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idarticulo", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se puedo desactivar el registro";

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

        public async Task<string> Existe(string valor)
        {
            string Respuesta = "";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("articulo_existe", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@valor", valor);
                //Debemos configurar el parametro de salida @existe del procedimiento almacenado 
                SqlParameter existe = new SqlParameter(); // aqui vamos a obtener el resultado que me devuelve el @existe
                existe.ParameterName = "@existe"; // le decimos su equivalente en el Store Procedure
                existe.SqlDbType = SqlDbType.Int; // le decimos que tipo de datos va a recibir
                existe.Direction = ParameterDirection.Output; // le decimos que es un parametro de salida
                Comando.Parameters.Add(existe); //agregamos el objeto al comando
                await sqlConnection.OpenAsync();
                await Comando.ExecuteNonQueryAsync();
                Respuesta = existe.Value.ToString();


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
