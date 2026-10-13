using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.Measure;
using SkiaSharp;
using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1.Helpers;

namespace WindowsFormsApp1.Widgets
{
    // Serie temporal de ventas por período con zoom, pan y exportación
    public class UcVentasPeriodo : UserControl
    {
        private readonly CartesianChart _chart;
        private readonly Label _lblTitulo;
        private readonly ToolStrip _toolbar;
        private readonly ToolStripButton _btnExportPng;
        private readonly ToolStripButton _btnZoomReset;
        private readonly ProgressBar _prgCargando;
        private FilterDto _currentFilter;

        public UcVentasPeriodo()
        {
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            Margin = new Padding(6);

            // ---- Toolbar ----
            _toolbar = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Color.WhiteSmoke,
                RenderMode = ToolStripRenderMode.Professional,
                Padding = new Padding(4, 2, 4, 2)
            };
            _btnExportPng = new ToolStripButton("💾 Exportar PNG") { Enabled = false, Margin = new Padding(4, 0, 0, 0) };
            _btnZoomReset = new ToolStripButton("🔍 Reset Zoom") { Enabled = false, Margin = new Padding(4, 0, 0, 0) };
            _toolbar.Items.Add(_btnExportPng);
            _toolbar.Items.Add(_btnZoomReset);

            _lblTitulo = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 34,
                Padding = new Padding(10, 0, 0, 0),
                Text = "VENTAS POR PERÍODO",
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

            _btnExportPng.Click += BtnExportPng_Click;
            _btnZoomReset.Click += (s, e) => _chart.ZoomMode = _chart.ZoomMode; // Reset zoom - simplified

            Controls.Add(_chart);
            Controls.Add(_toolbar);
            Controls.Add(_lblTitulo);
            Controls.Add(_prgCargando);
        }

        public void Cargar(VentasPeriodoDto dto, FilterDto filtro)
        {
            _currentFilter = filtro;
            MostrarCargando(false);
            _btnExportPng.Enabled = dto?.Puntos != null && dto.Puntos.Count > 0;
            _btnZoomReset.Enabled = dto?.Puntos != null && dto.Puntos.Count > 0;

            if (dto?.Puntos == null || dto.Puntos.Count == 0)
            {
                _chart.Series = Array.Empty<ISeries>();
                _chart.XAxes = new Axis[] { new Axis { Labels = Array.Empty<string>(), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                _chart.YAxes = new Axis[] { new Axis { Labeler = value => value.ToString("N0"), LabelsPaint = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColors.DimGray) } };
                return;
            }

            var valores = new List<double>();
            var labels = new List<string>();

            foreach (var punto in dto.Puntos)
            {
                valores.Add((double)punto.Valor);
                labels.Add(punto.PeriodoLabel);
            }

            string colorSerie = dto.Metrica == "CANTIDAD" ? "#FF9800" : "#1E88E5";

            var series = new LineSeries<double>
            {
                Name = dto.Metrica == "CANTIDAD" ? "Cantidad" : "Monto",
                Values = valores,
                GeometrySize = 6,
                Stroke = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColor.Parse(colorSerie), 3),
                Fill = new LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint(SKColor.Parse(colorSerie + "30")),
                LineSmoothness = 0.3
            };

            _chart.Series = new ISeries[] { series };
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

        private void BtnExportPng_Click(object sender, EventArgs e)
        {
            if (_chart.Series == null) return;

            using (var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                FileName = $"VentasPeriodo_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ChartExportHelper.ExportToPng(_chart, sfd.FileName);
                        MessageBox.Show("Exportado a PNG correctamente.", "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exportando: {ex.Message}", "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void MostrarCargando(bool cargando)
        {
            _prgCargando.Visible = cargando;
            _chart.Visible = !cargando;
        }
    }
}