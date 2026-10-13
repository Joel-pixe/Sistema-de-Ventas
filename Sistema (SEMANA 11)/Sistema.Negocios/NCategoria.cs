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
    public class NCategoria
    {
        //listar las categorias
        public static async Task<DataTable> Listar()
        {
            //debemos instanciar la capa de acceso a datos
            DCategoria datos = new DCategoria();
            return await datos.Listar();
        }

        //Buscar o filtrar categorias
        public static async Task<DataTable> Buscar(string valor)
        {
            DCategoria datos = new DCategoria();
            return await datos.Buscar(valor);
        }

        //Insertar una categoria
        public static async Task<string> Insertar(string nombre, string descripcion)
        {
            DCategoria datos = new DCategoria();
            //primero debemos determinar si ya hay una categoria con ese nombre
            string existe = await datos.Existe(nombre);
            if (existe.Equals("1"))
            {
                return "Existe una categoría con ese nombre";
            }
            else
            {
                //creamos la categoria
                var Categoria = new Categoria()
                {
                    Nombre = nombre,
                    Descripcion = descripcion
                };

                return await datos.Insertar(Categoria);
            }

        }

        //Actualizar una categoria
        public static async Task<string> Actualizar(int id, string nombreAnterior, string nombre, string descripcion)
        {
            DCategoria datos = new DCategoria();
            Categoria categoria = new Categoria();

            if (nombreAnterior.Equals(nombre, StringComparison.OrdinalIgnoreCase))
            {
                //el usuario no cambio el nombre por ende no tenemos que investigar si ya existia
                categoria.IdCategoria = id;
                categoria.Nombre = nombre;
                categoria.Descripcion = descripcion;
                return await datos.Actualizar(categoria);
            }
            else
            {
                //el nombre fue cambiado, debemos investigar si existe alguna categoria con ese nombre
                string existe = await datos.Existe(nombre);
                if (existe == "1")
                {
                    return "Ya existe una categoria con ese nombre";
                }
                else
                {
                    categoria.IdCategoria = id;
                    categoria.Nombre = nombre;
                    categoria.Descripcion = descripcion;
                    return await datos.Actualizar(categoria);
                }
            }
        }

        //Eliminar una categoria
        public static async Task<string> Eliminar(int id)
        {
            DCategoria datos = new DCategoria();
            return await datos.Eliminar(id);
        }

        //Activar una categoria 
        public static async Task<string> Activar(int id)
        {
            DCategoria datos = new DCategoria();
            return await datos.Activar(id);
        }

        //Desactivar una categoria
        public static async Task<string> Desactivar(int id)
        {
            DCategoria datos = new DCategoria();
            return await datos.Desactivar(id);
        }
    }
}
