using Sistema.Entidad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Widgets
{
    public class UcAlertasStock : UserControl
    {
        private readonly DataGridView Grid;
        private readonly Label LblTitulo;
        private readonly ProgressBar PrgCargando;

        public UcAlertasStock()
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
                Text = "ALERTAS DE STOCK",
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White,
                    Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(8, 4, 8, 4)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Century Gothic", 9F),
                    Padding = new Padding(8, 4, 8, 4),
                    SelectionBackColor = Color.FromArgb(200, 220, 240),
                    SelectionForeColor = Color.Black
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 248, 248)
                },
                GridColor = Color.Gainsboro,
                RowTemplate = { Height = 36 }
            };

            Grid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ColCodigo", HeaderText = "CÓDIGO", DataPropertyName = "Codigo", FillWeight = 10 },
                new DataGridViewTextBoxColumn { Name = "ColNombre", HeaderText = "ARTÍCULO", DataPropertyName = "Nombre", FillWeight = 30 },
                new DataGridViewTextBoxColumn { Name = "ColCategoria", HeaderText = "CATEGORÍA", DataPropertyName = "Categoria", FillWeight = 15 },
                new DataGridViewTextBoxColumn { Name = "ColStockActual", HeaderText = "STOCK ACTUAL", DataPropertyName = "StockActual", FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "ColStockMinimo", HeaderText = "STOCK MÍNIMO", DataPropertyName = "StockMinimo", FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } },
                new DataGridViewTextBoxColumn { Name = "ColEstado", HeaderText = "ESTADO", DataPropertyName = "Estado", FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } }
            });

            PrgCargando = new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };

            Controls.Add(Grid);
            Controls.Add(PrgCargando);
            Controls.Add(LblTitulo);
        }

        public void Cargar(StockBajoDto datos)
        {
            MostrarCargando(false);

            Grid.Rows.Clear();

            if (datos?.Articulos == null || datos.Articulos.Count == 0)
                return;

            foreach (var alerta in datos.Articulos)
            {
                int rowIndex = Grid.Rows.Add(alerta.Codigo, alerta.Nombre, alerta.Categoria, alerta.StockActual, alerta.StockMinimo, alerta.Estado);

                // Colorear fila según estado (el SP devuelve 'Critico' sin tilde,
                // 'Bajo' y 'OK'; se compara sin distinguir mayúsculas/acento)
                string estado = (alerta.Estado ?? string.Empty).Trim();
                bool esCritico = string.Equals(estado, "Critico", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(estado, "Crítico", StringComparison.OrdinalIgnoreCase);
                bool esBajo = string.Equals(estado, "Bajo", StringComparison.OrdinalIgnoreCase);

                Color backColor;
                if (esCritico)
                    backColor = Color.FromArgb(255, 235, 235); // Rojo claro
                else if (esBajo)
                    backColor = Color.FromArgb(255, 248, 220); // Amarillo claro
                else
                    backColor = Color.FromArgb(235, 255, 235); // Verde claro

                Grid.Rows[rowIndex].DefaultCellStyle.BackColor = backColor;

                if (esCritico)
                {
                    Grid.Rows[rowIndex].DefaultCellStyle.Font = new Font("Century Gothic", 9F, FontStyle.Bold);
                }
            }

            Grid.ClearSelection();
        }

        public void MostrarCargando(bool cargando)
        {
            PrgCargando.Visible = cargando;
            Grid.Visible = !cargando;
        }
    }
}