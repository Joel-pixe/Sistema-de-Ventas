using System;
using System.Collections.Generic;

namespace Sistema.Entidad
{
    // Filtros compartidos por todos los reportes del dashboard
    public record FilterDto(DateTime FechaInicio, DateTime FechaFin, int? IdUsuario, int? IdCategoria, string Granularidad, string Metrica);

    // 6 indicadores principales
    public record KpiCardsDto(decimal VentasTotales, decimal IngresosTotales, decimal TicketPromedio, int NumTransacciones, decimal VariacionVsPeriodoAnterior, decimal MargenBruto);

    // Punto de la serie temporal (ventas/ingresos por dia o mes)
    public record TrendPointDto(DateTime Fecha, string PeriodoLabel, decimal Ventas, decimal Ingresos);

    public record TrendDto(List<TrendPointDto> Puntos);

    // Top productos mas vendidos
    public record TopProductDto(int IdArticulo, string Codigo, string Nombre, string Categoria, int TotalCantidad, decimal TotalMonto);

    public record TopProductsDto(List<TopProductDto> Productos);

    // Distribucion de ingresos por categoria (con porcentaje)
    public record CategoryDto(int IdCategoria, string Nombre, decimal TotalIngresos, int TotalCantidad, decimal Porcentaje);

    public record CategoryDistributionDto(List<CategoryDto> Categorias);

    // Opciones de los filtros del dashboard
    public record VendedorDto(int Id, string NombreCompleto);

    public record CategoriaDto(int Id, string Nombre);

    public record FiltrosDisponiblesDto(List<VendedorDto> Vendedores, List<CategoriaDto> Categorias);
}

// Polyfill requerido para compilar records (init-only) sobre .NET Framework 4.8
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
