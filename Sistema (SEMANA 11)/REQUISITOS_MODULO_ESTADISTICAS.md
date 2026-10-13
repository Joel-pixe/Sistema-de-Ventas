# Documento de Requisitos — Módulo Estadísticas (Dashboard)

**Versión:** 1.0  
**Fecha:** 2026-10-11  
**Fase:** SDLC Fase 1 — Análisis de Requisitos  
**Autor:** Analista de Requisitos  
**Estado:** Listo para revisión y validación  

---

## 1. Resumen Ejecutivo

El presente documento define los requisitos formales para la implementación de un **módulo de Estadísticas** accesible desde el Dashboard principal (`FrmDashBoard`) del sistema WinForms "Sistema SEMANA 11" (ventas/inventario). El módulo proporcionará visualizaciones interactivas basadas en datos de **Ventas**, **Ingresos**, **Artículos**, **Clientes** y **Proveedores**, con filtros dinámicos y capacidad de exportación a imagen (PNG/PDF). El acceso estará restringido al rol **Administrador**. La integración se realizará añadiendo un botón "ESTADÍSTICAS" en el menú lateral (`PnlMenuVertical`) y cargando la vista por defecto en el contenedor principal (`PnlContenedor`) al abrir el Dashboard.

**Tecnología base detectada:**  
- .NET Framework 4.8 (WinForms)  
- Arquitectura 3 capas: `Sistema.Entidad` → `Sistema.Negocios` → `Sistema.Datos` → SQL Server (stored procedures)  
- **SkiaSharp 2.88.8** ya referenciado en el proyecto de presentación  
- **LiveChartsCore 2.0.5 + LiveChartsCore.SkiaSharpView.WinForms** disponibles en carpeta `packages/` (pendiente de referenciar)  

---

## 2. Historias de Usuario

| ID | Historia de Usuario | Prioridad |
|----|---------------------|-----------|
| HU-01 | **Como Administrador**, quiero acceder a un panel de Estadísticas desde el menú lateral del Dashboard para analizar el desempeño del negocio. | Must |
| HU-02 | **Como Administrador**, quiero ver la evolución de ventas en un gráfico de líneas/barras con selector de rango de fechas (día, semana, mes, año) para identificar tendencias. | Must |
| HU-03 | **Como Administrador**, quiero ver el ranking de productos más vendidos (por cantidad e ingresos) para tomar decisiones de stock y promociones. | Must |
| HU-04 | **Como Administrador**, quiero ver la distribución de ventas por categoría en gráfico de pastel/barras para entender el mix de productos. | Must |
| HU-05 | **Como Administrador**, quiero comparar Ingresos (compras) vs Ventas en un mismo período para evaluar márgenes y rotación. | Must |
| HU-06 | **Como Administrador**, quiero ver el ranking de clientes top por volumen de compras para estrategias de fidelización. | Should |
| HU-07 | **Como Administrador**, quiero visualizar alertas de stock bajo/crítico para gestionar reposiciones a tiempo. | Should |
| HU-08 | **Como Administrador**, quiero filtrar todos los gráficos por categoría, vendedor y cliente/proveedor para análisis segmentados. | Must |
| HU-09 | **Como Administrador**, quiero exportar los gráficos a PNG/PDF para incluirlos en reportes gerenciales. | Should |
| HU-10 | **Como Administrador**, quiero que la vista de Estadísticas cargue automáticamente al abrir el Dashboard para acceso inmediato. | Could |

---

## 3. Requisitos Funcionales

### 3.1 Navegación y Acceso

