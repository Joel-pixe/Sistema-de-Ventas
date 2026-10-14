using LiveChartsCore.SkiaSharpView.WinForms;
using Sistema.Entidad;
using Sistema.Negocios;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Widgets;
using WindowsFormsApp1.Helpers;

namespace WindowsFormsApp1
{
    public partial class FrmEstadisticas : Form
    {
        #region Controles UI
        // Barra superior
        private DateTimePicker DtpInicio;
        private DateTimePicker DtpFin;
        private ComboBox CmbPreset;
        private ComboBox CmbCategoria;
        private ComboBox CmbVendedor;
        private ComboBox CmbGranularidad;
        private ComboBox CmbMetrica;
        private Button BtnActualizar;
        private Button BtnExportar;
        private Label LblEstado;

        // TabControl con 6 pestañas
        private TabControl TabControl;
        private TabPage TabKpis;
        private TabPage TabTendencia;
        private TabPage TabTopProductos;
        private TabPage TabDistribucion;
        private TabPage TabVentasPeriodo;
        private TabPage TabStockBajo;

        // Widgets
        private UcKpiCardsEstadisticas UcKpis;
        private UcTrendChart UcTendencia;
        private UcTopProducts UcTopProductos;
        private UcCategoryDonut UcDistribucion;
        private UcVentasPeriodo UcVentasPeriodo;
        private UcAlertasStock UcStockBajo;

        // Progress bars
        private ProgressBar PrgKpis;
        private ProgressBar PrgTendencia;
        private ProgressBar PrgTopProductos;
        private ProgressBar PrgDistribucion;
        private ProgressBar PrgVentasPeriodo;
        private ProgressBar PrgStockBajo;

        private bool _cargando;
        #endregion

        public FrmEstadisticas()
        {
            BuildUI();
            Load += FrmEstadisticas_Load;
        }

        private void BuildUI()
        {
            Text = "Estadísticas";
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.WhiteSmoke;
            Dock = DockStyle.Fill;

            // ===== BARRA SUPERIOR DE FILTROS =====
            Panel pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.White,
                Padding = new Padding(10, 8, 10, 8)
            };

