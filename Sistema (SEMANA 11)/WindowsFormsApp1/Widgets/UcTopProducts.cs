using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.Measure;
using SkiaSharp;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Widgets
{
    // Ranking Top 10 productos (barras horizontales)
    public class UcTopProducts : UserControl
    {
        private readonly CartesianChart _chart;
        private readonly Label _lblTitulo;
        private readonly ProgressBar _prgCargando;

        public UcTopProducts()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            Margin = new Padding(6);

            _lblTitulo = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 34,
                Padding = new Padding(10, 0, 0, 0),
                Text = "TOP 10 PRODUCTOS MÁS VENDIDOS",
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                Series = Array.Empty<ISeries>(),
                TooltipPosition = TooltipPosition.Top,
                LegendPosition = LegendPosition.Top,
                AnimationsSpeed = TimeSpan.FromMilliseconds(400),
                EasingFunction = LiveChartsCore.EasingFunctions.BounceOut
            };

            _prgCargando = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };

            Controls.Add(_chart);
            Controls.Add(_prgCargando);
            Controls.Add(_lblTitulo);
        }

        public void Cargar(TopProductsDto dto)
        {
            MostrarCargando(false);

            if (dto?.Productos == null || dto.Productos.Count == 0)
            {
                _chart.Series = Array.Empty<ISeries>();
                _chart.YAxes = new Axis[] { new Axis { Labels = Array.Empty<string>(), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                _chart.XAxes = new Axis[] { new Axis { Labeler = value => value.ToString("N0"), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                return;
            }

            // Tomar Top 10 y revertir para que el mayor quede arriba
            var data = dto.Productos.Take(10).ToList();
            data.Reverse();

            var values = data.Select(p => (double)(p.TotalMonto > 0 ? p.TotalMonto : p.TotalCantidad)).ToArray();
            var labels = data.Select(p => p.Nombre).ToArray();

            var series = new RowSeries<double>
            {
                Values = values,
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SKColor(30, 144, 255, 200)),
                Stroke = null
            };

            _chart.Series = new ISeries[] { series };
            _chart.YAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray),
                    SeparatorsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    TextSize = 11
                }
            };
            _chart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("N0"),
                    LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray),
                    SeparatorsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    TextSize = 11
                }
            };
        }

        public void MostrarCargando(bool cargando)
        {
            _prgCargando.Visible = cargando;
            _chart.Visible = !cargando;
        }
    }
}