| ID | Requisito | Prioridad | Criterio de Aceptación |
|----|-----------|-----------|------------------------|
| RF-NAV-01 | Añadir botón **"ESTADÍSTICAS"** en `PnlMenuVertical` (menú lateral) con icono correspondiente, entre "USUARIOS" y "SALIR". | Must | Botón visible, con estilo consistente (Flat, color DodgerBlue, fuente Century Gothic 10pt bold, icono + texto alineado). Click abre `FrmEstadisticas` en `PnlContenedor`. |
| RF-NAV-02 | Cargar `FrmEstadisticas` automáticamente en `PnlContenedor` al ejecutar `FrmDashBoard_Load` (vista por defecto). | Must | Al iniciar sesión como Administrador, el contenedor muestra el módulo Estadísticas sin requerir click. |
| RF-NAV-03 | Aplicar permisos: botón y carga automática **solo visibles/habilitados para rol "Administrador"**. | Must | En `AplicarPermisos()`: si `Rol != "Administrador"` → `BtnEstadisticas.Visible = false` y no se carga auto. |
| RF-NAV-04 | Mantener patrón `AbrirFormulario(object frmHijo)` para instanciar `FrmEstadisticas` como formulario hijo no top-level, `Dock = Fill`. | Must | Formulario se muestra dentro del panel sin bordes, ocupa todo `PnlContenedor`. |

### 3.2 Formulario Principal — `FrmEstadisticas`

| ID | Requisito | Prioridad | Criterio de Aceptación |
|----|-----------|-----------|------------------------|
| RF-FRM-01 | Crear formulario `FrmEstadisticas` (hereda `Form`) con diseño responsive (`Dock = Fill`). | Must | Compila sin errores; se instancia desde `FrmDashBoard.AbrirFormulario`. |
| RF-FRM-02 | Estructura UI: **Barra superior** (título "Estadísticas", selector de rango de fechas, botón "Actualizar", botón "Exportar") + **Área de pestañas** (`TabControl`) con una pestaña por gráfico principal. | Must | Controles presentes, anclados correctamente (`Anchor`/`Dock`). |
| RF-FRM-03 | Selector de rango de fechas: `DateTimePicker` inicio/fin + `ComboBox` presets ("Hoy", "Esta semana", "Este mes", "Este año", "Personalizado"). Cambio de preset actualiza los pickers. | Must | Presets funcionan; rango personalizado editable. Evento `ValueChanged` dispara recarga de datos. |
| RF-FRM-04 | Filtros adicionales (acordeón/panel lateral colapsable): `ComboBox` Categoría, `ComboBox` Vendedor, `ComboBox` Cliente/Proveedor. Opción "Todos" por defecto. | Must | Combos cargan datos de BD al iniciar; selección filtra todos los gráficos simultáneamente. |
| RF-FRM-05 | Botón **"Actualizar"** ejecuta recarga completa de todos los gráficos aplicando filtros actuales. | Must | Click → `async Task CargarTodo()` → todos los gráficos reflejan filtros. |
| RF-FRM-06 | Botón **"Exportar"** abre `SaveFileDialog` (filtro PNG/PDF), renderiza la pestaña activa a imagen y guarda. | Should | Archivo generado válido, abre en visor estándar. |

### 3.3 Gráficos — Fuente de Datos y Consultas

> **Nota:** Todos los datos provienen de stored procedures existentes (`venta_listar`, `venta_buscar`, `ingreso_listar`, `articulo_listar`, `persona_listar_clientes`, `persona_listar_proveedores`) que retornan `DataTable`. Se añadirán **nuevos stored procedures agregados** en capa `Sistema.Datos` para rendimiento (evitar traer DataTables completos al cliente).

