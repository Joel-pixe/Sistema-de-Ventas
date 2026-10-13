using Sistema.Entidad;
using System.Threading.Tasks;

namespace Sistema.Negocios.Interfaces
{
    public interface IDashboardService
    {
        Task<KpiCardsDto> ObtenerKpisAsync(FilterDto filtro);
        Task<TrendDto> ObtenerTendenciaAsync(FilterDto filtro);
        Task<TopProductsDto> ObtenerTopProductosAsync(FilterDto filtro);
        Task<CategoryDistributionDto> ObtenerDistribucionCategoriasAsync(FilterDto filtro);
        Task<FiltrosDisponiblesDto> ObtenerFiltrosAsync(int? idUsuario);
    }
}
