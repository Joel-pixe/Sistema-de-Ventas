using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Datos
{
    public class Conexion
    {
        private string Base; // nombre de la base de datos
        private string Servidor; // ruta del servidor
        private string Usuario; // username
        private string Clave; // contraseña
        private bool Seguridad; // controlar si nos conectamos con standard security o trusted connection
        private static Conexion Conn = null;

        private Conexion()
        {
            this.Base = "mydatabase2";
            this.Servidor = "localhost\\SQLEXPRESS";
            this.Usuario = "sa";
            this.Clave = "";
            this.Seguridad = true;
        }

        // metodo que cree la cadena de conexion 
        public SqlConnection CrearConexion()
        {
            // crear una variable(objeto) que sea de tipo SqlConnection
            SqlConnection Cadena = new SqlConnection();

            try
            {
                //tratar de crear nuestra cadena
                // TrustServerCertificate: el certificado autofirmado de SQLEXPRESS local
                // no valida con Encrypt=obligatorio (Microsoft.Data.SqlClient >= 4.0)
                Cadena.ConnectionString = $"Server={Servidor}; Database={Base}; TrustServerCertificate=True;";
                if (Seguridad)
                {
                    //nos conectamos usando Trusted Connection
                    Cadena.ConnectionString += "Integrated Security = SSPI";

                }
                else
                {
                    // nos conectamos usando usuario y clave
                    Cadena.ConnectionString += $"User Id={Usuario}; Password = {Clave};";
                }

                return Cadena;

            }
            catch (Exception ex)
            {
                Cadena = null;
                throw ex;
            }

        }

        // metodo para instanciar desde adentro la clase y el constructor
        public static Conexion ObtenerInstancia()
        {
            if (Conn == null) // Singleton
            {
                Conn = new Conexion();
            }
            return Conn;
        }

    }
}