| ID | Gráfico / Pestaña | Datos Requeridos | SP Nuevo Sugerido | Prioridad | Criterio de Aceptación |
|----|-------------------|------------------|-------------------|-----------|------------------------|
| RF-GRF-01 | **Ventas por Período** (Línea/Barras) | Series temporales: fecha, total_ventas, cantidad_ventas | `venta_estadisticas_por_periodo @fecha_inicio, @fecha_fin, @idcategoria, @idvendedor, @idcliente` | Must | Gráfico renderiza serie temporal; tooltip muestra fecha, total, cantidad. Cambio de filtro actualiza en <2s. |
| RF-GRF-02 | **Top Productos Vendidos** (Barras horizontales Top 10) | Artículo, cantidad_vendida, total_ingresos | `venta_top_productos @fecha_inicio, @fecha_fin, @top_n, @idcategoria, @idvendedor` | Must | Top 10 por defecto; orden descendente por cantidad; tooltip muestra artículo, cant., ingresos. |
| RF-GRF-03 | **Ventas por Categoría** (Pastel / Dona) | Categoría, total_ventas, porcentaje | `venta_por_categoria @fecha_inicio, @fecha_fin, @idvendedor` | Must | Suma de porcentajes = 100%; click en segmento filtra otros gráficos (drill-down opcional). |
| RF-GRF-04 | **Ingresos vs Ventas** (Barras agrupadas o Línea dual) | Período, total_ingresos, total_ventas, margen | `estadisticas_ingresos_vs_ventas @fecha_inicio, @fecha_fin, @idcategoria, @idproveedor` | Must | Dos series comparables; leyenda clara; margen calculado = (ventas - ingresos) / ventas * 100. |
| RF-GRF-05 | **Clientes Top** (Barras horizontales Top 10) | Cliente, cantidad_compras, total_comprado | `venta_top_clientes @fecha_inicio, @fecha_fin, @top_n, @idvendedor` | Should | Top 10; orden por total_comprado desc. |
| RF-GRF-06 | **Alertas de Stock** (Tabla + indicadores visuales) | Artículo, stock_actual, stock_minimo, estado (OK/Bajo/Crítico) | `articulo_stock_alertas @stock_minimo_umbral` | Should | Filas coloreadas: Verde OK, Amarillo Bajo (< umbral), Rojo Crítico (0). Ordenadas por criticidad. |

### 3.4 Interactividad y UX

| ID | Requisito | Prioridad | Criterio de Aceptación |
|----|-----------|-----------|------------------------|
| RF-UX-01 | Tooltip en todos los gráficos: muestra valores exactos al pasar mouse (fecha, monto, cantidad, %). | Must | LiveCharts `TooltipPosition` configurado; formato moneda "C2" (RD$), números con separadores de miles. |
| RF-UX-02 | Leyenda clickeable: ocultar/mostrar series en gráficos multi-serie (Ingresos vs Ventas). | Should | Click en leyenda alterna `Series[i].IsVisible`. |
| RF-UX-03 | Zoom/pan en gráfico de series temporales (Ventas por Período). | Could | `ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X` habilitado. |
| RF-UX-04 | Indicador de carga (`ProgressBar` estilo marquee o `PictureBox` spinner) mientras se consultan datos. | Must | Visible durante `CargarTodo()`; oculto al completar o error. |
| RF-UX-05 | Manejo de errores: `try/catch` en cada carga individual; `MessageBox` amigable + log en `Output`/`Debug`. | Must | Fallo en un gráfico no bloquea los demás; mensaje "Error al cargar [Gráfico]: {msg}". |
| RF-UX-06 | Persistencia de último rango de fechas y filtros en `Settings` (user-scoped) para restaurar al reabrir. | Could | Al cerrar/abrir Dashboard, filtros se mantienen. |

### 3.5 Exportación

| ID | Requisito | Prioridad | Criterio de Aceptación |
|----|-----------|-----------|------------------------|
| RF-EXP-01 | Exportar **gráfico de pestaña activa** a **PNG** (300 DPI) usando `SKBitmap` + `SKCanvas` (SkiaSharp). | Should | Archivo PNG válido, dimensiones = tamaño real del control gráfico. |
| RF-EXP-02 | Exportar **gráfico de pestaña activa** a **PDF** (una página, gráfico centrado, título + fecha de generación + filtros aplicados). | Should | Usa `SkiaSharp.Pdf` o `PdfSharp` (nueva dependencia); PDF abre en Adobe Reader/Edge. |
| RF-EXP-03 | Nombre de archivo sugerido: `Estadisticas_{Pestaña}_{yyyyMMdd_HHmmss}.{png|pdf}`. | Should | `SaveFileDialog.FileName` prellenado con patrón. |

