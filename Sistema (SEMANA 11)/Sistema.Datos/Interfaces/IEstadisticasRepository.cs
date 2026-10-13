using Sistema.Entidad;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Sistema.Datos.Interfaces
{
    // 8 consultas del módulo Estadísticas -> 7 stored procedures
    // (5 compartidos con el dashboard + 2 nuevos en Scripts/estadisticas_sps.sql)
    public interface IEstadisticasRepository
    {
        Task<DataTable> ObtenerKpis(FilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerTendencia(FilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerTopProductos(FilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerDistribucionCategorias(FilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerVentasPeriodo(FilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerStockBajo(StockFilterDto filtro, CancellationToken ct = default);
        Task<DataTable> ObtenerVendedores(int? idUsuario, CancellationToken ct = default);
        Task<DataTable> ObtenerCategorias(CancellationToken ct = default);
    }
}
