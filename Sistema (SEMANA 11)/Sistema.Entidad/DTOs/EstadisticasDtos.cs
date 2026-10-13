using System;
using System.Collections.Generic;

namespace Sistema.Entidad
{
    // DTOs exclusivos del módulo Estadísticas.
    // Los tipos compartidos con el dashboard (FilterDto, KpiCardsDto, TrendDto,
    // TopProductsDto, CategoryDistributionDto y FiltrosDisponiblesDto) viven en
    // DTOs/DashboardDtos.cs y se reutilizan tal cual.

    // Punto de la serie de ventas por período (pestaña "Ventas Período").
    // Valor = la métrica seleccionada (MONTO -> Ventas, CANTIDAD -> Cantidad),
    // precalculada en la capa Negocios para que el widget solo la grafique.
    public record VentasPeriodoPointDto(DateTime Fecha, string PeriodoLabel, decimal Ventas, int Cantidad, decimal Valor);

    public record VentasPeriodoDto(string Granularidad, string Metrica, List<VentasPeriodoPointDto> Puntos);

    // Filtro de la pestaña "Stock Bajo"
    public record StockFilterDto(int Umbral, int? IdCategoria);

    // Artículo en situación de alerta (Stock Bajo / Crítico)
    public record StockBajoItemDto(int IdArticulo, string Codigo, string Nombre, string Categoria, int StockActual, int StockMinimo, string Estado);

    public record StockBajoDto(List<StockBajoItemDto> Articulos);
}