---

## 4. Requisitos No Funcionales

| ID | Categoría | Requisito | Métrica / Verificación |
|----|-----------|-----------|------------------------|
| RNF-01 | **Rendimiento** | Carga inicial de todos los gráficos (sin filtros restrictivos) ≤ 3 segundos en red LAN típica (1-5 ms latency a SQL Server). | Test manual con cronómetro; `Stopwatch` en `CargarTodo()`. |
| RNF-02 | **Rendimiento** | Recarga tras cambio de filtro ≤ 2 segundos. | Ídem. |
| RNF-03 | **Rendimiento** | Consultas agregadas en BD (SPs) devuelven ≤ 500 filas cada una (Top 10, series mensuales ~12 puntos, categorías ~20). | Revisión de planes de ejecución SQL; `SET STATISTICS TIME ON`. |
| RNF-04 | **Memoria** | Uso de memoria del proceso no aumenta > 50 MB tras 10 recargas consecutivas. | Perfilador (dotMemory / VS Diagnostic Tools). |
| RNF-05 | **Seguridad** | Acceso solo para rol "Administrador" (validado en UI y en capa Negocios). | Prueba con usuario "Vendedor" → botón invisible, formulario no instanciable. |
| RNF-06 | **Seguridad** | Parámetros de SPs validados (SqlParameter, no concatenación). | Revisión de código `DVenta`, `DIngresos`, nuevo `DEstadisticas`. |
| RNF-07 | **Usabilidad** | Resolución mínima soportada: 1366×768 (Dashboard actual 1583×840). Gráficos legibles sin scroll horizontal. | Test en VM/resolución reducida. |
| RNF-08 | **Mantenibilidad** | Código separado en capa `Sistema.Negocios.NEstadisticas` + `Sistema.Datos.DEstadisticas` (nuevos archivos). | Estructura de carpetas respeta arquitectura existente. |
| RNF-09 | **Compatibilidad** | .NET Framework 4.8, Windows 10/11, SQL Server Express 2019+. | Matriz de compatibilidad del proyecto. |
| RNF-10 | **Localización** | Textos en español (RD$ formato moneda, fechas dd/MM/yyyy). | `CultureInfo("es-DO")` o `es-ES` en formato. |

---

## 5. Alcance

### 5.1 Incluye (In Scope)

- Nuevo formulario `FrmEstadisticas` en `WindowsFormsApp1`.
- Nuevas clases `DEstadisticas` (capac Datos) y `NEstadisticas` (capa Negocios).
- 6 stored procedures nuevos en BD (`venta_estadisticas_por_periodo`, `venta_top_productos`, `venta_por_categoria`, `estadisticas_ingresos_vs_ventas`, `venta_top_clientes`, `articulo_stock_alertas`).
- Referencia a **LiveChartsCore.SkiaSharpView.WinForms** (ya en `packages/`, agregar al `.csproj`).
- Botón "ESTADÍSTICAS" en menú lateral + carga automática en `FrmDashBoard`.
- Lógica de permisos integrada en `AplicarPermisos()`.
- Exportación PNG/PDF del gráfico activo.
- Persistencia opcional de filtros en `Settings.settings`.

### 5.2 Excluye (Out of Scope)

- Exportación a Excel/CSV de datos tabulares (solo imagen/PDF del gráfico).
- Drill-down navegante (click en segmento → abre `FrmVenta` filtrado) — *fase futura*.
- Gráficos en tiempo real / SignalR / polling automático.
- Dashboards personalizables por usuario (widgets arrastrables).
- Notificaciones/alertas push por stock crítico.
- Multi-idioma (solo español).
- Versión web / móvil.

---

