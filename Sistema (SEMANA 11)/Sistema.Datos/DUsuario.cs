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
    public class DUsuario
    {
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
                SqlCommand Comando = new SqlCommand("usuario_listar", sqlConnection);
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
                SqlCommand Comando = new SqlCommand("usuario_buscar", sqlConnection);
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

        public async Task<string> Insertar(Usuario usuario)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("usuario_insertar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idrol", usuario.IdRol);
                Comando.Parameters.AddWithValue("@nombre", usuario.Nombre);
                Comando.Parameters.AddWithValue("@tipo_documento", usuario.TipoDocumento);
                Comando.Parameters.AddWithValue("@num_documento", usuario.NumDocumento);
                Comando.Parameters.AddWithValue("@direccion", usuario.Direccion);
                Comando.Parameters.AddWithValue("@telefono", usuario.Telefono);
                Comando.Parameters.AddWithValue("@email", usuario.Email);
                Comando.Parameters.AddWithValue("@clave", usuario.Clave);
                await sqlConnection.OpenAsync();
                //en este caso, no vamos a traer registro, por ende ejecutaremos una insercion a la bd
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "OK" : "No se pudo insertar el Registro";
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

        public async Task<string> Actualizar(Usuario usuario)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("usuario_actualizar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idusuario", usuario.IdUsuario);
                Comando.Parameters.AddWithValue("@idrol", usuario.IdRol);
                Comando.Parameters.AddWithValue("@nombre", usuario.Nombre);
                Comando.Parameters.AddWithValue("@tipo_documento", usuario.TipoDocumento);
                Comando.Parameters.AddWithValue("@num_documento", usuario.NumDocumento);
                Comando.Parameters.AddWithValue("@direccion", usuario.Direccion);
                Comando.Parameters.AddWithValue("@telefono", usuario.Telefono);
                Comando.Parameters.AddWithValue("@email", usuario.Email);
                Comando.Parameters.AddWithValue("@clave", usuario.Clave);
                await sqlConnection.OpenAsync();
                //en este caso, no vamos a traer registro, por ende ejecutaremos una insercion a la bd
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "OK" : "No se pudo actualizar el Registro";
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
                SqlCommand Comando = new SqlCommand("usuario_eliminar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idusuario", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "Ok" : "No se pudo eliminar el registro";

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
                SqlCommand Comando = new SqlCommand("usuario_activar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idusuario", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "Ok" : "No se puedo activar el registro";

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
                SqlCommand Comando = new SqlCommand("usuario_desactivar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idusuario", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "Ok" : "No se puedo desactivar el registro";

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
                SqlCommand Comando = new SqlCommand("usuario_existe", sqlConnection);
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

        public async Task<DataTable> Login(string email, string clave)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("usuario_login", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@email", email);
                Comando.Parameters.AddWithValue("@clave", clave);
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


        public async Task<DataTable> BuscarPorEmail(string email)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("usuario_buscar_por_email", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@email", email);
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

        public async Task<string> CambiarClave(int idUsuario, string nuevaClave)
        {
            string Respuesta = "";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("usuario_cambiar_clave", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idusuario", idUsuario);
                Comando.Parameters.AddWithValue("@clave", nuevaClave);
                await sqlConnection.OpenAsync();
                Respuesta = await Comando.ExecuteNonQueryAsync() == 1 ? "OK" : "No se pudo actualizar la clave";
            }
            catch (Exception ex)
            {
                Respuesta = ex.Message;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }

            return Respuesta;
        }
    }
}
