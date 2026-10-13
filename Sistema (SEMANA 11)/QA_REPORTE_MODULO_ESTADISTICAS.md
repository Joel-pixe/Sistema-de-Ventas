# QA — Reporte de Pruebas: Módulo Estadísticas

**Proyecto:** Sistema SEMANA 11 (WinForms, .NET Framework 4.8, 3 capas)
**Módulo:** Estadísticas (`FrmEstadisticas` + `NEstadisticas`/`DEstadisticas` + widgets LiveCharts)
**Fecha:** 12/10/2026 · **Ingeniero QA:** agente de Testing · **Requisitos base:** `REQUISITOS_MODULO_ESTADISTICAS.md`

---

## 1. Resumen ejecutivo

| Indicador | Resultado |
|---|---|
| Tests automatizados ejecutados | **30** (11 integración negocio+BD, 19 UI/gráficos/permisos) |
| Resultado final | **✅ 28 PASS / ❌ 2 FAIL** |
| Los 2 FAIL | Bugs reales abiertos (exportación PNG y granularidad SEMANA en Tendencia) |
| Bugs encontrados en total | **12** (6 corregidos y validados, 6 abiertos/deuda) |
| Compilación | ✅ 4 proyectos: **0 errores**, 4 warnings CS0169 preexistentes (ajenos al módulo) |
| SPs / índices en BD real | ✅ 7/7 SPs creados · 6/6 índices creados y habilitados |
| Arranque real de la app | ✅ Abre ventana `FrmLogin` (antes: crash instantáneo) |
| Conexión a BD real | ✅ `Conexion` opera contra `mydatabase2` (antes: fallo SSL total) |
| **Gate de calidad** | ⚠️ **APROBACIÓN CONDICIONADA**: cerrar ABIERTO-1 (RF-EXP-01) y ABIERTO-2 (granularidad SEMANA) |

> Hallazgo crítico de esta ronda: el módulo **no podía probarse en absoluto** porque tres bugs globales impedían arrancar la app, conectar a SQL Server y construir el `FrmDashBoard`. Los6 bugs corregidos (secc. 5) fueron validados re-ejecutando toda la suite.

---

## 2. Entorno y método

**Entorno**

- Windows · Visual Studio 18 (MSBuild `Current\Bin`, Roslyn `csc`) · .NET Framework 4.8
- SQL Server 2025 Express `localhost\SQLEXPRESS`, BD `mydatabase2`, collation `Modern_Spanish_CI_AS`
- LiveChartsCore 2.0.5 + SkiaSharp 3.119 (binding redirect activo) · Microsoft.Data.SqlClient 5.1.9
- Datos de la ventana de prueba (hoy−29 … hoy = 13/09/2026–12/10/2026): **1 venta** (id 14, RD$59.00 con ITBIS / RD$50.00 neto, Pollo Victorina ×1), ingresos no anulados RD$243.00, 1 artículo con stock 0 (Crítico), 3 vendedores, 6 categorías.

**Método (pirámide de pruebas)**

