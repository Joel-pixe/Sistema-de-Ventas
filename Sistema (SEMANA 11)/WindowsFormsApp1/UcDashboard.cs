using Sistema.Entidad;
using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Widgets;

namespace WindowsFormsApp1
{
    // Contenedor principal del modulo Dashboard Analytics
    public class UcDashboard : UserControl
    {
        private readonly DateTimePicker DtpInicio;
        private readonly DateTimePicker DtpFin;
        private readonly ComboBox CmbVendedor;
        private readonly ComboBox CmbCategoria;
        private readonly ComboBox CmbGranularidad;
        private readonly ComboBox CmbMetrica;
        private readonly ComboBox CmbAutoRefresh;
        private readonly Button BtnActualizar;
        private readonly Label LblUltimaActualizacion;

        private readonly UcKpiCards UcKpis;
        private readonly UcTrendChart UcTendencia;
        private readonly UcTopProducts UcTopProductos;
        private readonly UcCategoryDonut UcDistribucion;

        private readonly ProgressBar PrgKpis;
        private readonly ProgressBar PrgTendencia;
        private readonly ProgressBar PrgTopProductos;
        private readonly ProgressBar PrgDistribucion;

        private readonly Timer TimerAutoRefresh;
        private bool Cargando;

        public UcDashboard()
        {
            BackColor = Color.WhiteSmoke;
            Dock = DockStyle.Fill;

            // ---------------- Barra de filtros ----------------
            Panel PnlFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 96,
                BackColor = Color.White,
                Padding = new Padding(10, 8, 10, 8)
            };