            TableLayoutPanel tlp = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 10,
                RowCount = 1,
                Margin = new Padding(0)
            };
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            float[] widths = { 10, 10, 10, 11, 11, 11, 11, 9, 9, 8 };
            foreach (float w in widths) tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, w));

            DtpInicio = CrearFecha();
            DtpFin = CrearFecha();
            CmbPreset = CrearCombo();
            CmbCategoria = CrearCombo();
            CmbVendedor = CrearCombo();
            CmbGranularidad = CrearCombo();
            CmbMetrica = CrearCombo();

            DtpInicio.Value = DateTime.Today.AddDays(-29);
            DtpFin.Value = DateTime.Today;

            CmbPreset.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 0, Valor = "", Nombre = "Personalizado" },
                new OpcionFiltro { Id = 1, Valor = "HOY", Nombre = "Hoy" },
                new OpcionFiltro { Id = 2, Valor = "SEMANA", Nombre = "Esta semana" },
                new OpcionFiltro { Id = 3, Valor = "MES", Nombre = "Este mes" },
                new OpcionFiltro { Id = 4, Valor = "ANO", Nombre = "Este año" }
            };
            CmbPreset.SelectedIndexChanged += CmbPreset_SelectedIndexChanged;

            CmbGranularidad.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 1, Valor = "DIA", Nombre = "Día" },
                new OpcionFiltro { Id = 2, Valor = "SEMANA", Nombre = "Semana" },
                new OpcionFiltro { Id = 3, Valor = "MES", Nombre = "Mes" },
                new OpcionFiltro { Id = 4, Valor = "ANIO", Nombre = "Año" }
            };
            CmbMetrica.DataSource = new List<OpcionFiltro>
            {
                new OpcionFiltro { Id = 1, Valor = "MONTO", Nombre = "Monto" },
                new OpcionFiltro { Id = 2, Valor = "CANTIDAD", Nombre = "Cantidad" }
            };

            tlp.Controls.Add(CrearCampo("DESDE", DtpInicio), 0, 0);
            tlp.Controls.Add(CrearCampo("HASTA", DtpFin), 1, 0);
            tlp.Controls.Add(CrearCampo("PERÍODO", CmbPreset), 2, 0);
            tlp.Controls.Add(CrearCampo("CATEGORÍA", CmbCategoria), 3, 0);
            tlp.Controls.Add(CrearCampo("VENDEDOR", CmbVendedor), 4, 0);
            tlp.Controls.Add(CrearCampo("GRANULARIDAD", CmbGranularidad), 7, 0);
            tlp.Controls.Add(CrearCampo("MÉTRICA", CmbMetrica), 8, 0);

            // Panel Acciones
            Panel pnlAcciones = new Panel { Dock = DockStyle.Fill };

            TableLayoutPanel tlpAcciones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            tlpAcciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpAcciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));  // ACTUALIZAR
            tlpAcciones.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));  // EXPORTAR
            tlpAcciones.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // estado

            BtnActualizar = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                Text = "ACTUALIZAR",
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnActualizar.FlatAppearance.BorderSize = 0;
            BtnActualizar.Click += async (s, e) => await CargarTodoAsync();

            BtnExportar = new Button
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 2, 0, 2),
                Text = "EXPORTAR",
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Century Gothic", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            BtnExportar.FlatAppearance.BorderSize = 0;
            BtnExportar.Click += BtnExportar_Click;

            LblEstado = new Label
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                Text = "Listo",
                Font = new Font("Century Gothic", 7.5F),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft
            };

            tlpAcciones.Controls.Add(BtnActualizar, 0, 0);
            tlpAcciones.Controls.Add(BtnExportar, 0, 1);
            tlpAcciones.Controls.Add(LblEstado, 0, 2);
            pnlAcciones.Controls.Add(tlpAcciones);

            Panel celdaAcciones = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 2, 4, 2),
                Padding = new Padding(4, 0, 0, 0)
            };
            celdaAcciones.Controls.Add(pnlAcciones);
            tlp.Controls.Add(celdaAcciones, 9, 0);

            pnlTop.Controls.Add(tlp);

            // ===== TABCONTROL =====
            TabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Century Gothic", 9F),
                Padding = new Point(8, 6)
            };

            // --- Pestaña 1: KPIs ---
            TabKpis = new TabPage("KPIs");
            UcKpis = new UcKpiCardsEstadisticas { Dock = DockStyle.Fill };
            PrgKpis = CrearSkeleton(TabKpis);
            TabKpis.Controls.Add(UcKpis);
            TabKpis.Controls.Add(PrgKpis);

            // --- Pestaña 2: Tendencia ---
            TabTendencia = new TabPage("Tendencia");
            UcTendencia = new UcTrendChart { Dock = DockStyle.Fill };
            PrgTendencia = CrearSkeleton(TabTendencia);
            TabTendencia.Controls.Add(UcTendencia);
            TabTendencia.Controls.Add(PrgTendencia);

            // --- Pestaña 3: Top Productos ---
            TabTopProductos = new TabPage("Top Productos");
            UcTopProductos = new UcTopProducts { Dock = DockStyle.Fill };
            PrgTopProductos = CrearSkeleton(TabTopProductos);
            TabTopProductos.Controls.Add(UcTopProductos);
            TabTopProductos.Controls.Add(PrgTopProductos);

            // --- Pestaña 4: Distribución ---
            TabDistribucion = new TabPage("Distribución");
            UcDistribucion = new UcCategoryDonut { Dock = DockStyle.Fill };
            PrgDistribucion = CrearSkeleton(TabDistribucion);
            TabDistribucion.Controls.Add(UcDistribucion);
            TabDistribucion.Controls.Add(PrgDistribucion);

            // --- Pestaña 5: Ventas Período ---
            TabVentasPeriodo = new TabPage("Ventas Período");
            UcVentasPeriodo = new UcVentasPeriodo { Dock = DockStyle.Fill };
            PrgVentasPeriodo = CrearSkeleton(TabVentasPeriodo);
            TabVentasPeriodo.Controls.Add(UcVentasPeriodo);
            TabVentasPeriodo.Controls.Add(PrgVentasPeriodo);

            // --- Pestaña 6: Stock Bajo ---
            TabStockBajo = new TabPage("Stock Bajo");
            UcStockBajo = new UcAlertasStock { Dock = DockStyle.Fill };
            PrgStockBajo = CrearSkeleton(TabStockBajo);
            TabStockBajo.Controls.Add(UcStockBajo);
            TabStockBajo.Controls.Add(PrgStockBajo);

            TabControl.TabPages.AddRange(new TabPage[] {
                TabKpis, TabTendencia, TabTopProductos, TabDistribucion, TabVentasPeriodo, TabStockBajo
            });

            Controls.Add(TabControl);
            Controls.Add(pnlTop);
        }

        private async void FrmEstadisticas_Load(object sender, EventArgs e)
        {
            await CargarFiltrosAsync();
            await CargarTodoAsync();
        }

        private async Task CargarFiltrosAsync()
        {
            try
            {
                int? idUsuarioFiltro = Variables.Rol != "Administrador" ? Variables.IdUsuario : (int?)null;
                FiltrosDisponiblesDto filtros = await NEstadisticas.ObtenerFiltrosAsync(idUsuarioFiltro);

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
                MessageBox.Show("Error al cargar los filtros: " + ex.Message, "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarTodoAsync()
        {
            if (_cargando) return;

            FilterDto filtro = CrearFiltro();
            if (filtro == null) return;

            _cargando = true;
            MostrarCargandoGlobal(true);
            BtnActualizar.Enabled = false;
            BtnActualizar.Text = "Cargando...";
            LblEstado.Text = "Cargando estadísticas...";

            try
            {
                var tasks = new[]
                {
                    CargarKpisAsync(filtro),
                    CargarTendenciaAsync(filtro),
                    CargarTopProductosAsync(filtro),
                    CargarDistribucionAsync(filtro),
                    CargarVentasPeriodoAsync(filtro),
                    CargarStockBajoAsync(filtro)
                };

                await Task.WhenAll(tasks);

                LblEstado.Text = $"Última actualización: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error: " + ex.Message;
                MessageBox.Show("Error al cargar estadísticas: " + ex.Message, "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MostrarCargandoGlobal(false);
                BtnActualizar.Enabled = true;
                BtnActualizar.Text = "ACTUALIZAR";
                _cargando = false;
            }
        }

        private async Task CargarKpisAsync(FilterDto filtro)
        {
            try
            {
                PrgKpis.Visible = true;
                KpiCardsDto dto = await NEstadisticas.ObtenerKpisAsync(filtro);
                UcKpis.Cargar(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[KPIs] " + ex.Message);
            }
            finally { PrgKpis.Visible = false; }
        }

        private async Task CargarTendenciaAsync(FilterDto filtro)
        {
            try
            {
                PrgTendencia.Visible = true;
                TrendDto dto = await NEstadisticas.ObtenerTendenciaAsync(filtro);
                UcTendencia.Cargar(dto, filtro);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Tendencia] " + ex.Message);
            }
            finally { PrgTendencia.Visible = false; }
        }

        private async Task CargarTopProductosAsync(FilterDto filtro)
        {
            try
            {
                PrgTopProductos.Visible = true;
                TopProductsDto dto = await NEstadisticas.ObtenerTopProductosAsync(filtro);
                UcTopProductos.Cargar(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[TopProductos] " + ex.Message);
            }
            finally { PrgTopProductos.Visible = false; }
        }

        private async Task CargarDistribucionAsync(FilterDto filtro)
        {
            try
            {
                PrgDistribucion.Visible = true;
                CategoryDistributionDto dto = await NEstadisticas.ObtenerDistribucionCategoriasAsync(filtro);
                UcDistribucion.Cargar(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[Distribucion] " + ex.Message);
            }
            finally { PrgDistribucion.Visible = false; }
        }

        private async Task CargarVentasPeriodoAsync(FilterDto filtro)
        {
            try
            {
                PrgVentasPeriodo.Visible = true;
                VentasPeriodoDto dto = await NEstadisticas.ObtenerVentasPeriodoAsync(filtro);
                UcVentasPeriodo.Cargar(dto, filtro);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[VentasPeriodo] " + ex.Message);
            }
            finally { PrgVentasPeriodo.Visible = false; }
        }

        private async Task CargarStockBajoAsync(FilterDto filtro)
        {
            try
            {
                PrgStockBajo.Visible = true;
                int? idCat = filtro.IdCategoria.HasValue && filtro.IdCategoria.Value > 0 ? filtro.IdCategoria : null;
                StockFilterDto stockFilter = new StockFilterDto(5, idCat);
                StockBajoDto dto = await NEstadisticas.ObtenerStockBajoAsync(stockFilter);
                UcStockBajo.Cargar(dto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[StockBajo] " + ex.Message);
            }
            finally { PrgStockBajo.Visible = false; }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            TabPage activeTab = TabControl.SelectedTab;
            if (activeTab == null) return;

            try
            {
                if (activeTab == TabVentasPeriodo)
                {
                    using (SaveFileDialog sfd = ChartExportHelper.CreateSaveDialog("VentasPeriodo", ".png"))
                    {
                        if (sfd.ShowDialog(this) == DialogResult.OK)
                        {
                            MessageBox.Show("Exportación disponible en la pestaña Ventas Período (botón en toolbar)", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else if (activeTab == TabTendencia)
                {
                    MessageBox.Show("Exportación disponible próximamente para esta pestaña.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Exportación disponible solo para gráficos de series temporales.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpcionFiltro opcion = CmbPreset.SelectedItem as OpcionFiltro;
            if (opcion == null) return;

            DateTime hoy = DateTime.Today;
            switch (opcion.Valor)
            {
                case "HOY":
                    DtpInicio.Value = hoy;
                    DtpFin.Value = hoy;
                    break;
                case "SEMANA":
                    DtpInicio.Value = hoy.AddDays(-(int)hoy.DayOfWeek);
                    DtpFin.Value = hoy;
                    break;
                case "MES":
                    DtpInicio.Value = new DateTime(hoy.Year, hoy.Month, 1);
                    DtpFin.Value = hoy;
                    break;
                case "ANO":
                    DtpInicio.Value = new DateTime(hoy.Year, 1, 1);
                    DtpFin.Value = hoy;
                    break;
                default:
                    break; // Personalizado: no cambia las fechas
            }
        }

        private FilterDto CrearFiltro()
        {
            if (DtpFin.Value.Date < DtpInicio.Value.Date)
            {
                MessageBox.Show("La fecha inicial no puede ser mayor que la fecha final.", "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

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

        private void MostrarCargandoGlobal(bool cargando)
        {
            PrgKpis.Visible = cargando;
            PrgTendencia.Visible = cargando;
            PrgTopProductos.Visible = cargando;
            PrgDistribucion.Visible = cargando;
            PrgVentasPeriodo.Visible = cargando;
            PrgStockBajo.Visible = cargando;
        }

        // ---------------- Constructores de controles ----------------

        private class OpcionFiltro
        {
            public int Id { get; set; }
            public string Valor { get; set; }
            public string Nombre { get; set; }

            public override string ToString() => Nombre;
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

        private static ProgressBar CrearSkeleton(Control parent)
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
    }
}