## 6. Supuestos y Dependencias

| ID | Supuesto / Dependencia | Impacto si Falso |
|----|------------------------|------------------|
| SUP-01 | SQL Server Express accesible en `localhost\SQLEXPRESS`, BD `mydatabase2` con permisos `sa` / Integrated Security. | Requiere cambio en `Conexion.cs` y scripts de despliegue. |
| SUP-02 | Stored procedures existentes (`venta_listar`, `ingreso_listar`, etc.) devuelven columnas consistentes con entidades actuales. | Nuevos SPs deben replicar nomenclatura de columnas. |
| SUP-03 | Paquetes NuGet `LiveChartsCore.SkiaSharpView.WinForms` 2.0.5 + dependencias (`LiveChartsCore`, `SkiaSharp`) compatibles con .NET 4.8. | Si no, evaluar `LiveCharts.Wpf` (v0) o `ScottPlot.WinForms` como alternativa. |
| SUP-04 | SkiaSharp 2.88.8 ya referenciado funciona correctamente en WinForms (render a `PictureBox` o control nativo). | Validar con prototipo rápido (`SKGLControl` o `SKPictureBox`). |
| SUP-05 | El equipo de desarrollo tiene permisos para crear SPs en la BD de desarrollo/producción. | Si no, lógica de agregación moverse a capa Negocios (LINQ sobre DataTable) — menor rendimiento. |
| SUP-06 | No hay conflictos de versión entre `System.Drawing.Common` 4.5.1 y SkiaSharp. | Test de compilación y ejecución en limpio. |

---

## 7. Riesgos Identificados

| ID | Riesgo | Probabilidad | Impacto | Mitigación |
|----|--------|--------------|---------|------------|
| RSK-01 | LiveChartsCore 2.x (beta) tiene API inestable / breaking changes en futuras actualizaciones. | Media | Alto | Fijar versión 2.0.5 en `packages.config`; no actualizar sin prueba completa. Documentar wrappers propios. |
| RSK-02 | Rendimiento de consultas agregadas en BD con grandes volúmenes (>100k ventas). | Media | Medio | Índices en `Venta.Fecha`, `Venta.IdCategoria`, `Venta.IdUsuario`; paginación/server-side en SPs. |
| RSK-03 | SkiaSharp renderizado en WinForms requiere `SKGLControl` (OpenGL) o `SKPictureBox` (CPU) — posible flicker en Remote Desktop. | Baja | Medio | Usar `DoubleBuffered = true` en contenedor; probar en RDP. |
| RSK-04 | Exportación PDF requiere librería adicional (`PdfSharp` o `SkiaSharp.Pdf`) — aumenta superficie de dependencias. | Media | Bajo | Evaluar `SkiaSharp.Pdf` (nativo, sin dependencia extra) como primera opción. |
| RSK-05 | Cambios en esquema de BD (nuevas columnas en Venta/Ingreso) rompen SPs de estadísticas. | Baja | Alto | Tests de integración automatizados; versionado de scripts SQL. |
| RSK-06 | Permisos de rol "Administrador" hardcoded en `Variables.Rol` — si cambia nomenclatura, falla silenciosamente. | Baja | Medio | Centralizar roles en `enum` o tabla `Rol` y validar por `IdRol` no por string. |

---

## 8. Cambios Técnicos Requeridos (Resumen para Arquitecto)

### 8.1 Nuevos Archivos / Clases

| Capa | Archivo | Responsabilidad |
|------|---------|-----------------|
| `Sistema.Datos` | `DEstadisticas.cs` | 6 métodos `async Task<DataTable>` invocando SPs nuevos; `SqlParameter` tipados. |
| `Sistema.Negocios` | `NEstadisticas.cs` | 6 métodos `static async Task<DataTable>` wrappeando `DEstadisticas`; validación básica de parámetros. |
| `WindowsFormsApp1` | `FrmEstadisticas.cs` + `.Designer.cs` + `.resx` | UI completa: barra filtros, `TabControl` con 6 pestañas, controles LiveCharts, exportación. |
| `WindowsFormsApp1` | `FrmDashBoard.cs` (modificar) | Añadir `BtnEstadisticas` en designer + lógica en `AplicarPermisos()` + carga auto en `Load`. |
| `WindowsFormsApp1` | `Resources.resx` (modificar) | Añadir icono "estadisticas.png" al `ImageList`. |