            TableLayoutPanel TlpFiltros = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 1,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            TlpFiltros.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F)); // Desde
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F)); // Hasta
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F)); // Vendedor
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F)); // Categoria
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F)); // Granularidad
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F)); // Metrica
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14F)); // Auto-refresh
            TlpFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // Acciones

            DtpInicio = CrearFecha();
            DtpFin = CrearFecha();
            CmbVendedor = CrearCombo();
            CmbCategoria = CrearCombo();
            CmbGranularidad = CrearCombo();
            CmbMetrica = CrearCombo();
            CmbAutoRefresh = CrearCombo();

            DtpInicio.Value = DateTime.Today.AddDays(-29);
            DtpFin.Value = DateTime.Today;

            CmbGranularidad.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 1, Valor = "DIA", Nombre = "Día" },
                new OpcionFiltro { Id = 2, Valor = "MES", Nombre = "Mes" }
            };
            CmbMetrica.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 1, Valor = "MONTO", Nombre = "Monto" },
                new OpcionFiltro { Id = 2, Valor = "CANTIDAD", Nombre = "Cantidad" }
            };
            CmbAutoRefresh.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 0, Valor = "", Nombre = "OFF" },
                new OpcionFiltro { Id = 5, Valor = "", Nombre = "5 minutos" },
                new OpcionFiltro { Id = 15, Valor = "", Nombre = "15 minutos" },
                new OpcionFiltro { Id = 30, Valor = "", Nombre = "30 minutos" },
                new OpcionFiltro { Id = 60, Valor = "", Nombre = "60 minutos" }
            };

            TlpFiltros.Controls.Add(CrearCampo("DESDE", DtpInicio), 0, 0);
            TlpFiltros.Controls.Add(CrearCampo("HASTA", DtpFin), 1, 0);
            TlpFiltros.Controls.Add(CrearCampo("VENDEDOR", CmbVendedor), 2, 0);
            TlpFiltros.Controls.Add(CrearCampo("CATEGORÍA", CmbCategoria), 3, 0);
            TlpFiltros.Controls.Add(CrearCampo("GRANULARIDAD", CmbGranularidad), 4, 0);
            TlpFiltros.Controls.Add(CrearCampo("MÉTRICA", CmbMetrica), 5, 0);
            TlpFiltros.Controls.Add(CrearCampo("ACTUALIZACIÓN AUTOMÁTICA", CmbAutoRefresh), 6, 0);

            Panel PnlAcciones = new Panel { Dock = DockStyle.Fill };
            BtnActualizar = new Button
            {
                Dock = DockStyle.Top,
                Height = 34,
                Text = "ACTUALIZAR",
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnActualizar.FlatAppearance.BorderSize = 0;
            LblUltimaActualizacion = new Label
            {
                Dock = DockStyle.Bottom,
                AutoSize = false,
                Height = 26,
                Text = "Sin actualizar aún",
                Font = new Font("Century Gothic", 7.5F, FontStyle.Regular),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };
            PnlAcciones.Controls.Add(LblUltimaActualizacion);
            PnlAcciones.Controls.Add(BtnActualizar);
            TlpFiltros.Controls.Add(CrearCampo("", PnlAcciones), 7, 0);

            PnlFiltros.Controls.Add(TlpFiltros);

            // ---------------- Area de widgets ----------------
            Panel PnlCuerpo = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            TableLayoutPanel TlpWidgets = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 12,
                RowCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            for (int i = 0; i < 12; i++)
            {
                TlpWidgets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.3333F));
            }
            TlpWidgets.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            TlpWidgets.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
            TlpWidgets.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));

            UcKpis = new UcKpiCards();
            UcTendencia = new UcTrendChart();
            UcTopProductos = new UcTopProducts();
            UcDistribucion = new UcCategoryDonut();

            PrgKpis = CrearSkeleton();
            PrgTendencia = CrearSkeleton();
            PrgTopProductos = CrearSkeleton();
            PrgDistribucion = CrearSkeleton();

            TlpWidgets.Controls.Add(CrearContenedorWidget(UcKpis, PrgKpis), 0, 0);
            TlpWidgets.SetColumnSpan(TlpWidgets.GetControlFromPosition(0, 0), 12);
            TlpWidgets.Controls.Add(CrearContenedorWidget(UcTendencia, PrgTendencia), 0, 1);
            TlpWidgets.SetColumnSpan(TlpWidgets.GetControlFromPosition(0, 1), 8);
            TlpWidgets.Controls.Add(CrearContenedorWidget(UcDistribucion, PrgDistribucion), 8, 1);
            TlpWidgets.Controls.Add(CrearContenedorWidget(UcTopProductos, PrgTopProductos), 0, 2);
            TlpWidgets.SetColumnSpan(TlpWidgets.GetControlFromPosition(0, 2), 12);

            PnlCuerpo.Controls.Add(TlpWidgets);

            Controls.Add(PnlCuerpo);
            Controls.Add(PnlFiltros);

            // ---------------- Timer de auto-refresh ----------------
            TimerAutoRefresh = new Timer();
            TimerAutoRefresh.Tick += TimerAutoRefresh_Tick;

            BtnActualizar.Click += BtnActualizar_Click;
            CmbAutoRefresh.SelectedIndexChanged += CmbAutoRefresh_SelectedIndexChanged;
            Load += UcDashboard_Load;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && TimerAutoRefresh != null)
            {
                TimerAutoRefresh.Stop();
                TimerAutoRefresh.Dispose();
            }
            base.Dispose(disposing);
        }

        private async void UcDashboard_Load(object sender, EventArgs e)
        {
            ApplyRolePermissions();
            await LoadFiltersAsync();
            await LoadDashboardAsync();
        }

        // Aplica los permisos de la interfaz segun el rol de la sesion
        public void ApplyRolePermissions()
        {
            bool esAdministrador = Variables.Rol == "Administrador";
            // Solo un Administrador puede consultar las ventas de otros vendedores
            CmbVendedor.Enabled = esAdministrador;
        }

        private async Task LoadFiltersAsync()
        {
            try
            {
                int? idUsuarioFiltro = Variables.Rol != "Administrador" ? Variables.IdUsuario : (int?)null;
                FiltrosDisponiblesDto filtros = await NDashboard.ObtenerFiltrosAsync(idUsuarioFiltro);

                List<OpcionFiltro> vendedores = new List<OpcionFiltro>();
                if (Variables.Rol == "Administrador")
                {
                    vendedores.Add(new OpcionFiltro { Id = 0, Valor = "", Nombre = "Todos" });
                }
                foreach (VendedorDto vendedor in filtros.Vendedores)
                {
                    vendedores.Add(new OpcionFiltro { Id = vendedor.Id, Valor = vendedor.Id.ToString(), Nombre = vendedor.NombreCompleto });
                }
                CmbVendedor.DataSource = vendedores;
                CmbVendedor.SelectedIndex = 0;

                List<OpcionFiltro> categorias = new List<OpcionFiltro>
                {
                    new OpcionFiltro { Id = 0, Valor = "", Nombre = "Todas" }
                };
                foreach (CategoriaDto categoria in filtros.Categorias)
                {
                    categorias.Add(new OpcionFiltro { Id = categoria.Id, Valor = categoria.Id.ToString(), Nombre = categoria.Nombre });
                }
                CmbCategoria.DataSource = categorias;
                CmbCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los filtros del dashboard: " + ex.Message,
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadDashboardAsync()
        {
            if (Cargando)
            {
                return;
            }

            if (DtpFin.Value.Date < DtpInicio.Value.Date)
            {
                MessageBox.Show("La fecha inicial no puede ser mayor que la fecha final.",
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cargando = true;
            MostrarCargando(true);

            try
            {
                FilterDto filtro = CrearFiltro();

                KpiCardsDto kpis = await NDashboard.ObtenerKpisAsync(filtro);
                UcKpis.Cargar(kpis);

                // Sprint 2: aqui se cargaran Tendencia, TopProductos y Distribucion
                // cuando sus widgets se implementen con LiveCharts2.

                LblUltimaActualizacion.Text = "Última actualización: " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el dashboard: " + ex.Message,
                    "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MostrarCargando(false);
                Cargando = false;
            }
        }

        // Construye el filtro aplicando el alcance por rol:
        // un usuario no Administrador siempre ve unicamente sus propias ventas.
        private FilterDto CrearFiltro()
        {
            int? idUsuario = null;

            if (Variables.Rol != "Administrador")
            {
                idUsuario = Variables.IdUsuario;
            }
            else
            {
                OpcionFiltro vendedor = CmbVendedor.SelectedItem as OpcionFiltro;
                if (vendedor != null && vendedor.Id != 0)
                {
                    idUsuario = vendedor.Id;
                }
            }

            int? idCategoria = null;
            OpcionFiltro categoria = CmbCategoria.SelectedItem as OpcionFiltro;
            if (categoria != null && categoria.Id != 0)
            {
                idCategoria = categoria.Id;
            }

            OpcionFiltro granularidad = CmbGranularidad.SelectedItem as OpcionFiltro;
            OpcionFiltro metrica = CmbMetrica.SelectedItem as OpcionFiltro;

            return new FilterDto(
                DtpInicio.Value.Date,
                DtpFin.Value.Date,
                idUsuario,
                idCategoria,
                granularidad != null ? granularidad.Valor : "DIA",
                metrica != null ? metrica.Valor : "MONTO");
        }

        private void MostrarCargando(bool cargando)
        {
            PrgKpis.Visible = cargando;
            PrgTendencia.Visible = cargando;
            PrgTopProductos.Visible = cargando;
            PrgDistribucion.Visible = cargando;
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            await LoadDashboardAsync();
        }

        private async void TimerAutoRefresh_Tick(object sender, EventArgs e)
        {
            await LoadDashboardAsync();
        }

        private void CmbAutoRefresh_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpcionFiltro opcion = CmbAutoRefresh.SelectedItem as OpcionFiltro;
            int minutos = opcion != null ? opcion.Id : 0;

            if (minutos <= 0)
            {
                TimerAutoRefresh.Stop();
            }
            else
            {
                TimerAutoRefresh.Interval = minutos * 60 * 1000;
                TimerAutoRefresh.Start();
            }
        }

        // ---------------- Constructores de controles ----------------

        private class OpcionFiltro
        {
            public int Id { get; set; }
            public string Valor { get; set; }
            public string Nombre { get; set; }

            public override string ToString()
            {
                return Nombre;
            }
        }

        private static DateTimePicker CrearFecha()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Century Gothic", 9F),
                Margin = new Padding(3)
            };
        }

        private static ComboBox CrearCombo()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Century Gothic", 9F),
                Margin = new Padding(3)
            };
        }

        private static ProgressBar CrearSkeleton()
        {
            return new ProgressBar
            {
                Dock = DockStyle.Bottom,
                Height = 8,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25,
                Visible = false
            };
        }

        // Etiqueta pequeña arriba + control debajo
        private static Control CrearCampo(string texto, Control control)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4, 2, 4, 2) };

            Label lbl = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 20,
                Text = texto,
                Font = new Font("Century Gothic", 7.5F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.BottomLeft
            };

            panel.Controls.Add(control);
            panel.Controls.Add(lbl);
            return panel;
        }

        // Panel que aloja un widget con su barra de carga (skeleton)
        private static Control CrearContenedorWidget(Control widget, ProgressBar skeleton)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(4) };
            widget.Margin = new Padding(0);
            panel.Controls.Add(widget);
            panel.Controls.Add(skeleton);
            return panel;
        }
    }
}
