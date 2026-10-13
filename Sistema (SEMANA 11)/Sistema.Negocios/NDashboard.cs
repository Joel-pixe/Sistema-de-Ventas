using Sistema.Datos;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sistema.Negocios
{
    // Los metodos son estaticos al igual que el resto de las clases N* del proyecto.
    // El filtro por rol se inyecta en FilterDto.IdUsuario / ObtenerFiltrosAsync desde
    // Presentacion (capa que tiene acceso a Variables.Rol e Variables.IdUsuario).
    public class NDashboard
    {
        public static async Task<KpiCardsDto> ObtenerKpisAsync(FilterDto filtro)
        {
            DDashboard datos = new DDashboard();
            DataTable Tabla = await datos.ObtenerKpis(filtro);

            if (Tabla.Rows.Count == 0)
            {
                return new KpiCardsDto(0, 0, 0, 0, 0, 0);
            }

            DataRow Fila = Tabla.Rows[0];
            return new KpiCardsDto(
                Convert.ToDecimal(Fila["VentasTotales"]),
                Convert.ToDecimal(Fila["IngresosTotales"]),
                Convert.ToDecimal(Fila["TicketPromedio"]),
                Convert.ToInt32(Fila["NumTransacciones"]),
                Convert.ToDecimal(Fila["VariacionVsPeriodoAnterior"]),
                Convert.ToDecimal(Fila["MargenBruto"]));
        }

        public static async Task<TrendDto> ObtenerTendenciaAsync(FilterDto filtro)
        {
            DDashboard datos = new DDashboard();
            DataTable Tabla = await datos.ObtenerTendencia(filtro);

            List<TrendPointDto> Puntos = new List<TrendPointDto>();
            foreach (DataRow Fila in Tabla.Rows)
            {
                Puntos.Add(new TrendPointDto(
                    Convert.ToDateTime(Fila["Fecha"]),
                    Convert.ToString(Fila["PeriodoLabel"]),
                    Convert.ToDecimal(Fila["Ventas"]),
                    Convert.ToDecimal(Fila["Ingresos"])));
            }

            return new TrendDto(Puntos);
        }

        public static async Task<TopProductsDto> ObtenerTopProductosAsync(FilterDto filtro)
        {
            DDashboard datos = new DDashboard();
            DataTable Tabla = await datos.ObtenerTopProductos(filtro);

            List<TopProductDto> Productos = new List<TopProductDto>();
            foreach (DataRow Fila in Tabla.Rows)
            {
                Productos.Add(new TopProductDto(
                    Convert.ToInt32(Fila["IdArticulo"]),
                    Convert.ToString(Fila["Codigo"]),
                    Convert.ToString(Fila["Nombre"]),
                    Convert.ToString(Fila["Categoria"]),
                    Convert.ToInt32(Fila["TotalCantidad"]),
                    Convert.ToDecimal(Fila["TotalMonto"])));
            }

            return new TopProductsDto(Productos);
        }

        public static async Task<CategoryDistributionDto> ObtenerDistribucionCategoriasAsync(FilterDto filtro)
        {
            DDashboard datos = new DDashboard();
            DataTable Tabla = await datos.ObtenerDistribucionCategorias(filtro);

            List<CategoryDto> Categorias = new List<CategoryDto>();
            foreach (DataRow Fila in Tabla.Rows)
            {
                Categorias.Add(new CategoryDto(
                    Convert.ToInt32(Fila["IdCategoria"]),
                    Convert.ToString(Fila["Nombre"]),
                    Convert.ToDecimal(Fila["TotalIngresos"]),
                    Convert.ToInt32(Fila["TotalCantidad"]),
                    Convert.ToDecimal(Fila["Porcentaje"])));
            }

            return new CategoryDistributionDto(Categorias);
        }

        // idUsuario = NULL -> lista todos los vendedores (Administrador)
        // idUsuario con valor -> lista solo ese vendedor (permisos por rol)
        public static async Task<FiltrosDisponiblesDto> ObtenerFiltrosAsync(int? idUsuario)
        {
            DDashboard datos = new DDashboard();

            Task<DataTable> TareaVendedores = datos.ObtenerVendedores(idUsuario);
            Task<DataTable> TareaCategorias = datos.ObtenerCategorias();
            await Task.WhenAll(TareaVendedores, TareaCategorias);

            List<VendedorDto> Vendedores = new List<VendedorDto>();
            foreach (DataRow Fila in TareaVendedores.Result.Rows)
            {
                Vendedores.Add(new VendedorDto(
                    Convert.ToInt32(Fila["Id"]),
                    Convert.ToString(Fila["NombreCompleto"])));
            }

            List<CategoriaDto> Categorias = new List<CategoriaDto>();
            foreach (DataRow Fila in TareaCategorias.Result.Rows)
            {
                Categorias.Add(new CategoriaDto(
                    Convert.ToInt32(Fila["Id"]),
                    Convert.ToString(Fila["Nombre"])));
            }

            return new FiltrosDisponiblesDto(Vendedores, Categorias);
        }
    }
}
