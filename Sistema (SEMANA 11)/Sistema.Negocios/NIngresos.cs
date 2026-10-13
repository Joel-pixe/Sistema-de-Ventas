using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sistema.Datos.DIngresos;

namespace Sistema.Negocios
{
    public class NIngresos
    {
        //listar los ingresos
        public static async Task<DataTable> Listar()
        {
            //debemos instanciar la capa de acceso a datos
            DIngreso datos = new DIngreso();
            return await datos.Listar();
        }

        //Buscar o filtrar ingresos
        public static async Task<DataTable> Buscar(string valor)
        {
            DIngreso datos = new DIngreso();
            return await datos.Buscar(valor);
        }

        //Listar el detalle de un ingreso especifico
        public static async Task<DataTable> ListarDetalle(int idIngreso)
        {
            DIngreso datos = new DIngreso();
            return await datos.ListarDetalle(idIngreso);
        }

        //Insertar un ingreso (cabecera + detalle)
        public async static Task<string> Insertar(int idProveedor, int idUsuario,
            string tipoComprobante, string serieComprobante,
            string numeroComprobante, decimal impuesto,
            decimal total, DataTable detalle)
        {
            DIngreso datos = new DIngreso();

            // Validar que exista al menos un artículo en el detalle
            if (detalle == null || detalle.Rows.Count == 0)
            {
                return "Debe agregar al menos un artículo al detalle del ingreso";
            }

            // Crear el objeto Ingreso
            Ingreso ingreso = new Ingreso()
            {
                IdProveedor = idProveedor,
                IdUsuario = idUsuario,
                TipoComprobante = tipoComprobante,
                SerieComprobante = serieComprobante,
                NumeroComprobante = numeroComprobante,
                Fecha = DateTime.Now,
                Impuesto = impuesto,
                Total = total
            };

            // Insertar el ingreso y su detalle
            return await datos.Insertar(ingreso, detalle);



        }

        public static async Task<string> Anular(int id)
        {
            DIngreso datos = new DIngreso();
            return await datos.Anular(id);
        }
    }

}


