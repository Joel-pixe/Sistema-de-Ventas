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
    public class DPersona
    {
        public async Task<string> ValidarDuplicados(int idPersona, string numDocumento, string telefono, string direccion, string email)
        {
            string campoDuplicado = null;
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_validar_duplicados", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idpersona", SqlDbType.Int).Value = idPersona;
                Comando.Parameters.Add("@numDocumento", SqlDbType.VarChar, 20).Value = numDocumento;
                Comando.Parameters.Add("@telefono", SqlDbType.VarChar, 20).Value = telefono;
                Comando.Parameters.Add("@direccion", SqlDbType.VarChar, 150).Value = direccion;
                Comando.Parameters.Add("@email", SqlDbType.VarChar, 100).Value = email;

                await sqlConnection.OpenAsync();
                SqlDataReader reader = await Comando.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    campoDuplicado = reader["campo_duplicado"] == DBNull.Value ? null : reader["campo_duplicado"].ToString();
                }
                return campoDuplicado;
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

        //Listar todas las personas
        public async Task<DataTable> Listar()
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_listar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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

        //Listar solo Proveedores
        public async Task<DataTable> ListarProveedores()
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_listar_proveedores", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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

        //Listar solo Clientes
        public async Task<DataTable> ListarClientes()
        {
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            SqlDataReader Resultado;

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_listar_clientes", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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

        //Buscar Personas
        public async Task<DataTable> Buscar(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_buscar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        //Buscar solo entre Proveedores
        public async Task<DataTable> BuscarProveedores(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_buscar_proveedores", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        //Buscar solo entre Clientes
        public async Task<DataTable> BuscarClientes(string valor)
        {
            SqlDataReader Resultado;
            DataTable Tabla = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_buscar_clientes", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public async Task<string> Insertar(Persona persona)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_insertar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@tipo_persona", persona.TipoPersona);
                Comando.Parameters.AddWithValue("@nombre", persona.Nombre);
                Comando.Parameters.AddWithValue("@tipo_documento", persona.TipoDocumento);
                Comando.Parameters.AddWithValue("@num_documento", persona.NumDocumento);
                Comando.Parameters.AddWithValue("@direccion", persona.Direccion);
                Comando.Parameters.AddWithValue("@telefono", persona.Telefono);
                Comando.Parameters.AddWithValue("@email", persona.Email);
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                //en este caso, no vamos a traer registro, por ende ejecutaremos una insercion a la bd
                Respuesta = filasAfectadas == 1 ? "OK" : "No se pudo insertar el Registro";
            }
            catch (Exception ex)
            {
                Respuesta = ex.Message; // convertimos excepcion a un texto
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

        public async Task<string> Actualizar(Persona persona)
        {
            string Respuesta = ""; //esta es la variable que vamos a retornar 
            SqlConnection sqlConnection = new SqlConnection();
            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_actualizar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idpersona", persona.IdPersona);
                Comando.Parameters.AddWithValue("@tipo_persona", persona.TipoPersona);
                Comando.Parameters.AddWithValue("@nombre", persona.Nombre);
                Comando.Parameters.AddWithValue("@tipo_documento", persona.TipoDocumento);
                Comando.Parameters.AddWithValue("@num_documento", persona.NumDocumento);
                Comando.Parameters.AddWithValue("@direccion", persona.Direccion);
                Comando.Parameters.AddWithValue("@telefono", persona.Telefono);
                Comando.Parameters.AddWithValue("@email", persona.Email);
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "OK" : "No se pudo actualizar el Registro";
            }
            catch (Exception ex)
            {
                Respuesta = ex.Message; // convertimos excepcion a un texto
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

        public async Task<string> Eliminar(int id)
        {
            string Respuesta = " ";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_eliminar", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.Add("@idpersona", SqlDbType.Int).Value = id;
                await sqlConnection.OpenAsync();
                int filasAfectadas = await Comando.ExecuteNonQueryAsync();
                Respuesta = filasAfectadas == 1 ? "OK" : "No se pudo eliminar el registro";
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

        public async Task<string> Existe(string valor)
        {
            string Respuesta = "";
            SqlConnection sqlConnection = new SqlConnection();

            try
            {
                sqlConnection = Conexion.ObtenerInstancia().CrearConexion();
                SqlCommand Comando = new SqlCommand("persona_existe", sqlConnection);
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@valor", valor);
                //Debemos configurar el parametro de salida @existe del procedimiento almacenado 
                SqlParameter existe = new SqlParameter(); // aqui vamos a obtener el resultado que me devuelve el @existe
                existe.ParameterName = "@existe"; // le decimos su equivalente en el Store Procedure
                existe.SqlDbType = SqlDbType.Bit; // el procedimiento declara @existe como bit output
                existe.Direction = ParameterDirection.Output; // le decimos que es un parametro de salida
                Comando.Parameters.Add(existe); //agregamos el objeto al comando
                await sqlConnection.OpenAsync();
                await Comando.ExecuteNonQueryAsync();
                Respuesta = Convert.ToBoolean(existe.Value) ? "1" : "0";
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
