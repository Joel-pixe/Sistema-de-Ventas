using Sistema.Entidad;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Widgets
{
    // Fila de 6 tarjetas con los indicadores principales para FrmEstadisticas
    public class UcKpiCardsEstadisticas : UserControl
    {
        private readonly KpiCardControl CardVentas;
        private readonly KpiCardControl CardIngresos;
        private readonly KpiCardControl CardTicket;
        private readonly KpiCardControl CardTransacciones;
        private readonly KpiCardControl CardVariacion;
        private readonly KpiCardControl CardMargen;

        public UcKpiCardsEstadisticas()
        {
            BackColor = Color.WhiteSmoke;
            Dock = DockStyle.Fill;

            TableLayoutPanel Tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            Tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 6; i++)
            {
                Tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6666F));
            }

            CardVentas = new KpiCardControl("VENTAS TOTALES", "Del periodo seleccionado", Color.DodgerBlue);
            CardIngresos = new KpiCardControl("INGRESOS TOTALES", "Compras de mercancía", Color.MediumSeaGreen);
            CardTicket = new KpiCardControl("TICKET PROMEDIO", "Promedio por venta", Color.DarkOrange);
            CardTransacciones = new KpiCardControl("TRANSACCIONES", "Ventas aceptadas", Color.MediumVioletRed);
            CardVariacion = new KpiCardControl("VS. PERIODO ANTERIOR", "Crecimiento de ventas", Color.SteelBlue);
            CardMargen = new KpiCardControl("MARGEN BRUTO", "Ventas - costo estimado", Color.Teal);

            Tabla.Controls.Add(CardVentas, 0, 0);
            Tabla.Controls.Add(CardIngresos, 1, 0);
            Tabla.Controls.Add(CardTicket, 2, 0);
            Tabla.Controls.Add(CardTransacciones, 3, 0);
            Tabla.Controls.Add(CardVariacion, 4, 0);
            Tabla.Controls.Add(CardMargen, 5, 0);

            Controls.Add(Tabla);
        }

        public void Cargar(KpiCardsDto kpis)
        {
            if (kpis == null) return;

            CardVentas.Valor = kpis.VentasTotales.ToString("N2");
            CardIngresos.Valor = kpis.IngresosTotales.ToString("N2");
            CardTicket.Valor = kpis.TicketPromedio.ToString("N2");
            CardTransacciones.Valor = kpis.NumTransacciones.ToString("N0");

            CardVariacion.Valor = (kpis.VariacionVsPeriodoAnterior >= 0 ? "+" : "")
                + kpis.VariacionVsPeriodoAnterior.ToString("N2") + "%";
            CardVariacion.ColorValor = kpis.VariacionVsPeriodoAnterior >= 0
                ? Color.ForestGreen
                : Color.Crimson;

            CardMargen.Valor = kpis.MargenBruto.ToString("N2");
            CardMargen.ColorValor = kpis.MargenBruto >= 0
                ? Color.FromArgb(30, 30, 30)
                : Color.Crimson;
        }

        // Tarjeta individual: barra de acento + titulo + valor + subtitulo
        public class KpiCardControl : UserControl
        {
            private readonly Label LblTitulo;
            private readonly Label LblValor;
            private readonly Label LblSubtitulo;
            private readonly Color ColorBarra;

            public KpiCardControl(string titulo, string subtitulo, Color colorBarra)
            {
                ColorBarra = colorBarra;
                BackColor = Color.White;
                Margin = new Padding(6, 8, 6, 8);
                Padding = new Padding(12, 10, 10, 8);

                LblTitulo = new Label
                {
                    Dock = DockStyle.Top,
                    AutoSize = false,
                    Height = 32,
                    Text = titulo,
                    Font = new Font("Century Gothic", 8.5F, FontStyle.Bold),
                    ForeColor = Color.DimGray,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                LblValor = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "0.00",
                    Font = new Font("Century Gothic", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 30, 30),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                LblSubtitulo = new Label
                {
                    Dock = DockStyle.Bottom,
                    AutoSize = false,
                    Height = 22,
                    Text = subtitulo,
                    Font = new Font("Century Gothic", 7.5F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Controls.Add(LblValor);
                Controls.Add(LblSubtitulo);
                Controls.Add(LblTitulo);
            }

            public string Titulo
            {
                get { return LblTitulo.Text; }
                set { LblTitulo.Text = value; }
            }

            public string Valor
            {
                get { return LblValor.Text; }
                set { LblValor.Text = value; }
            }

            public string Subtitulo
            {
                get { return LblSubtitulo.Text; }
                set { LblSubtitulo.Text = value; }
            }

            public Color ColorValor
            {
                get { return LblValor.ForeColor; }
                set { LblValor.ForeColor = value; }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                // Borde suave y barra de acento a la izquierda
                using (Pen LapizBorde = new Pen(Color.Gainsboro))
                {
                    e.Graphics.DrawRectangle(LapizBorde, 0, 0, Width - 1, Height - 1);
                }
                using (Brush Pincel = new SolidBrush(ColorBarra))
                {
                    e.Graphics.FillRectangle(Pincel, 0, 0, 4, Height);
                }
            }
        }
    }
}