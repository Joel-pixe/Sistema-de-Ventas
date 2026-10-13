using Sistema.Entidad;
using System.Threading.Tasks;

namespace Sistema.Negocios.Interfaces
{
    public interface IEstadisticasService
    {
        Task<KpiCardsDto> ObtenerKpisAsync(FilterDto filtro);
        Task<VentasPeriodoDto> ObtenerVentasPorPeriodoAsync(FilterDto filtro);
        Task<TopProductosVendidosDto> ObtenerTopProductosVendidosAsync(FilterDto filtro);
        Task<VentasPorCategoriaDto> ObtenerVentasPorCategoriaAsync(FilterDto filtro);
        Task<IngresosVsVentasDto> ObtenerIngresosVsVentasAsync(FilterDto filtro);
        Task<TopClientesDto> ObtenerTopClientesAsync(FilterDto filtro);
        Task<AlertasStockDto> ObtenerAlertasStockAsync(int stockMinimoUmbral);
        Task<FiltrosEstadisticasDto> ObtenerFiltrosAsync(int? idUsuario);
    }
}
