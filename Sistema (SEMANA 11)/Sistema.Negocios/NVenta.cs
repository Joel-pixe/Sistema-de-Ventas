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
    public class NVenta
    {
        //listar las ventas
        public static async Task<DataTable> Listar()
        {
            //debemos instanciar la capa de acceso a datos
            DVenta datos = new DVenta();
            return await datos.Listar();
        }

        //Buscar o filtrar ventas
        public static async Task<DataTable> Buscar(string valor)
        {
            DVenta datos = new DVenta();
            return await datos.Buscar(valor);
        }

        //Listar el detalle de una venta especifica
        public static async Task<DataTable> ListarDetalle(int idVenta)
        {
            DVenta datos = new DVenta();
            return await datos.ListarDetalle(idVenta);
        }

        //Mostrar el comprobante (factura) de una venta especifica
        public static async Task<DataTable> MostrarComprobante(int idVenta)
        {
            DVenta datos = new DVenta();
            return await datos.MostrarComprobante(idVenta);
        }

        //Insertar una venta (cabecera + detalle)
        public static async Task<string> Insertar(int idCliente, int idUsuario,
            string tipoComprobante, string serieComprobante,
            int numComprobante, decimal impuesto,
            decimal total, DataTable detalle)
        {
            DVenta datos = new DVenta();

            // Validar que exista al menos un artículo en el detalle
            if (detalle == null || detalle.Rows.Count == 0)
            {
                return "Debe agregar al menos un artículo al detalle de la venta";
            }

            // Crear el objeto Venta
            Venta venta = new Venta()
            {
                IdCliente = idCliente,
                IdUsuario = idUsuario,
                TipoComprobante = tipoComprobante,
                SerieComprobante = serieComprobante,
                NumComprobante = numComprobante,
                Fecha = DateTime.Now,
                Impuesto = impuesto,
                Total = total
            };

            // Insertar la venta y su detalle
            return await datos.Insertar(venta, detalle);



        }

        public static async Task<string> Anular(int id)
        {
            DVenta datos = new DVenta();
            return await datos.Anular(id);
        }
    }
}