### 8.2 Base de Datos (Scripts SQL)

```sql
-- 1. Ventas por período (serie temporal)
CREATE PROCEDURE venta_estadisticas_por_periodo
    @fecha_inicio DATE, @fecha_fin DATE,
    @idcategoria INT = NULL, @idvendedor INT = NULL, @idcliente INT = NULL
AS
SELECT 
    CAST(v.Fecha AS DATE) AS Fecha,
    SUM(v.Total) AS TotalVentas,
    COUNT(*) AS CantidadVentas
FROM Venta v
LEFT JOIN Detalle_Venta dv ON v.IdVenta = dv.IdVenta
LEFT JOIN Articulo a ON dv.IdArticulo = a.IdArticulo
WHERE v.Fecha BETWEEN @fecha_inicio AND @fecha_fin
  AND v.Estado = 'Aceptado'
  AND (@idcategoria IS NULL OR a.IdCategoria = @idcategoria)
  AND (@idvendedor IS NULL OR v.IdUsuario = @idvendedor)
  AND (@idcliente IS NULL OR v.IdCliente = @idcliente)
GROUP BY CAST(v.Fecha AS DATE)
ORDER BY Fecha;

-- 2. Top productos
CREATE PROCEDURE venta_top_productos
    @fecha_inicio DATE, @fecha_fin DATE,
    @top_n INT = 10, @idcategoria INT = NULL, @idvendedor INT = NULL
AS
SELECT TOP (@top_n)
    a.Nombre AS Articulo,
    SUM(dv.Cantidad) AS CantidadVendida,
    SUM(dv.Importe) AS TotalIngresos
FROM Detalle_Venta dv
JOIN Venta v ON dv.IdVenta = v.IdVenta
JOIN Articulo a ON dv.IdArticulo = a.IdArticulo
WHERE v.Fecha BETWEEN @fecha_inicio AND @fecha_fin
  AND v.Estado = 'Aceptado'
  AND (@idcategoria IS NULL OR a.IdCategoria = @idcategoria)
  AND (@idvendedor IS NULL OR v.IdUsuario = @idvendedor)
GROUP BY a.Nombre
ORDER BY CantidadVendida DESC;

-- 3. Ventas por categoría
CREATE PROCEDURE venta_por_categoria
    @fecha_inicio DATE, @fecha_fin DATE, @idvendedor INT = NULL
AS
SELECT 
    c.Nombre AS Categoria,
    SUM(v.Total) AS TotalVentas,
    CAST(SUM(v.Total) * 100.0 / SUM(SUM(v.Total)) OVER() AS DECIMAL(10,2)) AS Porcentaje
FROM Venta v
JOIN Detalle_Venta dv ON v.IdVenta = dv.IdVenta
JOIN Articulo a ON dv.IdArticulo = a.IdArticulo
JOIN Categoria c ON a.IdCategoria = c.IdCategoria
WHERE v.Fecha BETWEEN @fecha_inicio AND @fecha_fin
  AND v.Estado = 'Aceptado'
  AND (@idvendedor IS NULL OR v.IdUsuario = @idvendedor)
GROUP BY c.Nombre
ORDER BY TotalVentas DESC;

-- 4. Ingresos vs Ventas
CREATE PROCEDURE estadisticas_ingresos_vs_ventas
    @fecha_inicio DATE, @fecha_fin DATE,
    @idcategoria INT = NULL, @idproveedor INT = NULL
AS
SELECT 
    FORMAT(v.Fecha, 'yyyy-MM') AS Periodo,
    SUM(v.Total) AS TotalVentas,
    ISNULL((SELECT SUM(i.Total) FROM Ingreso i 
            WHERE i.Fecha BETWEEN @fecha_inicio AND @fecha_fin
              AND i.Estado = 'Aceptado'
              AND (@idproveedor IS NULL OR i.IdProveedor = @idproveedor)), 0) AS TotalIngresos
FROM Venta v
LEFT JOIN Detalle_Venta dv ON v.IdVenta = dv.IdVenta
LEFT JOIN Articulo a ON dv.IdArticulo = a.IdArticulo
WHERE v.Fecha BETWEEN @fecha_inicio AND @fecha_fin
  AND v.Estado = 'Aceptado'
  AND (@idcategoria IS NULL OR a.IdCategoria = @idcategoria)
GROUP BY FORMAT(v.Fecha, 'yyyy-MM')
ORDER BY Periodo;

-- 5. Clientes top
CREATE PROCEDURE venta_top_clientes
    @fecha_inicio DATE, @fecha_fin DATE, @top_n INT = 10, @idvendedor INT = NULL
AS
SELECT TOP (@top_n)
    p.Nombre AS Cliente,
    COUNT(v.IdVenta) AS CantidadCompras,
    SUM(v.Total) AS TotalComprado
FROM Venta v
JOIN Persona p ON v.IdCliente = p.IdPersona
WHERE v.Fecha BETWEEN @fecha_inicio AND @fecha_fin
  AND v.Estado = 'Aceptado'
  AND (@idvendedor IS NULL OR v.IdUsuario = @idvendedor)
GROUP BY p.Nombre
ORDER BY TotalComprado DESC;

-- 6. Alertas de stock
CREATE PROCEDURE articulo_stock_alertas
    @stock_minimo_umbral INT = 5
AS
SELECT 
    a.Nombre AS Articulo,
    a.Stock,
    @stock_minimo_umbral AS StockMinimo,
    CASE 
        WHEN a.Stock = 0 THEN 'Crítico'
        WHEN a.Stock <= @stock_minimo_umbral THEN 'Bajo'
        ELSE 'OK'
    END AS Estado
FROM Articulo a
WHERE a.Estado = 1
ORDER BY 
    CASE WHEN a.Stock = 0 THEN 0 WHEN a.Stock <= @stock_minimo_umbral THEN 1 ELSE 2 END,
    a.Stock ASC;
```

