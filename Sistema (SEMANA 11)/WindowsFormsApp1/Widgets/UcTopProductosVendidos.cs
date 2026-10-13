using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Widgets
{
    public class UcTopProductosVendidos : UserControl
    {
        private readonly CartesianChart Chart;
        private readonly Label LblTitulo;
        private readonly ProgressBar PrgCargando;

        public UcTopProductosVendidos()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            Margin = new Padding(6);

            LblTitulo = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 34,
                Padding = new Padding(10, 0, 0, 0),
                Text =  TOP 10 PRODUCTOS MÁS VENDIDOS,
                Font = new Font(Century Gothic, 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Top,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
            };

            PrgCargando = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };

            Controls.Add(Chart);
            Controls.Add(PrgCargando);
            Controls.Add(LblTitulo);
        }

        public void Cargar(TopProductosVendidosDto datos)
        {
            MostrarCargando(false);

            if (datos?.Productos == null || datos.Productos.Count == 0)
            {
                Chart.Series = Array.Empty<ISeries>();
                Chart.XAxes = Array.Empty<Axis>();
                Chart.YAxes = Array.Empty<Axis>();
                return;
            }

            var productos = datos.Productos.OrderBy(p => p.TotalCantidad).ToList(); // Ascendente para barras horizontales

            var serieCantidad = new RowSeries<int>
            {
                Name = Cantidad Vendida,
                Values = productos.Select(p => p.TotalCantidad).ToArray(),
                Fill = new SolidColorPaint(new SKColor(30, 144, 255)), // DodgerBlue
                Stroke = null,
                MaxBarWidth = 30
            };

            Chart.Series = new ISeries[] { serieCantidad };

            Chart.XAxes = new Axis[]
            {
                new Axis
                {
                    Name = Cantidad,
                    NamePaint = new SolidColorPaint(SKColors.DimGray),
                    Labeler = value => value.ToString(N0),
                    LabelsPaint = new SolidColorPaint(SKColors.DimGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gainsboro),
                    MinLimit = 0
                }
            };

            Chart.YAxes = new Axis[]
            {
                new Axis
                {
                    Labels = productos.Select(p => p.Nombre.Length > 25 ? p.Nombre.Substring(0, 25) + ... : p.Nombre).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.DimGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gainsboro),
                    TextSize = 10
                }
            };
        }

        public void MostrarCargando(bool cargando)
        {
            PrgCargando.Visible = cargando;
            Chart.Visible = !cargando;
        }
    }
}
