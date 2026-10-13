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
    public class DCategoria
    {
        //Listar las categorias 
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
                SqlCommand Comando = new SqlCommand("categoria_listar", sqlConnection);
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

        //buscar o filtrar categorias
        public async Task<DataTable> Buscar(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_buscar", sqlConnection);
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

        public async Task<string> Insertar(Categoria categoria)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_insertar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@nombre", categoria.Nombre);
                Comando.Parameters.AddWithValue("@descripcion", categoria.Descripcion);
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

        //actualizar una categoria
        public async Task<string> Actualizar(Categoria categoria)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_actualizar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idcategoria", categoria.IdCategoria);
                Comando.Parameters.AddWithValue("@nombre", categoria.Nombre);
                Comando.Parameters.AddWithValue("@descripcion", categoria.Descripcion);
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

        //Eliminar una categoria

        public async Task<string> Eliminar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_eliminar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idcategoria", SqlDbType.Int).Value = id;

                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se pudo eliminar el registro";

            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // 547 = violación de llave foránea (FK) en SQL Server
                Respuesta = "No se puede eliminar la categoría porque tiene artículos asociados";
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

        //Activar una categoria
        public async Task<string> Activar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_activar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idcategoria", SqlDbType.Int).Value = id;

                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se pudo activar el registro";

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

        //Desactivar una categoria
        public async Task<string> Desactivar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_desactivar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idcategoria", SqlDbType.Int).Value = id;

                await sqlConnection.OpenAsync();

                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "Ok" : "No se pudo desactivar el registro";

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
        //Determinar si una categoria existe
        public async Task<string> Existe(string valor)
        {
            string Respuesta = "";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("categoria_existe", sqlConnection);
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
