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
    // Distribución de ventas por categoría (gráfico dona)
    public class UcCategoryDonut : UserControl
    {
        private readonly PieChart _chart;
        private readonly Label _lblTitulo;
        private readonly ProgressBar _prgCargando;

        public UcCategoryDonut()
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
                Text = "DISTRIBUCIÓN POR CATEGORÍA",
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _chart = new PieChart
            {
                Dock = DockStyle.Fill,
                Series = Array.Empty<ISeries>(),
                TooltipPosition = TooltipPosition.Top,
                LegendPosition = LegendPosition.Right,
                AnimationsSpeed = TimeSpan.FromMilliseconds(500),
                EasingFunction = LiveChartsCore.EasingFunctions.BounceOut,
                InitialRotation = -90,
                MaxAngle = 360
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

        public void Cargar(CategoryDistributionDto dto)
        {
            MostrarCargando(false);

            if (dto?.Categorias == null || dto.Categorias.Count == 0)
            {
                _chart.Series = Array.Empty<ISeries>();
                return;
            }

            // Colores para las categorías
            var colores = new[]
            {
                new SKColor(30, 136, 229),    // Azul
                new SKColor(76, 175, 80),     // Verde
                new SKColor(255, 152, 0),     // Naranja
                new SKColor(156, 39, 176),    // Púrpura
                new SKColor(233, 30, 99),     // Rosa
                new SKColor(0, 150, 136),     // Teal
                new SKColor(255, 87, 34),     // Rojo naranja
                new SKColor(63, 81, 181),     // Índigo
                new SKColor(205, 220, 57),    // Lima
                new SKColor(96, 125, 139)     // Azul gris
            };

            var series = new List<PieSeries<double>>();
            for (int i = 0; i < dto.Categorias.Count; i++)
            {
                var cat = dto.Categorias[i];
                var color = colores[i % colores.Length];

                series.Add(new PieSeries<double>
                {
                    Values = new double[] { (double)cat.TotalIngresos },
                    Name = cat.Nombre,
                    Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(color),
                    Stroke = null,
                    InnerRadius = 60,
                    Pushout = 2,
                    DataLabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12
                });
            }

            _chart.Series = series;
        }

        public void MostrarCargando(bool cargando)
        {
            _prgCargando.Visible = cargando;
            _chart.Visible = !cargando;
        }
    }
}