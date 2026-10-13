using Sistema.Datos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema.Entidad;

namespace Sistema.Negocios
{
    public class NUsuario
    {
        public static async Task<DataTable> Listar()
        {
            //Debemos instanciar la capa de acceso a datos
            DUsuario datos = new DUsuario();
            return await datos.Listar();
        }

        public static async Task<DataTable> Buscar(string valor)
        {
            DUsuario datos = new DUsuario();
            return await datos.Buscar(valor);
        }

        public static async Task<string> Insertar(int idRol, string nombre, string tipoDocumento, string numDocumento, string direccion, string telefono, string email, string clave)
        {
            DUsuario datos = new DUsuario();

            //Primero debemos determinar si ya hay un Articulo con ese nombre
            string existe = await datos.Existe(email);

            if (existe.Equals("1"))
            {
                return "Ya existe un usuario con ese email";
            }
            else
            {
                var Usuario = new Usuario()
                {
                    IdRol = idRol,
                    Nombre = nombre,
                    TipoDocumento = tipoDocumento,
                    NumDocumento = numDocumento,
                    Direccion = direccion,
                    Telefono = telefono,
                    Email = email,
                    Clave = clave  // el hash lo aplica directamente el SP con HASHBYTES
                };


                return await datos.Insertar(Usuario);
            }

        }

        public static async Task<string> Actualizar(int idUsuario, int idRol, string emailAnterior, string nombre, string tipoDocumento, string numDocumento, string direccion, string telefono, string email, string clave)
        {
            DUsuario datos = new DUsuario();
            Usuario usuario = new Usuario();

            if (emailAnterior.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                // El email no cambió, no hay que verificar duplicado
                usuario.IdUsuario = idUsuario;
                usuario.IdRol = idRol;
                usuario.Nombre = nombre;         // ← estaba como "email"
                usuario.TipoDocumento = tipoDocumento;
                usuario.NumDocumento = numDocumento;
                usuario.Direccion = direccion;
                usuario.Telefono = telefono;
                usuario.Email = email;
                usuario.Clave = clave;           // ← faltaba

                return await datos.Actualizar(usuario);
            }
            else
            {
                // El email cambió, verificar que no exista otro usuario con ese email
                string existe = await datos.Existe(email);  // ← estaba verificando por "nombre"
                if (existe == "1")
                {
                    return "Ya existe un usuario con ese email";
                }
                else
                {
                    usuario.IdUsuario = idUsuario;
                    usuario.IdRol = idRol;
                    usuario.Nombre = nombre;     // ← estaba como "email"
                    usuario.TipoDocumento = tipoDocumento;
                    usuario.NumDocumento = numDocumento;
                    usuario.Direccion = direccion;
                    usuario.Telefono = telefono;
                    usuario.Email = email;
                    usuario.Clave = clave;       // ← faltaba

                    return await datos.Actualizar(usuario);
                }
            }
        }

        public static async Task<string> Eliminar(int id)
        {
            DUsuario datos = new DUsuario();
            return await datos.Eliminar(id);
        }

        public static async Task<string> Activar(int id)
        {
            DUsuario datos = new DUsuario();
            return await datos.Activar(id);
        }

        public static async Task<string> Desactivar(int id)
        {
            DUsuario datos = new DUsuario();
            return await datos.Desactivar(id);
        }

        public static async Task<DataTable> Login(string email, string clave)
        {
            DUsuario datos = new DUsuario();
            return await datos.Login(email, clave);
        }

        public static async Task<DataTable> BuscarPorEmail(string email)
        {
            DUsuario datos = new DUsuario();
            return await datos.BuscarPorEmail(email);
        }

        public static async Task<string> CambiarClave(int idUsuario, string nuevaClave)
        {
            DUsuario datos = new DUsuario();
            return await datos.CambiarClave(idUsuario, nuevaClave);
        }

    }
}
