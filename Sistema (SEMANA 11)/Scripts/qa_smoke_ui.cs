using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using Sistema.Entidad;
using Sistema.Negocios;
using WindowsFormsApp1;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Widgets;

namespace QaSmoke
{
    // Harness de QA: tests de integración (NEstadisticas + BD real) y smoke UI
    // (FrmEstadisticas / FrmDashBoard / widgets LiveCharts). Sin MessageBox.
    internal static class Program
    {
        private static StreamWriter _log;
        private static int _pass, _fail;

        [STAThread]
        private static int Main()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qa_smoke_out.txt");
            _log = new StreamWriter(ruta, false) { AutoFlush = true };
            Console.SetOut(new MultiWriter(Console.Out, _log));

            try
            {
                ParteIntegracion().GetAwaiter().GetResult();
                ParteUi();
            }
            catch (Exception ex)
            {
                Resultado(false, "HARNESS", "Excepción no controlada: " + ex);
            }

            Console.WriteLine();
            Console.WriteLine($"RESUMEN: PASS={_pass}  FAIL={_fail}");
            _log.Dispose();
            return _fail == 0 ? 0 : 1;
        }

        // ===================== INTEGRACIÓN: NEstadisticas + BD =====================
        private static async Task ParteIntegracion()
        {
            Console.WriteLine("======== PARTE 1: NEGOCIOS + BD ========");

            DateTime fin = DateTime.Today;
            DateTime inicio = fin.AddDays(-29);
            var filtro = new FilterDto(inicio, fin, null, null, "DIA", "MONTO");

            // T1 KPIs (verdad de terreno: 1 venta no anulada en la ventana = 59.00)
            try
            {
                KpiCardsDto k = await NEstadisticas.ObtenerKpisAsync(filtro);
                bool ok = Math.Abs(k.VentasTotales - 59m) < 0.01m
                       && k.NumTransacciones == 1
                       && Math.Abs(k.TicketPromedio - 59m) < 0.01m;
                Resultado(ok, "T1 KPIs",
                    $"VentasTotales={k.VentasTotales} (esperado 59.00), Transacciones={k.NumTransacciones} (esperado 1), Ticket={k.TicketPromedio} (esperado 59.00)");
            }
            catch (Exception ex) { Resultado(false, "T1 KPIs", "Excepción: " + Mensaje(ex)); }

            // T2 Tendencia: serie DIARIA completa para ventana de 30 días (30 puntos)
            try
            {
                TrendDto t = await NEstadisticas.ObtenerTendenciaAsync(filtro);
                bool ok = t.Puntos.Count == 30 && t.Puntos[0].PeriodoLabel.Length > 0;
                Resultado(ok, "T2 Tendencia", $"puntos={t.Puntos.Count} (esperado 30: serie diaria completa para la ventana de 30 días)");
            }
            catch (Exception ex) { Resultado(false, "T2 Tendencia", "Excepción: " + Mensaje(ex)); }

            // T2b Tendencia con granularidad SEMANA: debe agrupar por semana
            try
            {
                var fSem = new FilterDto(inicio, fin, null, null, "SEMANA", "MONTO");
                TrendDto t = await NEstadisticas.ObtenerTendenciaAsync(fSem);
                bool semanal = t.Puntos.Count <= 8;
                Resultado(semanal, "T2b Tendencia granularidad SEMANA",
                    $"puntos={t.Puntos.Count} (esperado <=8 semanales): sp_dashboard_tendencia solo acepta DIA/MES y hace fallback a DIA");
            }
            catch (Exception ex) { Resultado(false, "T2b Tendencia SEMANA", "Excepción: " + Mensaje(ex)); }

            // T3 Top productos (verdad de terreno: 1 producto en la ventana)
            try
            {
                TopProductsDto p = await NEstadisticas.ObtenerTopProductosAsync(filtro);
                bool ok = p.Productos.Count == 1
                       && p.Productos[0].Nombre == "Pollo Victorina"
                       && p.Productos[0].TotalCantidad == 1
                       && Math.Abs(p.Productos[0].TotalMonto - 50m) < 0.01m;
                Resultado(ok, "T3 TopProductos", $"n={p.Productos.Count}, primero={(p.Productos.Count > 0 ? p.Productos[0].Nombre + "/" + p.Productos[0].TotalMonto : "-")} (esperado Pollo Victorina/50.00)");
            }
            catch (Exception ex) { Resultado(false, "T3 TopProductos", "Excepción: " + Mensaje(ex)); }

            // T4 Distribución categorías
            try
            {
                CategoryDistributionDto d = await NEstadisticas.ObtenerDistribucionCategoriasAsync(filtro);
                decimal suma = 0; foreach (CategoryDto c in d.Categorias) suma += c.Porcentaje;
                bool ok = d.Categorias.Count == 1 && Math.Abs(suma - 100m) < 0.5m;
                Resultado(ok, "T4 DistribucionCategorias", $"categorias={d.Categorias.Count}, suma porcentajes={suma}");
            }
            catch (Exception ex) { Resultado(false, "T4 Distribucion", "Excepción: " + Mensaje(ex)); }

            // T5 Ventas periodo DIA (30 buckets diarios para la ventana)
            try
            {
                VentasPeriodoDto v = await NEstadisticas.ObtenerVentasPeriodoAsync(filtro);
                bool ok = v.Puntos.Count == 30 && v.Metrica == "MONTO" && v.Granularidad == "DIA";
                Resultado(ok, "T5 VentasPeriodo DIA", $"puntos={v.Puntos.Count} (esperado 30), metrica={v.Metrica}, granularidad={v.Granularidad}");
            }
            catch (Exception ex) { Resultado(false, "T5 VentasPeriodo", "Excepción: " + Mensaje(ex)); }

            // T5b Ventas periodo SEMANA + CANTIDAD (≈6 buckets semanales en 30 días)
            try
            {
                var fSem = new FilterDto(inicio, fin, null, null, "SEMANA", "CANTIDAD");
                VentasPeriodoDto v = await NEstadisticas.ObtenerVentasPeriodoAsync(fSem);
                bool coherente = true;
                foreach (VentasPeriodoPointDto p in v.Puntos)
                    if (p.Valor != p.Cantidad) { coherente = false; break; }
                bool ok = v.Puntos.Count >= 5 && v.Puntos.Count <= 6 && coherente && v.Metrica == "CANTIDAD";
                Resultado(ok, "T5b VentasPeriodo SEMANA/CANTIDAD", $"puntos={v.Puntos.Count} (esperado 5-6 semanales), Valor==Cantidad: {coherente}");
            }
            catch (Exception ex) { Resultado(false, "T5b VentasPeriodo SEMANA", "Excepción: " + Mensaje(ex)); }

            // T6 Stock bajo (parámetro corregido: @StockMinimoUmbral)
            try
            {
                StockBajoDto s = await NEstadisticas.ObtenerStockBajoAsync(new StockFilterDto(5, null));
                bool ok = s.Articulos.Count == 1
                       && s.Articulos[0].Nombre == "Pollo Victorina"
                       && s.Articulos[0].Estado == "Critico";
                Resultado(ok, "T6 StockBajo",
                    $"articulos={s.Articulos.Count} (esperado 1: Pollo Victorina/Critico con umbral 5)");
            }
            catch (Exception ex) { Resultado(false, "T6 StockBajo", "Excepción: " + Mensaje(ex)); }

            // T7 Filtros disponibles
            try
            {
                FiltrosDisponiblesDto f = await NEstadisticas.ObtenerFiltrosAsync(null);
                bool ok = f.Vendedores.Count == 3 && f.Categorias.Count == 6;
                Resultado(ok, "T7 FiltrosDisponibles", $"vendedores={f.Vendedores.Count}, categorias={f.Categorias.Count}");
            }
            catch (Exception ex) { Resultado(false, "T7 Filtros", "Excepción: " + Mensaje(ex)); }

            // T7b Filtros con idUsuario
            try
            {
                List<VendedorDto> v = await NEstadisticas.ObtenerVendedoresAsync(6);
                Resultado(v.Count == 1 && v[0].Id == 6, "T7b Filtro vendedor por id", $"vendedores={v.Count}, id={(v.Count > 0 ? v[0].Id.ToString() : "-")}");
            }
            catch (Exception ex) { Resultado(false, "T7b Filtro idUsuario", "Excepción: " + Mensaje(ex)); }

            // T8 Validaciones de filtro
            try
            {
                bool nullLanza = false;
                try { await NEstadisticas.ObtenerKpisAsync(null); }
                catch (ArgumentNullException) { nullLanza = true; }

                var invertido = new FilterDto(fin, inicio, null, null, "NO_VALIDA", "ZZZ");
                VentasPeriodoDto v = await NEstadisticas.ObtenerVentasPeriodoAsync(invertido);
                bool normalizado = v.Granularidad == "DIA" && v.Metrica == "MONTO";

                bool ok = nullLanza && normalizado && v.Puntos.Count > 0;
                Resultado(ok, "T8 Validación filtros",
                    $"null lanza ArgumentNullException={nullLanza}; fechas invertidas+granularidad/métrica inválidas -> DIA/MONTO={normalizado}");
            }
            catch (Exception ex) { Resultado(false, "T8 Validación filtros", "Excepción: " + Mensaje(ex)); }
        }