### 8.3 Referencias NuGet a Agregar (WindowsFormsApp1.csproj)

```xml
<PackageReference Include="LiveChartsCore" Version="2.0.5" />
<PackageReference Include="LiveChartsCore.SkiaSharpView" Version="2.0.5" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WinForms" Version="2.0.5" />
<!-- SkiaSharp 2.88.8 ya existe -->
<!-- Opcional para PDF: -->
<!-- <PackageReference Include="SkiaSharp.Pdf" Version="2.88.8" /> -->
```

### 8.4 Modificaciones en Archivos Existentes

| Archivo | Cambio |
|---------|--------|
| `FrmDashBoard.Designer.cs` | Añadir `BtnEstadisticas` (Button) al `panel1` con `Dock=Top`, `ImageKey` nuevo, evento `Click += BtnEstadisticas_Click`. |
| `FrmDashBoard.cs` | En `AplicarPermisos()`: `BtnEstadisticas.Visible = (Rol == "Administrador");` En `FrmDashBoard_Load`: si Admin → `AbrirFormulario(new FrmEstadisticas());` |
| `Variables.cs` | (Opcional) Añadir `public static int IdCategoriaFiltro;` etc. si se requiere estado global. |
| `Settings.settings` | (Opcional) Añadir `UltimoRangoInicio`, `UltimoRangoFin`, `UltimaCategoria`, `UltimoVendedor`, `UltimoCliente` (user-scoped). |

