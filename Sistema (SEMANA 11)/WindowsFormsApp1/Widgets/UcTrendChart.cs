using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.Measure;
using SkiaSharp;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Widgets
{
    // Gráfico de tendencia dual: Ventas e Ingresos (línea suave con área)
    public class UcTrendChart : UserControl
    {
        private readonly CartesianChart _chart;
        private readonly Label _lblTitulo;
        private readonly ProgressBar _prgCargando;
        private FilterDto _currentFilter;

        public UcTrendChart()
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
                Text = "TENDENCIA DE VENTAS E INGRESOS",
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

            _chart.ZoomMode = ZoomAndPanMode.X;

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

        public void Cargar(TrendDto dto, FilterDto filtro)
        {
            _currentFilter = filtro;
            MostrarCargando(false);

            if (dto?.Puntos == null || dto.Puntos.Count == 0)
            {
                _chart.Series = Array.Empty<ISeries>();
                _chart.XAxes = new Axis[] { new Axis { Labels = Array.Empty<string>(), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                _chart.YAxes = new Axis[] { new Axis { Labeler = value => value.ToString("N0"), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                return;
            }

            var ventasValues = new List<double>();
            var ingresosValues = new List<double>();
            var labels = new List<string>();

            foreach (var punto in dto.Puntos)
            {
                ventasValues.Add((double)punto.Ventas);
                ingresosValues.Add((double)punto.Ingresos);
                labels.Add(punto.PeriodoLabel);
            }

            var ventasSeries = new LineSeries<double>
            {
                Name = "Ventas",
                Values = ventasValues,
                GeometrySize = 6,
                Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SKColor(30, 136, 229), 3),
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SKColor(30, 136, 229, 30)),
                LineSmoothness = 0.3
            };

            var ingresosSeries = new LineSeries<double>
            {
                Name = "Ingresos",
                Values = ingresosValues,
                GeometrySize = 6,
                Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SKColor(76, 175, 80), 3),
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(new SKColor(76, 175, 80, 30)),
                LineSmoothness = 0.3
            };

            _chart.Series = new ISeries[] { ventasSeries, ingresosSeries };
            _chart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = labels.Count > 12 ? 45 : 0,
                    LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray),
                    SeparatorsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.LightGray) { StrokeThickness = 0.5f },
                    TextSize = 11
                }
            };
            _chart.YAxes = new Axis[]
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