        // ===================== SMOKE UI (sin diálogos) =====================
        private static void ParteUi()
        {
            Console.WriteLine();
            Console.WriteLine("======== PARTE 2: UI / WIDGETS ========");

            // T9 FrmEstadisticas se instancia
            FrmEstadisticas frm = null;
            try
            {
                frm = new FrmEstadisticas();
                Resultado(true, "T9 FrmEstadisticas ctor", "Se instanció sin excepción");
            }
            catch (Exception ex) { Resultado(false, "T9 FrmEstadisticas ctor", "Excepción: " + Mensaje(ex)); }

            if (frm != null)
            {
                // T10 6 pestañas
                try
                {
                    TabControl tc = Campo<TabControl>(frm, "TabControl");
                    string[] esperadas = { "KPIs", "Tendencia", "Top Productos", "Distribución", "Ventas Período", "Stock Bajo" };
                    bool ok = tc != null && tc.TabPages.Count == 6;
                    if (ok)
                        for (int i = 0; i < 6; i++)
                            if (tc.TabPages[i].Text != esperadas[i]) ok = false;
                    Resultado(ok, "T10 TabControl 6 pestañas", tc == null ? "TabControl nulo" : "nombres=" + Nombres(tc));
                }
                catch (Exception ex) { Resultado(false, "T10 Pestañas", "Excepción: " + Mensaje(ex)); }

                // T11 Barra de filtros
                try
                {
                    string[] campos = { "DtpInicio", "DtpFin", "CmbPreset", "CmbCategoria", "CmbVendedor", "CmbGranularidad", "CmbMetrica", "BtnActualizar", "BtnExportar", "LblEstado" };
                    List<string> faltan = new List<string>();
                    foreach (string c in campos)
                        if (Campo<object>(frm, c) == null) faltan.Add(c);
                    Resultado(faltan.Count == 0, "T11 Barra de filtros", faltan.Count == 0 ? "10 controles presentes" : "faltan: " + string.Join(",", faltan));
                }
                catch (Exception ex) { Resultado(false, "T11 Barra filtros", "Excepción: " + Mensaje(ex)); }

                // T12 Combos con opciones (DataSource: Items.Count es 0 sin BindingContext)
                try
                {
                    ComboBox preset = Campo<ComboBox>(frm, "CmbPreset");
                    ComboBox gran = Campo<ComboBox>(frm, "CmbGranularidad");
                    ComboBox met = Campo<ComboBox>(frm, "CmbMetrica");
                    int nPreset = ContarOpciones(preset), nGran = ContarOpciones(gran), nMet = ContarOpciones(met);
                    bool ok = nPreset == 5 && nGran == 4 && nMet == 2;
                    Resultado(ok, "T12 Opciones de combos", $"preset={nPreset}/5, granularidad={nGran}/4, metrica={nMet}/2");
                }
                catch (Exception ex) { Resultado(false, "T12 Combos", "Excepción: " + Mensaje(ex)); }

                // T13 ProgressBars marquee, ocultas, una por pestaña
                try
                {
                    string[] prgs = { "PrgKpis", "PrgTendencia", "PrgTopProductos", "PrgDistribucion", "PrgVentasPeriodo", "PrgStockBajo" };
                    bool ok = true; List<string> detalle = new List<string>();
                    foreach (string p in prgs)
                    {
                        ProgressBar b = Campo<ProgressBar>(frm, p);
                        if (b == null) { ok = false; detalle.Add(p + "=NULL"); continue; }
                        if (b.Style != ProgressBarStyle.Marquee || b.Visible) { ok = false; detalle.Add(p + " style/visible"); }
                        if (b.Parent == null || !(b.Parent is TabPage)) { ok = false; detalle.Add(p + " sin TabPage"); }
                    }
                    Resultado(ok, "T13 ProgressBars por pestaña", ok ? "6 marquee, visibles=false, dentro de cada TabPage" : string.Join("; ", detalle));
                }
                catch (Exception ex) { Resultado(false, "T13 ProgressBars", "Excepción: " + Mensaje(ex)); }

                // T14 Widgets por pestaña
                try
                {
                    string[] widgets = { "UcKpis", "UcTendencia", "UcTopProductos", "UcDistribucion", "UcVentasPeriodo", "UcStockBajo" };
                    List<string> faltan = new List<string>();
                    foreach (string w in widgets) if (Campo<object>(frm, w) == null) faltan.Add(w);
                    Resultado(faltan.Count == 0, "T14 Widgets por pestaña", faltan.Count == 0 ? "6 widgets presentes" : "faltan: " + string.Join(",", faltan));
                }
                catch (Exception ex) { Resultado(false, "T14 Widgets", "Excepción: " + Mensaje(ex)); }

                // T15 Botones con manejadores suscritos (via EventHandlerList)
                try
                {
                    Button act = Campo<Button>(frm, "BtnActualizar");
                    Button exp = Campo<Button>(frm, "BtnExportar");
                    int nAct = ContarSuscriptores(act, "EventClick");
                    int nExp = ContarSuscriptores(exp, "EventClick");
                    Resultado(nAct >= 1 && nExp >= 1, "T15 Manejadores Click",
                        $"BtnActualizar handlers={nAct} (>=1), BtnExportar handlers={nExp} (>=1)");
                }
                catch (Exception ex) { Resultado(false, "T15 Manejadores Click", "Excepción: " + Mensaje(ex)); }
            }

            // T16 Render LiveCharts: construcción + Cargar con datos
            SmokeChart("T16a UcTrendChart", () =>
            {
                var uc = new UcTrendChart();
                var f = new FilterDto(DateTime.Today.AddDays(-29), DateTime.Today, null, null, "DIA", "MONTO");
                var dto = new TrendDto(new List<TrendPointDto> {
                    new TrendPointDto(DateTime.Today, "hoy", 100m, 50m),
                    new TrendPointDto(DateTime.Today.AddDays(1), "mañana", 120m, 60m) });
                uc.Cargar(dto, f);
            });

            SmokeChart("T16b UcVentasPeriodo", () =>
            {
                var uc = new UcVentasPeriodo();
                var f = new FilterDto(DateTime.Today.AddDays(-29), DateTime.Today, null, null, "DIA", "MONTO");
                var dto = new VentasPeriodoDto("DIA", "MONTO", new List<VentasPeriodoPointDto> {
                    new VentasPeriodoPointDto(DateTime.Today, "hoy", 100m, 2, 100m) });
                uc.Cargar(dto, f);
            });

            SmokeChart("T16c UcCategoryDonut", () =>
            {
                var uc = new UcCategoryDonut();
                var dto = new CategoryDistributionDto(new List<CategoryDto> {
                    new CategoryDto(1, "Cat A", 300m, 3, 60m), new CategoryDto(2, "Cat B", 200m, 2, 40m) });
                uc.Cargar(dto);
            });

            SmokeChart("T16d UcTopProducts", () =>
            {
                var uc = new UcTopProducts();
                var dto = new TopProductsDto(new List<TopProductDto> {
                    new TopProductDto(1, "A1", "Prod A", "Cat", 10, 100m) });
                uc.Cargar(dto);
            });

            SmokeChart("T16e UcKpiCardsEstadisticas", () =>
            {
                var uc = new UcKpiCardsEstadisticas();
                uc.Cargar(new KpiCardsDto(559m, 543m, 279.5m, 2, 12.5m, 33m));
            });

            // T17 Grid de alertas: coloreado de 'Critico'
            try
            {
                var uc = new UcAlertasStock();
                var dto = new StockBajoDto(new List<StockBajoItemDto> {
                    new StockBajoItemDto(1, "X1", "Producto critico", "Cat", 0, 5, "Critico"),
                    new StockBajoItemDto(2, "X2", "Producto bajo", "Cat", 3, 5, "Bajo") });
                uc.Cargar(dto);
                DataGridView grid = Campo<DataGridView>(uc, "Grid");
                Color c0 = grid.Rows[0].DefaultCellStyle.BackColor;
                Color c1 = grid.Rows[1].DefaultCellStyle.BackColor;
                bool criticoRojo = c0.R > c0.B;   // rojo claro (255,235,235) vs verde claro (235,255,235)
                bool bajoAmarillo = c1.R > c1.B && c1.G > c1.B;
                Resultado(criticoRojo && bajoAmarillo, "T17 Coloreado alertas (Estado='Critico'/'Bajo')",
                    $"Critico={c0.ToArgb().ToString("X8")} (esperado rojo claro FFEBEBEB), Bajo={c1.ToArgb().ToString("X8")} (esperado amarillo claro) -> fila crítica pintada como {(criticoRojo ? "CRÍTICA" : "OK/VERDE (bug)")}");
            }
            catch (Exception ex) { Resultado(false, "T17 Coloreado alertas", "Excepción: " + Mensaje(ex)); }

            // T18 Exportación PNG (¿real o placeholder?)
            try
            {
                string rutaPng = Path.Combine(Path.GetTempPath(), "qa_export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
                var f = new FilterDto(DateTime.Today.AddDays(-29), DateTime.Today, null, null, "DIA", "MONTO");
                var uc = new UcVentasPeriodo();
                uc.Cargar(new VentasPeriodoDto("DIA", "MONTO", new List<VentasPeriodoPointDto> {
                    new VentasPeriodoPointDto(DateTime.Today, "hoy", 100m, 2, 100m) }), f);

                CartesianChart chart = Campo<CartesianChart>(uc, "_chart");
                ChartExportHelper.ExportToPng(chart, rutaPng);
                byte[] bytes = File.ReadAllBytes(rutaPng);
                bool png = bytes.Length > 24 && bytes[0] == 0x89 && bytes[1] == 0x50;
                int ancho = png ? (bytes[16] << 24 | bytes[17] << 16 | bytes[18] << 8 | bytes[19]) : 0;
                int alto = png ? (bytes[20] << 24 | bytes[21] << 16 | bytes[22] << 8 | bytes[23]) : 0;
                bool placeholder = ancho == 800 && alto == 600;
                Resultado(png && !placeholder, "T18 ExportToPng",
                    $"PNG válido={png}, dimensiones={ancho}x{alto}" +
                    (placeholder ? " -> PLACEHOLDER 800x600: no renderiza el gráfico (bug RF-EXP-01)" : ""));
                File.Delete(rutaPng);
            }
            catch (Exception ex) { Resultado(false, "T18 ExportToPng", "Excepción: " + Mensaje(ex)); }

            // T19 Permisos en FrmDashBoard
            foreach (string rol in new[] { "Administrador", "Vendedor", "Almacen" })
            {
                try
                {
                    Variables.Rol = rol;
                    var db = new FrmDashBoard();
                    MethodInfo m = typeof(FrmDashBoard).GetMethod("AplicarPermisos", BindingFlags.NonPublic | BindingFlags.Instance);
                    m.Invoke(db, null);
                    Button btn = Campo<Button>(db, "BtnEstadisticas");
                    bool esperado = rol == "Administrador";
                    Panel panel = Campo<Panel>(db, "panel1");
                    int indice = panel.Controls.GetChildIndex(btn);
                    ImageList il = Campo<ImageList>(db, "imageList1");
                    bool iconoOk = il != null && il.Images.ContainsKey("estadisticas.png");
                    bool visiblePropio = VisiblePropio(btn);
                    Resultado(visiblePropio == esperado && iconoOk, "T19 Permisos rol=" + rol,
                        $"BtnEstadisticas.Visible propio={visiblePropio} (esperado {esperado}), indiceZOrder={indice} (diseñado=3), icono='estadisticas.png' presente={iconoOk}");
                }
                catch (Exception ex) { Resultado(false, "T19 Permisos rol=" + rol, "Excepción: " + Mensaje(ex)); }
            }

            // T20 Orden visual del botón Estadísticas en el menú
            try
            {
                var db = new FrmDashBoard();
                Panel panel = Campo<Panel>(db, "panel1");
                Button btn = Campo<Button>(db, "BtnEstadisticas");
                int idx = panel.Controls.GetChildIndex(btn);
                string orden = "";
                for (int i = panel.Controls.Count - 1; i >= 0; i--)
                {
                    Control c = panel.Controls[i];
                    if (c is Button) orden += c.Text + (i == idx ? "*" : "") + " > ";
                }
                bool ok = idx == 3;
                Resultado(ok, "T20 Posición botón Estadísticas",
                    $"indice={idx} (esperado 3 => entre USUARIOS y VENTAS). Orden visual: {orden.TrimEnd(' ', '>')}");
            }
            catch (Exception ex) { Resultado(false, "T20 Posición botón", "Excepción: " + Mensaje(ex)); }

            // T21 FrmDashBoard abre FrmEstadisticas (reflejando FrmDashBoard_Load)
            try
            {
                Variables.Rol = "Administrador";
                var db = new FrmDashBoard();
                Panel pnl = Campo<Panel>(db, "PnlContenedor");
                MethodInfo load = typeof(FrmDashBoard).GetMethod("FrmDashBoard_Load", BindingFlags.NonPublic | BindingFlags.Instance);
                // FrmDashBoard_Load muestra un MessageBox de bienvenida: se sustituye el handler
                // invocando solo la parte de navegación mediante AbrirFormulario.
                MethodInfo abrir = typeof(FrmDashBoard).GetMethod("AbrirFormulario", BindingFlags.NonPublic | BindingFlags.Instance);
                abrir.Invoke(db, new object[] { new FrmEstadisticas() });
                bool ok = pnl.Controls.Count == 1 && pnl.Controls[0] is FrmEstadisticas;
                Resultado(ok, "T21 Navegación a FrmEstadisticas", $"PnlContenedor contiene FrmEstadisticas={ok} (hijos={pnl.Controls.Count})");
            }
            catch (Exception ex) { Resultado(false, "T21 Navegación", "Excepción: " + Mensaje(ex)); }

            Variables.Rol = null;
        }

        // ===================== utilidades =====================
        private static void SmokeChart(string nombre, Action accion)
        {
            try { accion(); Resultado(true, nombre, "Construct + Cargar sin excepción"); }
            catch (Exception ex)
            {
                string m = Mensaje(ex);
                string extra = m.Contains("SkiaSharp") ? " <- INCOMPATIBILIDAD LiveCharts 2.0.5 (SkiaSharp 2.88) vs SkiaSharp 3.119" : "";
                Resultado(false, nombre, "Excepción: " + m + extra);
            }
        }

        private static T Campo<T>(object obj, string nombre) where T : class
        {
            Type t = obj.GetType();
            while (t != null)
            {
                FieldInfo f = t.GetField(nombre, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (f != null) return f.GetValue(obj) as T;
                t = t.BaseType;
            }
            return null;
        }

        private static int ContarSuscriptores(object control, string evento)
        {
            try
            {
                PropertyInfo pi = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                object lista = pi.GetValue(control, null);
                if (lista == null) return -1;
                FieldInfo clave = typeof(Control).GetField(evento, BindingFlags.NonPublic | BindingFlags.Static);
                if (clave == null) return -2;
                object key = clave.GetValue(null);
                object d = lista.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance)
                                   .GetValue(lista, new object[] { key });
                Delegate del = d as Delegate;
                return del == null ? 0 : del.GetInvocationList().Length;
            }
            catch { return -3; }
        }

        // Visibilidad propia del control (STATE_VISIBLE=0x2 sin heredar de padres;
        // el form nunca se muestra en el harness, Visible efectivo sería siempre false)
        private static bool VisiblePropio(Control c)
        {
            MethodInfo gs = typeof(Control).GetMethod("GetState", BindingFlags.NonPublic | BindingFlags.Instance);
            if (gs != null) return (bool)gs.Invoke(c, new object[] { 0x2 });
            return c.Visible;
        }

        private static int ContarOpciones(ComboBox cb)
        {
            System.Collections.IList lista = cb.DataSource as System.Collections.IList;
            return lista != null ? lista.Count : cb.Items.Count;
        }

        private static string Nombres(TabControl tc)
        {
            List<string> l = new List<string>();
            foreach (TabPage p in tc.TabPages) l.Add(p.Text);
            return string.Join(", ", l);
        }

        private static string Mensaje(Exception ex)
        {
            Exception e = ex;
            while (e.InnerException != null) e = e.InnerException;
            return e.GetType().Name + ": " + e.Message;
        }

        private static void Resultado(bool ok, string test, string detalle)
        {
            if (ok) _pass++; else _fail++;
            Console.WriteLine((ok ? "[PASS] " : "[FAIL] ") + test + " | " + detalle);
        }

        private class MultiWriter : TextWriter
        {
            private readonly TextWriter[] _w;
            public MultiWriter(params TextWriter[] w) { _w = w; }
            public override System.Text.Encoding Encoding { get { return System.Text.Encoding.UTF8; } }
            public override void Write(char value) { foreach (TextWriter t in _w) t.Write(value); }
            public override void WriteLine(string value) { foreach (TextWriter t in _w) t.WriteLine(value); }
        }
    }
}
