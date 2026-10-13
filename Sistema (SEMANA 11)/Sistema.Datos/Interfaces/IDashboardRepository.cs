using Sistema.Entidad;
using System.Data;
using System.Threading.Tasks;

namespace Sistema.Datos.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DataTable> ObtenerKpis(FilterDto filtro);
        Task<DataTable> ObtenerTendencia(FilterDto filtro);
        Task<DataTable> ObtenerTopProductos(FilterDto filtro);
        Task<DataTable> ObtenerDistribucionCategorias(FilterDto filtro);
        Task<DataTable> ObtenerVendedores(int? idUsuario);
        Task<DataTable> ObtenerCategorias();
    }
}