1. **Build gate**: rebuild de `Sistema.Entidad`, `Sistema.Datos`, `Sistema.Negocios`, `Sistema.Presentacion` → 0 errores.
2. **Integración BD (sqlcmd)**: existencia y shape de los 7 SPs, 6 índices, verdad de terreno (`qa_integracion_estadisticas.sql`, `qa_verdad_terreno.sql`, `qa_confirmacion_bug.sql`).
3. **Unit/negocio + UI smoke (harness C# `qa_smoke_ui.cs`)**:30 tests vía reflexión sobre `NEstadisticas` y los formularios/widgets reales; STA, sin `MessageBox`, log incremental.
4. **Sondas puntuales (`qa_probe.cs`)**: cadena de conexión original vs corregida, clase `Conexion`, ctors de `FrmLogin`/`FrmDashBoard`, ícono del ImageList.
5. **Estáticos**: auditoría de código fuente, scan de desajustes `SetKeyName` vs `ImageStream` (`qa_imglist_tool.cs scan`), revisión de `.csproj`/`App.config`.
6. **E2E mínimo**: lanzamiento real del `WindowsFormsApp1.exe` compilado → ventana `FrmLogin` visible.

---

## 3. Resultados — Tabla de tests

### 3.1 Parte 1 — Negocio `NEstadisticas` + integración con BD real (11)

| # | Test | Resultado | Detalle |
|---|---|---|---|
| T1 | KPIs (ventas, transacciones, ticket) | ✅ | 59.00 / 1 / 59.00 = verdad cruda SQL y `sp_dashboard_kpis` |
| T2 | Tendencia serie diaria | ✅ | 30 puntos completos para ventana de 30 días, labels no vacíos |
| T2b | Tendencia granularidad **SEMANA** | ❌ | Devuelve **30 puntos DIARIOS** (labels `13/Sep…12/Oct`); el SP hace fallback a DIA → **ABIERTO-2** |
| T3 | Top productos | ✅ | 1 producto: Pollo Victorina, 1 ud, RD$50.00 |
| T4 | Distribución por categorías | ✅ | 1 categoría, Σ porcentajes = 100.00 |
| T5 | Ventas Período DIA/MONTO | ✅ | 30 buckets, métrica y granularidad correctas |
| T5b | Ventas Período SEMANA/CANTIDAD | ✅ | 6 buckets semanales, Valor==Cantidad (el SP de ventas periodo **sí** agrupa por semana) |
| T6 | Stock bajo (umbral 5) | ✅ | 1 artículo Pollo Victorina / `Critico` (tras fix de parámetro) |
| T7 | Filtros disponibles | ✅ | 3 vendedores, 6 categorías |
| T7b | Filtro vendedor por `idUsuario` | ✅ | id 6 → 1 vendedor |
| T8 | Validaciones de filtro | ✅ | `null` → `ArgumentNullException`; fechas invertidas + granularidad/métrica inválidas → normalizadas a DIA/MONTO |

### 3.2 Parte 2 — UI, gráficos y permisos (19)

| # | Test | Resultado | Detalle |
|---|---|---|---|
| T9 | `FrmEstadisticas` ctor | ✅ | Instancia sin excepción |
| T10 | `TabControl` con 6 pestañas | ✅ | KPIs, Tendencia, Top Productos, Distribución, Ventas Período, Stock Bajo |
| T11 | Barra de filtros | ✅ | 10 controles (DtpInicio/DtpFin, 5 combos, 2 botones, LblEstado) |
| T12 | Opciones de combos | ✅ | preset 5/5, granularidad 4/4, métrica 2/2 |
| T13 | ProgressBars | ✅ | 6 marquee, ocultas, una por pestaña |
| T14 | Widgets por pestaña | ✅ | 6 widgets presentes |
| T15 | Manejadores Click | ✅ | ACTUALIZAR y EXPORTAR con handler suscrito |
| T16a–e | Render LiveCharts (5 widgets) | ✅ | `UcTrendChart`, `UcVentasPeriodo`, `UcCategoryDonut`, `UcTopProducts`, `UcKpiCardsEstadisticas`: construct + `Cargar` sin excepción (sin incompatibilidad SkiaSharp en runtime) |
| T17 | Coloreado de alertas de stock | ✅ | `Critico`=FFFFEBEB (rojo), `Bajo`=FFFFF8DC (amarillo) — tras fix |
| T18 | Exportación PNG | ❌ | PNG válido pero **800×600 placeholder** con texto fijo, no renderiza el gráfico → **ABIERTO-1** |
| T19 | Permisos rol Administrador | ✅ | Botón visible, índice 3, ícono `estadisticas.png` presente |
| T19 | Permisos rol Vendedor | ✅ | Botón oculto |
| T19 | Permisos rol Almacén | ✅ | Botón oculto |
| T20 | Posición del botón en el menú | ✅ | Índice z-order 3 (tras USUARIOS, antes de VENTAS → "entre USUARIOS y SALIR", RF-NAV-01) |
| T21 | Navegación Dashboard → Estadísticas | ✅ | `PnlContenedor` contiene exactamente `FrmEstadisticas` |

### 3.3 Evidencia complementaria

| Verificación | Resultado |
|---|---|
| Rebuild MSBuild (4 proyectos) | ✅ 0 errores · ⚠ 4 warnings CS0169 preexistentes (`FrmIngresos`, `FrmVenta`) |
| 7 SPs del módulo en BD | ✅ existen y sus columnas coinciden 100% con el mapeo de `NEstadisticas` |
| 6 índices (`estadisticas_*`) | ✅ creados y `is_disabled = 0` |
| Sonda cadena de conexión **original** (`Server=…; Integrated Security=SSPI`) | ❌ `SqlException: certificate chain was issued by an authority that not trusted` (bug BUG-02) |
| Sonda con `TrustServerCertificate=True` | ✅ conecta (server 17.00.1000) |
| Sonda clase `Conexion` (producción, tras fix) | ✅ conecta a `mydatabase2` |
| Sonda `new FrmLogin()` | ✅ (recurso preserializado cargado) |
| Sonda `new FrmDashBoard()` + ícono | ✅ 21 imágenes, clave `estadisticas.png` presente |
| Scan `SetKeyName` vs `ImageStream` (todos los `.Designer.cs`) | ✅ sin desajustes (antes: `FrmDashBoard` max=20 vs 20 imágenes) |
| Lanzamiento real del EXE | ✅ proceso vivo con ventana `FrmLogin` (antes: exit `0xE0434352`, evento .NET Runtime 1026 `FileLoadException`) |

---

## 4. Cobertura vs requisitos (extracto)

| Requisito | Prioridad | Estado |
|---|---|---|
| HU-01 / RF-NAV-01 botón ESTADÍSTICAS con ícono entre USUARIOS y SALIR | Must | ✅ T19/T20 |
| RF-NAV-03 / RNF-05 permisos solo Administrador | Must | ✅ T19 (3 roles) |
| RF-FRM-02 barra de filtros + 6 pestañas | Must | ✅ T10–T15 |
| RF-GRF-01..04 gráficos y render | Must | ✅ T16 (ver ABIERTO-2 para granularidad) |
| RF-UX-01 tooltips | Must | ✅ configurado (audit de código) |
| RF-UX-05 errores con `MessageBox` amigable | **Must** | ❌ ABIERTO-3 |
| RF-EXP-01 export PNG del gráfico activo | Should | ❌ ABIERTO-1 (placeholder) |
| HU-06 / RF-GRF-05 Top Clientes (`venta_top_clientes`) | Should | ❌ ABIERTO-4 (SP/widget no existen) |
| RNF-10 localización `es-DO` / RD$ | — | ⚠ ABIERTO-5 |
| RF-UX-02 leyenda clickeable / RF-UX-06 persistencia de filtros | Should/Could | ⚠ ABIERTO-5 |
| Datos: KPIs, top, distribución, filtros, stock | — | ✅ T1–T8 contra verdad SQL |

---

## 5. Bugs corregidos en esta ronda (6 — todos re-validados)

### BUG-01 · CRÍTICO · La aplicación no arrancaba (`FileLoadException`)
- **Síntoma:** ejecutar `WindowsFormsApp1.exe` termina inmediatamente (exit `0xE0434352`); evento *Application → .NET Runtime 1026*: `System.IO.FileLoadException` en `ManifestBasedResourceGroveler.CreateResourceSet` pidiendo `System.Resources.Extensions, Version=4.0.0.0`.
- **Causa raíz:** `Sistema.Presentacion.csproj` tenía `GenerateResourceUsePreserializedResources=true` + paquete `System.Resources.Extensions 8.0.0` → los `.resources` de `FrmLogin`, `FrmDashBoard`, `FrmRecuperarContraseña` y `Properties.Resources` se generan con `DeserializingResourceReader` (versión 4.0.0.0) y en `bin\Debug` solo existe la 8.0.0.0 **sin binding redirect**.
- **Fix aplicado:** (a) `App.config` → `bindingRedirect` `System.Resources.Extensions` 0.0.0.0–8.0.0.0 → 8.0.0.0; (b) retirada la propiedad `GenerateResourceUsePreserializedResources` del Debug (vuelve el resgen clásico de .NET Framework; el EXE ya no contiene marcadores preserializados).
- **Validación:** EXE rebuilt sin marcadores; app real abre `FrmLogin`; sondas [4]/[5] OK.

### BUG-02 · CRÍTICO · Ningún módulo conectaba a SQL Server (certificado SSL)
- **Síntoma:** toda llamada a datos falla con `Win32Exception / SqlException: The certificate chain was issued by an authority that is not trusted` (sonda [1]).
- **Causa raíz:** `Conexion.cs` construye `Server=…; Database=…; Integrated Security = SSPI` sin `TrustServerCertificate`. Con Microsoft.Data.SqlClient ≥ 4.0, `Encrypt` es obligatorio y el certificado autofirmado de SQLEXPRESS local no valida (por eso `sqlcmd` necesita `-C`).
- **Fix aplicado:** `Sistema.Datos\Conexion.cs` → cadena con `TrustServerCertificate=True` (rebuild de `Sistema.Datos` + despliegue del DLL).
- **Validación:** sonda [3] conecta; toda la Parte 1 corre contra la BD real.

### BUG-03 · CRÍTICO · `FrmDashBoard` no podía construirse (menú roto)
- **Síntoma:** `IndexOutOfRangeException` en `ImageList.ImageCollection.SetKeyName` dentro de `InitializeComponent` → el Dashboard (y por tanto el acceso al módulo) era inutilizable. (Nota: antes de BUG-01 este crash estaba enmascarado por el `FileLoadException`.)
- **Causa raíz:** `FrmDashBoard.Designer.cs` nombra21 imágenes (índices 0–20, la última `estadisticas.png`) pero `imageList1.ImageStream` en `FrmDashBoard.resx` solo contenía **20**: el ícono del nuevo módulo nunca se agregó (el propio documento de requisitos lo lista como pendiente: *«Resources.resx → Añadir icono "estadisticas.png" al ImageList»*), y no existe ningún archivo `estadisticas.png` en el repo.
- **Fix aplicado:** herramienta `qa_imglist_tool.cs fix` → se regeneró `imageList1.ImageStream` con las 20 imágenes originales (60×60, 32 bpp) **+ ícono generado de barras** para `estadisticas.png`. Scan de verificación en todos los formularios: ✅.
- **Validación:** sonda [5] (21 imágenes + clave presente), T19 ícono OK, T20 posición OK.

### BUG-04 · ALTO · Pestaña "Stock Bajo" siempre vacía
- **Síntoma:** `sp_estadisticas_stock_alertas is not a parameter for procedure` (SqlException capturada y solo volcada a `Debug.WriteLine` → la pestaña quedaba vacía en silencio).
- **Causa raíz:** `DEstadisticas.ObtenerStockBajo` envía `@stock_minimo_umbral`; el SP declara `@StockMinimoUmbral` (la collation ignora mayúsculas pero **sí** distingue guiones bajos).
- **Fix aplicado:** nombre de parámetro corregido en `Sistema.Datos\DEstadisticas.cs`.
- **Validación:** T6 PASS (1 artículo `Critico`) y ejecución directa del SP por sqlcmd.

### BUG-05 · ALTO · Artículos "Críticos" pintados de verde
- **Síntoma:** fila crítica en verde (verde claro FFEBFFEB) en lugar de rojo → el usuario subestima el faltante de stock.
- **Causa raíz:** el SP devuelve `'Critico'` **sin tilde**; `UcAlertasStock` solo comparaba `"CRITICO"`/`"Crítico"`.
- **Fix aplicado:** comparación case/acento-insensible (`Critico`/`Crítico`/`Bajo`) en `Widgets\UcAlertasStock.cs`.
- **Validación:** T17 PASS (rojo FFFFEBEB, amarillo FFFFF8DC, negrita en críticos).

### BUG-06 · MEDIO · Botón ESTADÍSTICAS fuera de posición
- **Síntoma:** `panel1.Controls.SetChildIndex(BtnEstadisticas, 3)` se ejecutaba **antes** de `Add`, con el control aún sin padre → no cumplía el orden requerido (y era propenso a excepción).
- **Fix aplicado:** en `FrmDashBoard.CrearBotonEstadisticas` → primero `Add`, luego `SetChildIndex(..., 3)`.
- **Validación:** T20 PASS (índice 3, orden menú … USUARIOS → **ESTADÍSTICAS\*** → VENTAS …).

---

## 6. Bugs abiertos (priorizados) y recomendaciones

### ABIERTO-1 · MEDIO · Exportación PNG/PDF no implementada de verdad (RF-EXP-01, HU-…)
- **Evidencia:** T18 ❌ — `ChartExportHelper.ExportToPng` genera un lienzo fijo **800×600 con texto fijo**, no captura el gráfico. Además: `BtnExportar_Click` abre el `SaveFileDialog` y luego **solo muestra un MessageBox** (no exporta); `ExportToPdf` no tiene llamadores; `File.OpenWrite` no trunca (archivos previos dejan datos fantasma).
- **Recomendación:** usar la API de LiveCharts (`chart.GetImage()` → encode `SKEncodedImageFormat.Png` a300 DPI, o `SaveAsPng`), guardar con `FileStream(..., FileMode.Create)`, exportar el widget de la pestaña activa, y añadir `ExportToPdf` (SkiaSharp `SKDocument`) o retirar el botón PDF. Mensaje de éxito/error vía MessageBox (RF-UX-05).

### ABIERTO-2 · MEDIO · `sp_dashboard_tendencia` ignora SEMANA/ANIO
- **Evidencia:** T2b ❌ — con granularidad `SEMANA` devuelve30 filas diarias (`IF @Granularidad NOT IN ('DIA','MES') SET @Granularidad='DIA'`, línea ~317 del script), mientras la pestaña Ventas Período sí agrupa semanalmente (T5b ✅6 buckets) → gráficos inconsistentes entre pestañas con el mismo filtro.
- **Recomendación:** extender el SP para `SEMANA`/`ANIO` siguiendo el patrón ya probado de `sp_estadisticas_ventas_periodo` (anclas `DATEADD(WEEK, DATEDIFF(WEEK,0,…),0)` y labels `'Sem dd/MMM'`), o bien ocultar esas opciones en el combo hasta implementarlo. Re-ejecutar T2b tras el cambio.

### ABIERTO-3 · BAJO · Errores invisibles para el usuario (RF-UX-05 — *Must*)
- Los `catch` de `CargarTodoAsync`/widgets solo hacen `Debug.WriteLine`; el usuario no recibe el MessageBox amigable que exige el requisito ("Error al cargar [Gráfico]: …"), y no hay `CancellationToken` real durante la carga.
- **Recomendación:** MessageBox por pestaña (sin bloquear las demás) + log; pasar un `CancellationTokenSource` cancelado en cada nueva carga y en `FormClosing`.

### ABIERTO-4 · BAJO · HU-06 / RF-GRF-05 "Top Clientes" no existe (Should)
- No hay pestaña/widget/SP `venta_top_clientes` (los7 SPs desplegados no lo incluyen). Existe `Widgets\UcTopProductosVendidos.cs` **fuera del `.csproj`** con DTOs inexistentes.
- **Recomendación:** decidir si se implementa (SP + widget + pestaña) o se documenta como fuera de alcance en el documento de requisitos.

### ABIERTO-5 · BAJO · Localización y UX menor
- KPIs con `ToString("N2")` sin `es-DO` ni prefijo `RD$` (RNF-10 y RF-UX-01 piden formato moneda RD$).
- Falta leyenda clickeable (RF-UX-02), persistencia de filtros en Settings (RF-UX-06), el **margen calculado `(ventas−ingresos)/ventas*100` en la pestaña Tendencia** (RF-GRF-04) y márgenes de espaciado en ese gráfico; zoom-reset real: `_btnZoomReset` asigna `_chart.ZoomMode = _chart.ZoomMode` (no resetea) y `_currentFilter` es código muerto en `UcTrendChart`/`UcVentasPeriodo`.

### ABIERTO-6 · BAJO · Higiene de código / deuda técnica
- Código muerto fuera de compilación: `Sistema.Negocios\Interfaces\IEstadisticasService.cs` y `Widgets\UcTopProductosVendidos.cs` (no están en ningún `.csproj`).
- `ExportToPdf` sin uso; `Controls.RemoveAt(0)` sin `Dispose` al cambiar de formulario → posible fuga de recursos SkiaSharp.
- 4 warnings CS0169 preexistentes (`FrmIngresos.nombreAnt/id`, `FrmVenta.nombreAnt/id`).

### Warnings de datos (no del código)
- **Bases monetarias inconsistentes entre pestañas:** KPIs/Tendencia usan `venta.total` (con ITBIS → RD$59.00) y Top Productos/Distribución usan el neto de detalle (→ RD$50.00). Los totales no cuadran entre pestañas; conviene unificar la base (recomendado: neto + impuesto como línea separada) o documentarlo.
- `venta.estado` mezcla valores numéricos (`1`) y texto (`'Anulado'`); los SP filtran `ISNULL(estado,'') <> 'Anulado'`. Funciona hoy, pero el dominio del campo es frágil.
- Entorno: `sqlcmd` requiere `-C`; el `TrustServerCertificate=True` agregado es correcto para desarrollo local, en producción debe sustituirse por un certificado válido.

---

## 7. Artefactos de QA y cómo re-ejecutar

**Archivos (proyecto raíz `Scripts\`)** — no commitear si el repo no debe incluir QA:

| Archivo | Propósito |
|---|---|
| `qa_smoke_ui.cs` | Suite principal (30 tests: negocio+BD y UI/permisos/gráficos) |
| `qa_probe.cs` | Sondas: conexión, ctors de formularios, ícono ImageList |
| `qa_imglist_tool.cs` | `scan` (desajustes SetKeyName/ImageStream) y `fix` (regenerar ImageStream) |
| `qa_integracion_estadisticas.sql` / `qa_verdad_terreno.sql` / `qa_confirmacion_bug.sql` | Pruebas SQL de SPs, índices y verdad de terreno |
| `*_out.txt` | Logs de evidencia de la última ejecución |

**Re-ejecución** (desde la raíz del proyecto; `$csc` = Roslyn de VS18):

```bat
copy WindowsFormsApp1\bin\Debug\WindowsFormsApp1.exe.config WindowsFormsApp1\bin\Debug\qa_smoke.exe.config
%csc% /langversion:latest /out:WindowsFormsApp1\bin\Debug\qa_smoke.exe ^
 /r:WindowsFormsApp1\bin\Debug\WindowsFormsApp1.exe ^
 /r:WindowsFormsApp1\bin\Debug\Sistema.Negocios.dll ^
 /r:WindowsFormsApp1\bin\Debug\Sistema.Datos.dll ^
 /r:WindowsFormsApp1\bin\Debug\Sistema.Entidad.dll ^
 /r:WindowsFormsApp1\bin\Debug\LiveChartsCore.SkiaSharpView.WinForms.dll ^
 /r:System.Windows.Forms.dll /r:System.Drawing.dll Scripts\qa_smoke_ui.cs
WindowsFormsApp1\bin\Debug\qa_smoke.exe   → log en qa_smoke_out.txt
```

El directorio `WindowsFormsApp1\bin\Debug` quedó **limpio de artefactos `qa_*`** (solo se conservan fuentes y logs en `Scripts\`).

**Nota de entorno:** durante laQA se cerró una instancia zombie de la app (PID 21632, sin ventana, bloqueaba el rebuild). La app fue relanzada para verificar el arranque y cerrada tras la prueba.

---

## 8. Conclusión

El módulo Estadísticas **cumple su funcionalidad central** (KPIs, tendencia, top productos, distribución, ventas por período, stock bajo; filtros; permisos; navegación; render de gráficos) contra la BD real, tras corregir6 bugs —3 de ellos críticos que impedían usar el sistema completo (arranque, conexión SQL y Dashboard).

**Quedan2 FAIL de la suite**, ambos medios y acotados: exportación PNG real (RF-EXP-01) y agrupación SEMANA/ANIO en la Tendencia. La aprobación completa del módulo queda **condicionada a cerrar ABIERTO-1 y ABIERTO-2**; los ABIERTO-3..6 pueden entrar al backlog con sus prioridades (RF-UX-05 es *Must* y conviene tratarlo junto con ABIERTO-1).
