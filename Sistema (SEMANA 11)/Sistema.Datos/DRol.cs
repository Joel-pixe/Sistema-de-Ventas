using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Datos
{
    public class DRol
    {
        //Listar las categorias 
        public async Task<DataTable> Listar()
        {
            //Como devolvemos un DataTable, vamos a crear un objeto
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
                SqlCommand Comando = new SqlCommand("rol_listar", sqlConnection);
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
    }
}
