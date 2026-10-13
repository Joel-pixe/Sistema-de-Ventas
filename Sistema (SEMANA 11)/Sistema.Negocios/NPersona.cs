using Sistema.Datos;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Negocios
{
    public class NPersona
    {
        public static async Task<DataTable> Listar()
        {
            //Debemos instanciar la capa de acceso a datos
            DPersona datos = new DPersona();
            return await datos.Listar();
        }

        public static async Task<DataTable> ListarProveedores()
        {
            DPersona datos = new DPersona();
            return await datos.ListarProveedores();
        }

        public static async Task<DataTable> ListarClientes()
        {
            DPersona datos = new DPersona();
            return await datos.ListarClientes();
        }

        public static async Task<DataTable> Buscar(string valor)
        {
            DPersona datos = new DPersona();
            return await datos.Buscar(valor);
        }

        public static async Task<DataTable> BuscarProveedores(string valor)
        {
            DPersona datos = new DPersona();
            return await datos.BuscarProveedores(valor);
        }

        public static async Task<DataTable> BuscarClientes(string valor)
        {
            DPersona datos = new DPersona();
            return await datos.BuscarClientes(valor);
        }

        public static async Task<string> Insertar(string tipoPersona, string nombre, string tipoDocumento, string numDocumento, string direccion, string telefono, string email)
        {
            DPersona datos = new DPersona();

            //Verificar que documento, telefono y direccion no esten repetidos
            string campoDuplicado = await datos.ValidarDuplicados(0, numDocumento, telefono, direccion, email);
            if (campoDuplicado == "documento")
            {
                return "Ya existe una persona con ese numero de documento";
            }
            else if (campoDuplicado == "telefono")
            {
                return "Ya existe una persona con ese telefono";
            }
            else if (campoDuplicado == "direccion")
            {
                return "Ya existe una persona con esa direccion";
            }
            else if (campoDuplicado == "email")
            {
                return "Ya existe una persona con ese email";
            }

            //Primero debemos determinar si ya hay una Persona con ese nombre
            string existe = await datos.Existe(nombre);

            if (existe.Equals("1"))
            {
                return "Ya existe una persona con ese nombre";
            }
            else
            {
                // Creamos la Persona
                var persona = new Persona()
                {
                    TipoPersona = tipoPersona,
                    Nombre = nombre,
                    TipoDocumento = tipoDocumento,
                    NumDocumento = numDocumento,
                    Direccion = direccion,
                    Telefono = telefono,
                    Email = email
                };

                return await datos.Insertar(persona);
            }
        }

        public static async Task<string> Actualizar(int idPersona, string tipoPersona, string nombreAnterior, string nombre, string tipoDocumento, string numDocumento, string direccion, string telefono, string email)
        {
            DPersona datos = new DPersona();
            Persona persona = new Persona();

            if (nombreAnterior.Equals(nombre, StringComparison.OrdinalIgnoreCase))
            {
                //El usuario no cambio el nombre por ende no tenemos que investigar si ya existia
                persona.IdPersona = idPersona;
                persona.TipoPersona = tipoPersona;
                persona.Nombre = nombre;
                persona.TipoDocumento = tipoDocumento;
                persona.NumDocumento = numDocumento;
                persona.Direccion = direccion;
                persona.Telefono = telefono;
                persona.Email = email;

                return await datos.Actualizar(persona);
            }
            else
            {
                //El nombre fue cambiado, debemos investigar si existe alguna persona con ese nombre
                string existe = await datos.Existe(nombre);
                if (existe == "1")
                {
                    return "Ya existe una persona con ese nombre";
                }
                else
                {
                    persona.IdPersona = idPersona;
                    persona.TipoPersona = tipoPersona;
                    persona.Nombre = nombre;
                    persona.TipoDocumento = tipoDocumento;
                    persona.NumDocumento = numDocumento;
                    persona.Direccion = direccion;
                    persona.Telefono = telefono;
                    persona.Email = email;
                    return await datos.Actualizar(persona);
                }
            }
        }

        public static async Task<string> Eliminar(int id)
        {
            DPersona datos = new DPersona();
            return await datos.Eliminar(id);
        }
    }
}
