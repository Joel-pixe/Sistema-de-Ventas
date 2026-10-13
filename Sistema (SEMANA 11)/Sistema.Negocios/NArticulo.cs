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
    public class NArticulo
    {
        public static async Task<DataTable> Listar()
        {
            //Debemos instanciar la capa de acceso a datos
            DArticulo datos = new DArticulo();
            return await datos.Listar();
        }

        public static async Task<DataTable> Buscar(string valor)
        {
            DArticulo datos = new DArticulo();
            return await datos.Buscar(valor);
        }

        public static async Task<DataTable> BuscarCodigo(string valor)
        {
            DArticulo datos = new DArticulo();
            return await datos.BuscarCodigo(valor);
        }

        public static async Task<string> Insertar(int idCategoria, string codigo, string nombre, decimal precioVenta, int stock, string descripcion, string imagen)
        {
            DArticulo datos = new DArticulo();

            //Primero verificamos si ya existe un Articulo con ese codigo
            DataTable existeCodigo = await datos.BuscarCodigo(codigo);
            if (existeCodigo.Rows.Count > 0)
            {
                return "Ya existe un articulo con ese codigo";
            }

            //Luego determinar si ya hay un Articulo con ese nombre
            string existe = await datos.Existe(nombre);

            if (existe.Equals("1"))
            {
                return "Existe un articulo con ese nombre";
            }
            else
            {
                // Creamos el Articulo
                var Articulo = new Articulo()
                {
                    IdCategoria = idCategoria,
                    Codigo = codigo,
                    Nombre = nombre,
                    PrecioVenta = precioVenta,
                    Stock = stock,
                    Descripcion = descripcion,
                    Imagen = imagen
                };

                return await datos.Insertar(Articulo);
            }

        }

        public static async Task<string> Actualizar(int idArticulo, int idCategoria, string codigo, string nombreAnterior, string nombre, decimal precioVenta, int stock, string descripcion, string imagen)
        {
            DArticulo datos = new DArticulo();
            Articulo articulo = new Articulo();

            if (nombreAnterior.Equals(nombre, StringComparison.OrdinalIgnoreCase))
            {
                //El usuario no cambio el nombre por ende no tenemos que investigar si ya existia
                articulo.IdArticulo = idArticulo;
                articulo.IdCategoria = idCategoria;
                articulo.Codigo = codigo;
                articulo.Nombre = nombre;
                articulo.PrecioVenta = precioVenta;
                articulo.Stock = stock;
                articulo.Descripcion = descripcion;
                articulo.Imagen = imagen;

                return await datos.Actualizar(articulo);
            }
            else
            {
                //El nombre fue cambiado, debemos investigar si existe alguna categoria con ese nombre
                string existe = await datos.Existe(nombre);
                if (existe == "1")
                {
                    return "Ya existe un articulo con ese nombre";
                }
                else
                {
                    articulo.IdArticulo = idArticulo;
                    articulo.IdCategoria = idCategoria;
                    articulo.Codigo = codigo;
                    articulo.Nombre = nombre;
                    articulo.PrecioVenta = precioVenta;
                    articulo.Stock = stock;
                    articulo.Descripcion = descripcion;
                    articulo.Imagen = imagen;
                    return await datos.Actualizar(articulo);
                }
            }
        }

        public static async Task<string> Eliminar(int id)
        {
            DArticulo datos = new DArticulo();
            return await datos.Eliminar(id);
        }

        public async static Task<string> Activar(int id)
        {
            DArticulo datos = new DArticulo();
            return await datos.Activar(id);
        }

        public async static Task<string> Desactivar(int id)
        {
            DArticulo datos = new DArticulo();
            return await datos.Desactivar(id);
        }
    }
}