---

## 9. Criterios de Aceptación Globales (Definition of Done)

1. **Compilación**: `Build Solution` → 0 errores, 0 warnings nuevos.
2. **Ejecución**: Login como Administrador → Dashboard abre → `FrmEstadisticas` visible en contenedor + botón en menú lateral.
3. **Permisos**: Login como Vendedor/Almacén → botón no visible, contenedor no carga Estadísticas.
4. **Datos**: Cada pestaña muestra gráfico con datos reales de BD (mínimo 1 registro de prueba por entidad).
5. **Filtros**: Cambio de rango de fechas / categoría / vendedor / cliente → click "Actualizar" → todos los gráficos refrescan con datos filtrados.
6. **Exportación**: Click "Exportar" → `SaveFileDialog` → PNG/PDF generado válido.
7. **Rendimiento**: Carga completa < 3s en máquina de desarrollo (DB local).
8. **Sin regresiones**: Formularios existentes (Ventas, Ingresos, Artículos, Clientes, etc.) siguen funcionando igual.

---

## 10. Recomendación para Fase 2 (Arquitecto)

> **Estado: ✅ LISTO PARA FASE 2 — DISEÑO ARQUITECTÓNICO**

El analista recomienda proceder a la **Fase 2 (Arquitecto)** con los siguientes entregables esperados:
1. **Diagrama de componentes** actualizado (nuevas clases `DEstadisticas`, `NEstadisticas`, `FrmEstadisticas`).
2. **Diseño de SPs** — revisión y optimización de los 6 SPs propuestos (índices, planes de ejecución).
3. **Selección definitiva de librería gráfica**: Confirmar LiveChartsCore 2.x + SkiaSharp vs alternativa (ScottPlot, Microsoft Chart Control nativo). Prototipo de 1 gráfico para validar render/export.
4. **Patrón de carga asíncrona**: Definir `async/await` + `CancellationToken` + `Progress<T>` para UX fluida.
5. **Estrategia de testing**: Unit tests para `NEstadisticas` (mock `DEstadisticas`), integration tests para SPs, UI tests smoke para `FrmEstadisticas`.

---

## 11. Anexos

### A. Diccionario de Datos Relevantes (Subconjunto)

| Tabla / Vista | Columnas Clave para Estadísticas |
|---------------|----------------------------------|
| `Venta` | IdVenta, IdCliente, IdUsuario, Fecha, Total, Impuesto, Estado, TipoComprobante |
| `Detalle_Venta` | IdVenta, IdArticulo, Cantidad, Precio, Descuento, Importe |
| `Ingreso` | IdIngreso, IdProveedor, IdUsuario, Fecha, Total, Impuesto, Estado |
| `Detalle_Ingreso` | IdIngreso, IdArticulo, Cantidad, Precio, Importe |
| `Articulo` | IdArticulo, IdCategoria, Nombre, PrecioVenta, Stock, Estado |
| `Categoria` | IdCategoria, Nombre, Estado |
| `Persona` | IdPersona, TipoPersona (Cliente/Proveedor), Nombre |

### B. Mapeo Rol → Permisos (Actual + Nuevo)

| Funcionalidad | Administrador | Vendedor | Almacén |
|---------------|---------------|----------|---------|
| Ver Dashboard | ✅ | ✅ | ✅ |
| Ventas | ✅ | ✅ | ❌ |
| Ingresos | ✅ | ✅ | ✅ |
| Artículos | ✅ | ❌ | ✅ |
| Categorías | ✅ | ❌ | ✅ |
| Clientes | ✅ | ✅ | ❌ |
| Proveedores | ✅ | ❌ | ✅ |
| Usuarios | ✅ | ❌ | ❌ |
| **Estadísticas (NUEVO)** | ✅ | ❌ | ❌ |

---

**Fin del Documento**  
*Este documento debe ser validado por el Product Owner / Stakeholder antes de iniciar la Fase 2.*