# 01 — Requisitos (ingeniería inversa del sistema construido)

**Sistema:** Sistema de Ventas e Inventario "SEMANA 11" — aplicación de escritorio Windows Forms (.NET Framework 4.8, C#), arquitectura en 3 capas + entidades/DTOs, persistencia en SQL Server (`mydatabase2`, acceso vía stored procedures con `Microsoft.Data.SqlClient`), arranque en `Program.cs → FrmLogin`.

## 1. Resumen del sistema

Sistema monousuario-por-sesión de gestión comercial para una PYME dominicana ("COMERCIAL EJEMPLO S.R.L.", RNC y dirección constantes en `Empresa.cs` e impresas en factura). Cubre el ciclo compra→inventario→venta: maestros de **Categorías, Artículos (con imagen + código de barras CODE128), Proveedores, Clientes, Usuarios/Roles**, documentos de **Ingresos (compras)** y **Ventas (facturación con ITBIS, comprobantes NCF, impresión/vista previa)**, más dos superficies analíticas: **Dashboard** (`UcDashboard` + `FrmDashboardHost`, 4 widgets) y **Estadísticas** (`FrmEstadisticas`, 6 pestañas con LiveCharts2/SkiaSharp). El control de acceso es por **rol** (Administrador / Vendedor / Almacén) aplicado tanto en menús (`FrmPrincipal`, legado MDI) como en el shell vigente (`FrmDashBoard` + `PnlContenedor`), más alcance de datos por vendedor en los módulos analíticos. Todo el acceso a datos es asíncrono (`async/await`), con validaciones dominicanas (cédula 11 dígitos, teléfono 809/829/849, email, clave ≥8 + dígito + especial) y estados lógicos (Activo/Desactivado/Anulado/Eliminado).

**Módulos identificados:** (1) Autenticación y recuperación de clave; (2) Shell/navegación y autorización por rol; (3) Categorías; (4) Artículos + código de barras + imagen; (5) Personas: Proveedores / Clientes (+ vistas selectoras); (6) Usuarios + Roles; (7) Ingresos (compras); (8) Ventas (facturación, detalle, ITBIS, impresión); (9) Dashboard analítico; (10) Estadísticas (KPIs, tendencia, top productos, distribución, ventas período, stock bajo, exportación); (11) Transversales (factura impresa, empresa, filtros).

## 2. Actores / roles identificados

| Rol | Evidencia | Alcance observado |
|---|---|---|
| **Administrador** | `FrmDashBoard.AplicarPermisos()`, `FrmPrincipal.FrmPrincipal_Load`, `UcDashboard.ApplyRolePermissions`, `FrmEstadisticas.CrearFiltro/CargarFiltrosAsync` (`Variables.Rol != "Administrador" ? IdUsuario : null`), SP `sp_dashboard_*` con `@IdUsuario NULL = todos` | Acceso total: todos los botones/menús, filtro vendedor = "Todos", KPIs globales, gestión de usuarios/roles |
| **Vendedor** | Mismo código: ve Ingresos + Clientes; ocultos Categoría/Artículos/Proveedores/Usuarios/Estadísticas; dashboard con `CmbVendedor.Enabled=false` | Registra y consulta **sus propias** ventas/clientes; sin gestión de almacén ni usuarios |
| **Almacén** | Ve Categoría/Artículos/Ingresos/Proveedores; ocultos Clientes/Usuarios/Estadísticas/Ventas | Mantiene catálogo y stock vía ingresos; sin facturar |
| **Usuario no autenticado** | `Program.cs` arranca en `FrmLogin` | Solo login + recuperación de contraseña |
| **Sistema SMTP (actor externo)** | `FrmRecuperarContraseña.EnviarCorreo` (smtp.gmail.com:587, SSL) | Emite código de 6 dígitos para restablecer clave |

> Nota: `FrmPrincipal` (MDI legado) y `FrmDashBoard` (shell vigente) definen matrices de permisos **ligeramente distintas** para Vendedor/Almacén (ver RF-ADM y §7 Limitaciones).

## 3. Historias de usuario (inferidas del sistema real)

- Como **usuario** quiero **iniciar sesión con email y clave validados** para **acceder según mi rol** (`FrmLogin.BtnAcceder_Click`).
- Como **usuario** quiero **recuperar mi clave por código enviado a mi correo (expira en 10 min)** para **volver a entrar sin ayuda del admin** (`FrmRecuperarContraseña`: 3 paneles Enviar→Verificar→Cambiar).
- Como **Administrador** quiero **gestionar usuarios con rol, documento, contacto y estado** para **controlar el acceso** (`FrmUsuario`).
- Como **Almacén** quiero **gestionar categorías y artículos con precio, stock, imagen y código de barras** para **mantener el catálogo vendible** (`FrmCategoria`, `FrmArticulo` + BarcodeLib).
- Como **Almacén/Vendedor** quiero **registrar ingresos (compras a proveedor) con detalle y comprobante** para **aumentar stock** (`FrmIngresos` + `NIngresos.Insertar`).
- Como **Vendedor** quiero **facturar ventas con cliente, detalle, descuentos e ITBIS, y previsualizar/imprimir la factura NCF** para **cerrar la venta formalmente** (`FrmVenta` + `ImpresorFactura`).
- Como **Vendedor** quiero **anular (no borrar) ventas/ingresos con confirmación** para **corregir sin perder trazabilidad** (`BtnAnular` en ambos formularios).
- Como **Administrador** quiero **ver KPIs, tendencias, top productos, distribución por categoría, ventas por período y alertas de stock con filtros de fecha/vendedor/categoría/granularidad/métrica** para **decidir** (`FrmEstadisticas`, `UcDashboard`, SPs `sp_dashboard_*`).
- Como **Administrador** quiero **exportar gráficos a PNG/PDF** para **informes** (`ChartExportHelper`, `BtnExportar`, toolbar en `UcVentasPeriodo`).

## 4. Requisitos funcionales (por módulo)

### M-AUT — Autenticación y recuperación

| ID | Descripción | Evidencia (archivo/clase) | Criterio de aceptación inferido |
|---|---|---|---|
| RF-001 | Login con email + clave; valida formato email (`MailAddress` + host con punto) y clave (≥8 caracteres, ≥1 dígito, ≥1 especial) antes de consultar BD | `FrmLogin.ValidarFormato()`, `BtnAcceder_Click → NUsuario.Login` | Con credenciales válidas y usuario activo abre `FrmDashBoard` con `Idusuario/Rol/Nombre`; con formato inválido muestra advertencia y no consulta BD |
| RF-002 | Rechazo de credenciales incorrectas y de usuarios inactivos con mensajes diferenciados | `FrmLogin.BtnAcceder_Click` (Rows.Count==0 → "Email o Clave Incorrectas"; `Rows[0][4]==false` → "Usuario Inactivo") | Mensaje de error específico en cada caso; no se abre el shell |
| RF-003 | Mostrar/ocultar clave (botón press-and-hold) y placeholders "Usuario"/"Clave" | `FrmLogin.BtnShowPassword_MouseDown/Leave`, `Txt*_Enter/Leave` | Mantener presionado revela; al soltar vuelve a `*`; placeholders se restauran al vaciar |
| RF-004 | Recuperación en 3 pasos: (1) enviar código de 6 dígitos al email registrado, (2) verificar código con expiración de 10 min, (3) cambiar clave con confirmación e igual política que RF-001 | `FrmRecuperarContraseña` (`BtnEnviarCodigo_Click`, `BtnVerficarCodigo_Click`, `BtnCambiar_Click`, `EnviarCorreo` SMTP Gmail) | Solo emails existentes (`NUsuario.BuscarPorEmail`) reciben código; código erróneo/expirado se rechaza con retorno al paso 1; claves coincidentes y válidas → `NUsuario.CambiarClave` = "OK" y cierre del diálogo |
| RF-005 | Hash de clave en BD (el SP aplica `HASHBYTES`; la capa negocio pasa la clave en claro con el comentario explícito) | `NUsuario.Insertar` (comentario "el hash lo aplica directamente el SP") | La clave nunca se persiste en claro verificable desde la app; login compara contra hash en el SP |

### M-SHELL — Navegación y autorización

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-006 | Shell principal `FrmDashBoard` con barra de título propia (cerrar/maximizar/minimizar/restaurar, arrastre por P/Invoke) y contenedor `PnlContenedor` que aloja un formulario hijo a la vez (`AbrirFormulario`) | `FrmDashBoard.cs` | Solo un módulo visible por vez; maximizar ocupa el área de trabajo y restaurar vuelve a 1300×650 |
| RF-007 | Matriz de visibilidad por rol en el shell vigente | `FrmDashBoard.AplicarPermisos()`: Admin=todos+Estadísticas; Vendedor=Ingresos+Clientes; Almacén=Categoría+Artículos+Ingresos+Proveedores | Cada rol ve exactamente sus botones; `BtnEstadisticas` oculto salvo Admin |
| RF-008 | Vista inicial según rol: Admin → `FrmEstadisticas`; otros → `FrmDashboardHost` (dashboard) + saludo "Bienvenido(a)…" | `FrmDashBoard_Load` | Al entrar, el contenido inicial corresponde al rol |
| RF-009 | Shell legado MDI `FrmPrincipal` con menús Almacén/Ingresos/Ventas/Acceso/Consultas y matriz propia por rol + confirmación de salida | `FrmPrincipal.cs` | Funcional pero **no es la ruta principal** (el login abre `FrmDashBoard`, no `FrmPrincipal`) |
| RF-010 | Sesión global en memoria (`Variables.IdUsuario/Rol/…`) usada por ventas, ingresos y filtros analíticos | `Variables.cs`, usos en `FrmVenta/ FrmIngresos/ UcDashboard/ FrmEstadisticas` | Toda operación registra el `IdUsuario` de la sesión; los filtros no-Admin se fuerzan a ese id |

### M-CAT — Categorías

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-011 | CRUD de categorías (nombre obligatorio + descripción) con pestañas Listado/Mantenimiento, búsqueda en vivo, conteo "Total Registros", edición por doble clic (excepto en checkbox) | `FrmCategoria.cs` (`Listar/Buscar/Insertar/Actualizar/Eliminar/Activar/Desactivar`, `TxtBuscar_TextChanged`, `CellDoubleClick`) | Insertar/actualizar con nombre vacío se rechaza; doble clic carga el registro en la pestaña de mantenimiento |
| RF-012 | Activar / Desactivar / Eliminar lógicamente en lote vía columna checkbox + `ChkSeleccionar`, con confirmación Yes/No | `FrmCategoria.Desactivar/Activar/Eliminar` | Solo filas marcadas cambian de estado; tras operar se refresca el listado |

### M-ART — Artículos

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-013 | CRUD de artículos: categoría (solo activas en el combo), código, nombre, precio venta, stock, descripción, imagen (ruta completa VARCHAR(500)) | `FrmArticulo.cs` (`Insertar` exige los 6 campos + categoría; `CargarCategorias` filtra `Estado=true`; `Actualizar` conserva imagen previa si no hay nueva) | Sin campos obligatorios no inserta; el combo nunca ofrece categorías inactivas |
| RF-014 | Generar vista previa de código de barras CODE128 del campo Código y guardarla como PNG en disco | `BtnGenerarCodigo_Click` / `BtnGuardarCodigo_Click_1` (BarcodeLib `TYPE.CODE128`, `SaveFileDialog *.png`) | Tras generar, el botón se deshabilita; el PNG se guarda donde elija el usuario |
| RF-015 | Cargar imagen (jpeg/jpg/jpe/png) con vista previa; activación/desactivación/eliminación en lote como en categorías | `BtnCargarImagen_Click`, `Activar/Desactivar/Eliminar` | Imagen inexistente en disco → preview vacío sin romper la edición |
| RF-016 | Búsqueda de artículo por código de barras exacto para agregar a documentos | `NArticulo.BuscarCodigo`, usado en `FrmVenta.TxtCodigo_KeyDown` y `FrmIngresos.TxtCodigo_KeyDown` | Código inexistente → "No existe un artículo con ese código de barras" |

### M-PER — Proveedores y Clientes

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-017 | CRUD de proveedores (`tipoPersona="Proveedor"`) y de clientes (`"Cliente"`) con nombre + tipo/num documento obligatorios; validación dominicana: CÉDULA/DNI 11 dígitos (`\d{3}-?\d{7}-?\d{1}`), PASAPORTE letra+7 dígitos, dirección 5–150 caracteres permitidos, teléfono 809/829/849, email válido | `FrmProveedores.cs`, `FrmCliente.cs` (`ValidarFormato`, `Insertar/Actualizar/Eliminar`, `NPersona.*`) | Documento/teléfono/dirección/email inválidos bloquean el guardado con mensaje específico y foco en el campo |
| RF-018 | Búsqueda en vivo + conteo de registros + edición por doble clic + eliminación en lote con confirmación | Mismos formularios | Igual comportamiento que M-CAT |
| RF-019 | Diálogos selectores `FrmVistaProveedores` / `FrmVistaCliente` (listar+buscar, doble clic devuelve `Variables.IdProveedor/NombreProveedor` o `IdCliente/NombreCliente`) usados desde Ingresos y Ventas | `FrmVistaCliente.cs`, `BtnBuscarProveedor_Click` en ambos documentos | Elegir en el selector rellena Id+Nombre en el documento que lo invocó |
| RF-020 | Catálogo selector de artículos `FrmMostrarArticulos` (solo `Estado=true`, doble clic devuelve Id/Código/Nombre/Precio vía `Variables`) usado desde Ingresos y Ventas | `FrmMostrarArticulos.cs` | Solo artículos activos seleccionables; al aceptar (`DialogResult.OK`) el documento agrega la línea |

### M-USU — Usuarios y Roles

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-021 | CRUD de usuarios: rol (combo de `NRol`), nombre, tipo doc (DNI/Cédula/Pasaporte), num doc, dirección, teléfono, email, clave; mismas validaciones dominicanas que M-PER + clave con política RF-001 (obligatoria al crear, **opcional al actualizar**: vacío conserva la actual) | `FrmUsuario.cs` (`Insertar/Actualizar`, `ValidarFormato`, `CargarRoles`) | Crear sin obligatorios o con formato inválido se rechaza; actualizar con clave vacía no la cambia |
| RF-022 | Unicidad de email (verificación previa `Existe`; al actualizar solo si el email cambió, case-insensitive) | `NUsuario.Insertar/Actualizar` | Email duplicado → "Ya existe un usuario con ese email" |
| RF-023 | Activar/Desactivar/Eliminar en lote con confirmación; listado con columna Estado y conteo | `FrmUsuario.Activar/Desactivar/Eliminar` | Igual patrón que M-CAT |
| RF-024 | Mantenimiento de roles de **solo lectura** (listado + conteo) | `FrmRol.cs` (solo `Listar`) | El formulario no expone alta/baja/modificación de roles |

### M-ING — Ingresos (compras)

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-025 | Registrar ingreso: proveedor (selector), usuario de sesión, tipo/serie/número de comprobante, impuesto, total, y detalle (artículo/cantidad/precio/importe=precio×cantidad); validación de cantidad entera con mensaje específico | `FrmIngresos.cs` (`BtnInsertar_Click → NIngresos.Insertar`, `AgregarDetalle`, `CalcularTotales` con `Total/(1+Impuesto)`, `DgvDetalle_DataError`) | Sin proveedor/impuesto/número o sin líneas no guarda; cantidad decimal → advertencia "número entero" |
| RF-026 | Agregar líneas por código de barras (Enter) o por catálogo, sin duplicados (mismo artículo dos veces se rechaza) | `TxtCodigo_KeyDown`, `BtnVerArticulos_Click`, `AgregarDetalle` | Duplicado → "El articulo ya ha sido agregado" |
| RF-027 | Listar/buscar ingresos con formato de columnas, ver detalle por doble clic (subtotal/ITBIS/total recalculados), anular en lote con confirmación OK/Cancel | `Listar/Buscar/DgvListado_CellDoubleClick/BtnAnular_Click` | Anulados cambian `estado` a 'Anulado' (no se borran); quedan excluidos de los reportes (`<> 'Anulado'`) |

### M-VEN — Ventas (facturación)

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-028 | Registrar venta: cliente (selector), usuario de sesión, tipo/serie/número, impuesto, total, detalle con **descuento por línea** (importe=precio×cantidad−descuento); reglas: descuento ≥0 y ≤ importe; cantidad entera | `FrmVenta.cs` (`BtnInsertar_Click → NVenta.Insertar`, `DgvDetalle_CellEndEdit` con excepciones específicas, `CalcularTotales` Subtotal+ITBIS redondeado a 2) | Descuento negativo o mayor al importe se resetea a 0 y lanza error explicativo; totales con formato `RD$` (`ImpresorFactura.Monto`) |
| RF-029 | Agregar líneas por código de barras o catálogo, sin duplicados; ver detalle por doble clic; anular en lote con confirmación | Igual que M-ING + `CellDoubleClick` con panel `panelDetalleIngreso` | Mismo comportamiento; anuladas excluidas de analítica |
| RF-030 | Impresión de factura: botones **Imprimir / Reimprimir** habilitados solo si hay fila actual y `Tipo_Comprobante` ∈ {FACTURA, TICKET, BOLETA}; **las GUÍAs no imprimen** ("Las guías no generan factura"); vista previa con `PrintPreviewDialog`, multipágina, marca "REIMPRESIÓN" en reimpresión | `FrmVenta.ActualizarBotonesImpresion/EsComprobanteImprimible/ImprimirVenta`, `ImpresorFactura.VistaPrevia` | Guía seleccionada → botones deshabilitados y mensaje de bloqueo; reimpresión muestra sello rojo |
| RF-031 | Formato de factura dominicana: cabecera empresa (Nombre/RNC/Dirección de `Empresa.cs`), título según tipo (FACTURA→"Comprobante de Crédito Fiscal"; TICKET/BOLETA→"Comprobante de Consumo"), caja NCF (serie+número), fecha/vendedor/cliente/documento (etiqueta Cédula si 11 dígitos, RNC si 9), tabla 6 columnas, caja Subtotal/ITBIS %/Total, leyenda fiscal, firma del vendedor, "Gracias por su compra"; cultura `es-DO`, moneda `RD$`, papel Carta | `ImpresorFactura.cs` completo | Vista previa contiene todos los bloques; si el total recalculado difiere del guardado (>0.01) advierte y usa el recalculado |
| RF-032 | Datos de empresa editables como constantes (`Rnc`, `Nombre`, `Direccion`) | `Empresa.cs` | Cambiar las 3 constantes cambia todas las facturas futuras |

### M-DASH — Dashboard analítico

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-033 | Panel con filtros Desde/Hasta (defecto últimos 30 días), Vendedor, Categoría, Granularidad (Día/Mes), Métrica (Monto/Cantidad), Auto-refresh (OFF/5/15/30/60 min) + botón ACTUALIZAR + "Última actualización HH:mm:ss" + skeletons durante la carga | `UcDashboard.cs` | Cambiar auto-refresh ajusta `Timer.Interval` o lo detiene; durante la carga se ven las 4 barras marquee |
| RF-034 | Widgets: tarjetas KPI, tendencia (serie temporal), top productos y dona por categoría (LiveCharts2); carga secuencial actual (comentario Sprint 2 indica que tendencia/top/dona se cablearían cuando sus widgets estén listos) | `UcDashboard.LoadDashboardAsync` (solo `ObtenerKpisAsync` activo), 4 `Uc*` instanciados | KPIs se pueblan siempre; los demás widgets dependen del cableado pendiente en este control (en `FrmEstadisticas` sí están cableados) |
| RF-035 | Alcance por rol: no-Admin ve solo sus datos (`CmbVendedor.Enabled=false`, filtro forzado a `Variables.IdUsuario`); Admin puede elegir "Todos" o un vendedor | `UcDashboard.ApplyRolePermissions/CrearFiltro/LoadFiltersAsync`, SPs con `@IdUsuario NULL=todos` | Vendedor no puede ver datos ajenos ni habilitar el combo |

### M-EST — Estadísticas

| ID | Descripción | Evidencia | Criterio de aceptación inferido |
|---|---|---|---|
| RF-036 | 6 pestañas: KPIs (6 tarjetas: VentasTotales, IngresosTotales, TicketPromedio, NumTransacciones, VariaciónVsPeríodoAnterior %, MargenBruto), Tendencia, Top Productos, Distribución, Ventas Período, Stock Bajo; carga **paralela** (`Task.WhenAll` de 6) con skeletons por pestaña y estado global ("Cargando…/Última actualización/Hora del error") | `FrmEstadisticas.cs` (`CargarTodoAsync`, `Cargar*Async`, `BuildUI`), DTOs `KpiCardsDto/TrendDto/TopProductsDto/CategoryDistributionDto/VentasPeriodoDto/StockBajoDto` | Un fallo en una pestaña no tumba las demás (catch por pestaña con `Debug.WriteLine`); el botón queda deshabilitado con texto "Cargando…" durante la recarga |
| RF-037 | Filtros: Desde/Hasta (defecto −29 días→hoy), presets (Personalizado/Hoy/Esta semana/Este mes/Este año que recalculan las fechas), Categoría, Vendedor, Granularidad (Día/Semana/Mes/Año — se normaliza a valores válidos), Métrica (Monto/Cantidad); validación Fin<Inicio con advertencia | `BuildUI` (combos + `CmbPreset_SelectedIndexChanged`), `CrearFiltro`, `NEstadisticas.Validar/Normalizar` | Preset ajusta fechas; rango invertido se rechaza (UI) o se auto-corrige (capa negocio) |
| RF-038 | Alertas de stock: umbral por defecto 5 (negativo→5), opcional filtro por categoría; estados "Bajo/Crítico" por artículo | `CargarStockBajoAsync` (`new StockFilterDto(5, idCat)`), `UcAlertasStock`, `StockBajoItemDto` | Con umbral negativo se usa 5; la pestaña lista artículos bajo el umbral con su estado |
| RF-039 | Exportación: botón EXPORTAR contextual por pestaña (Ventas Período sugiere el botón del toolbar; Tendencia "próximamente"; resto "solo series temporales"); utilidades PNG (placeholder SkiaSharp 800×600) y PDF A4 (título + rango/granularidad/métrica + pie "Sistema SEMANA 11") + diálogo con nombre `Estadisticas_<Tab>_<yyyyMMdd_HHmmss>` | `BtnExportar_Click`, `Helpers/ChartExportHelper.cs` (`ExportToPng/ExportToPdf/CreateSaveDialog`) | Exportar fuera de series temporales informa la limitación en lugar de fallar; el PDF incluye filtros aplicados |
| RF-040 | Reglas de negocio analíticas (lado BD): solo documentos no anulados (`ISNULL(estado,'')<>'Anulado'`); series continuas sin huecos (tally días/meses); top 10 ordenable por MONTO/CANTIDAD; distribución % sobre ventas del período (solo categorías activas); KPIs con ticket promedio, variación vs período anterior del mismo tamaño y margen bruto (neto − costo promedio ponderado de compra); índices `IX_*` sugeridos | `SQL_Scripts/Dashboard_SPs.sql` (5 SPs + índices) | Métricas excluyen anulados; granularidad inválida cae a 'DIA'; métrica inválida a 'MONTO' |

## 5. Requisitos no funcionales (con evidencia)

| ID | Categoría | Descripción | Evidencia |
|---|---|---|---|
| RNF-001 | Plataforma/Compatibilidad | Escritorio Windows, .NET Framework 4.8, WinForms, AnyCPU; dependencias NuGet locales (`packages/`: LiveChartsCore 2.0.5, SkiaSharp 3.119, BarcodeLib 2.2.2, etc.) | `Sistema.Presentacion.csproj` (`TargetFrameworkVersion v4.8`), `App.config` (binding redirects), `packages.config` |
| RNF-002 | BD/Compatibilidad | SQL Server 2016 SP1+ (usa `FORMAT`, `DATEADD/DATEDIFF`, CTEs, `OUTER APPLY`); BD `mydatabase2` en `localhost\SQLEXPRESS` con **Windows Auth (SSPI)** por defecto y `TrustServerCertificate=True` | `Dashboard_SPs.sql` (cabecera + sintaxis), `Conexion.cs` (singleton, `Integrated Security=SSPI`, `Seguridad=true`) |
| RNF-003 | Rendimiento | Acceso a datos 100% asíncrono (`async/await` en todas las capas N*/D*); carga paralela de 6 reportes y de filtros (vendedores+categorías); SPs con índices covering sugeridos; skeletons + botón "Cargando…" para no bloquear la UI | `FrmEstadisticas.CargarTodoAsync` (`Task.WhenAll`), `NEstadisticas.ObtenerFiltrosAsync`, `Conexion`/D* con `Microsoft.Data.SqlClient` async, índices `IX_venta_fecha_estado`, etc. |
| RNF-004 | Usabilidad | Español dominicano en toda la UI; fuente corporativa Century Gothic; paleta (DodgerBlue ACTUALIZAR, MediumSeaGreen EXPORTAR); layout `TableLayoutPanel` responsivo; placeholders, totales de registros, mensajes OK/Error uniformes ("Sistema de Ventas"), confirmaciones Yes/No y OK/Cancel antes de operaciones destructivas | Todos los `Frm*.cs` (`MensajeOK/MensajeError`, `LblTotal`, `ChkSeleccionar`), `FrmEstadisticas.BuildUI`, `UcDashboard` |
| RNF-005 | Accesibilidad/UX | Ventanas sin borde arrastrables (login/recuperación), contraseña con revelado temporal, edición por doble clic, validación con foco en el campo erróneo, `DataError` con mensajes amigables para cantidades | `FrmLogin/FrmRecuperarContraseña` (P/Invoke arrastre), `DgvDetalle_DataError` en Ventas e Ingresos |
| RNF-006 | Seguridad (implementada) | Autorización por rol en UI + alcance de datos por vendedor en analítica; usuarios inactivos bloqueados; unicidad de email; hash de clave en SP; política de clave fuerte; código de recuperación de 6 dígitos con expiración de 10 min vía SMTP+SSL | `AplicarPermisos/ApplyRolePermissions`, `sp_dashboard_*` + `CrearFiltro`, `NUsuario`, `FrmRecuperarContraseña` |
| RNF-007 | Seguridad (debilidades observadas — no son requisitos, son riesgos) | Credencial SMTP de Gmail en claro en el fuente; cadena con `sa`/clave vacía como rama alternativa; sesión global mutable sin timeout; mensajes que concatenan `ex.StackTrace` | `FrmRecuperarContraseña` (constantes `CorreoEmisor/ContrasenaEmisor`), `Conexion.cs`, `Variables.cs`, `FrmCliente/FrmProveedores` (`MessageBox.Show(ex.Message + ex.StackTrace)`) |
| RNF-008 | Moneda/Localización | `RD$`, cultura `es-DO` (`N2`), fechas `dd/MM/yyyy`, ITBIS como impuesto parametrizable por documento redondeado a 2 decimales | `ImpresorFactura` (`Moneda`, `CulturaDO`, `Porcentaje`) |
| RNF-009 | Mantenibilidad | 3 capas (Datos/Negocios/Entidad) + Presentación; fachadas estáticas `N*`/`D*` por agregado; DTOs `record` con polyfill `IsExternalInit` para net48; widgets reutilizables (`Widgets/Uc*`); SPs versionados en `SQL_Scripts/` | Estructura de carpetas `Sistema.Datos/Negocios/Entidad`, `Widgets/`, `Helpers/`, `DTOs/` |
| RNF-010 | Fiabilidad/Robustez | Validaciones en UI + negocio + BD; normalización defensiva (fechas invertidas, granularidad/métrica inválidas, umbral negativo); errores por pestaña aislados; `Dispose` de `Timer` y de recursos GDI de impresión | `NEstadisticas.Validar/Normalizar`, `UcDashboard.Dispose`, `ImpresorFactura.Recursos.Dispose` |
| RNF-011 | Impresión | Carta 850×1100 a 72dpi, márgenes 40, paginación real (`HasMorePages`, reserva de cierre 250px), encabezado repetido desde pág. 2 | `ImpresorFactura.VistaPrevia/Documento_PrintPage` |
| RNF-012 | Identidad visual | Logo en grises puros sobre panel DodgerBlue (`PicLogo`, PNG `-bn` + resx); tarea LOGO-BN-001 cerrada en QA 7/7 | `FrmDashBoard.Designer.cs:349`, `Resources/*-bn.png`, `PIPELINE.md` |

## 6. Alcance de este levantamiento

**Dentro:** todo lo observable en el código fuente del repo (formularios, capas, DTOs, SPs, csproj/config, recursos) más los artefactos previos de `BUG-UI-001`/`LOGO-BN-001` citados en `PIPELINE.md`.
**Fuera:** comportamiento en ejecución (tiempos reales, volúmenes), esquema completo de BD (solo se vio `Dashboard_SPs.sql` + `mydatabase.bak` sin inspeccionar), reportes `.zip`, y la credencial del Admin (no persistida por política).

## 7. Supuestos y limitaciones

1. **Fuente única = lectura estática** (no se ejecutó la app ni se abrió el `.bak`); los criterios de aceptación son *inferidos*, no verificados E2E (salvo LOGO-BN-001 con QA 7/7).
2. **Doble shell:** `FrmDashBoard` es el vigente (lo abre el login); `FrmPrincipal` es legado con una matriz de permisos distinta — se documentan ambas pero la vigente manda.
3. **`UcDashboard` parcialmente cableado:** solo KPIs se cargan en ese control (comentario "Sprint 2"); el análisis completo vive en `FrmEstadisticas`.
4. **Exportación PNG** de estadísticas es actualmente un *placeholder* (dibuja texto indicativo, no el bitmap del chart); el PDF sí arma documento con metadatos.
5. **Rol "Almacen"** aparece como `"Almacen"` (sin tilde) en el código — se respeta literalmente.
6. **Seguridad:** hay secretos en claro y ramas de conexión con `sa` — se reportan como RNF-007, no como comportamiento a replicar.

## 8. Matriz de trazabilidad módulo ↔ archivos

| Módulo | Presentación | Negocios | Datos | Entidad/DTOs | Otros |
|---|---|---|---|---|---|
| Arranque/Sesión | `Program.cs`, `Variables.cs`, `App.config` | — | — | — | `Sistema.slnx`, `*.csproj` |
| Autenticación | `FrmLogin.*`, `FrmRecuperarContraseña.*` | `NUsuario.cs` | `DUsuario.cs`, `Conexion.cs` | `Usuario.cs` | SMTP Gmail externo |
| Shell/Roles | `FrmDashBoard.*`, `FrmPrincipal.*` | — | — | — | `Resources/` (logo `-bn`) |
| Categorías | `FrmCategoria.*` | `NCategoria.cs` | `DCategoria.cs` | `Categoría.cs` | SPs `categoria_*` (en BD) |
| Artículos | `FrmArticulo.*`, `FrmMostrarArticulos.*` | `NArticulo.cs` | `DArticulo.cs` | `Articulo.cs` | BarcodeLib |
| Personas | `FrmCliente.*`, `FrmProveedores.*`, `FrmVistaCliente.*`, `FrmVistaProveedores.*` | `NPersona.cs` | `DPersona.cs` | `Persona.cs` | — |
| Usuarios/Roles | `FrmUsuario.*`, `FrmRol.*` | `NUsuario.cs`, `NRol.cs` | `DUsuario.cs`, `DRol.cs` | `Usuario.cs`, `Rol.cs` | — |
| Ingresos | `FrmIngresos.*` | `NIngresos.cs` | `DIngresos.cs` | `Ingreso.cs` | — |
| Ventas/Factura | `FrmVenta.*`, `ImpresorFactura.cs`, `Empresa.cs` | `NVenta.cs` | `DVenta.cs` | `Venta.cs` | NCF/ITBIS `es-DO` |
| Dashboard | `UcDashboard.cs`, `FrmDashboardHost.cs`, `Widgets/UcKpiCards\|UcTrendChart\|UcTopProducts\|UcCategoryDonut.cs` | `NDashboard.cs` | `DDashboard.cs` | `DTOs/DashboardDtos.cs` | `SQL_Scripts/Dashboard_SPs.sql` (5 SPs + índices) |
| Estadísticas | `FrmEstadisticas.cs`, `Widgets/UcKpiCardsEstadisticas\|UcVentasPeriodo\|UcAlertasStock\|UcTopProductosVendidos.cs`, `Helpers/ChartExportHelper.cs` | `NEstadisticas.cs` | `DEstadisticas.cs` | `DTOs/EstadisticasDtos.cs` | LiveCharts2 + SkiaSharp |

### ⬅ HANDOFF → manager (fase 1)
- **Artefacto:** `.opencode/pipeline/01-requisitos.md`
- **Resumen ejecutivo:** Sistema WinForms 3 capas con 10 módulos; 40 RF + 12 RNF inventariados por lectura estática; roles Admin/Vendedor/Almacén con matrices duales; factura dominicana NCF/ITBIS; analítica con 5 SPs
- **Decisiones tomadas:** Levantamiento por ingeniería inversa, sin ejecución; criterios de aceptación inferidos; se respeta literal "Almacen"; secretos reportados como riesgo no requisito
- **Alcance / qué NO hacer:** No se ejecutó app ni se inspeccionó .bak; no se propone cambio funcional; no se persiste credencial Admin
- **Bloqueos o dudas abiertas:** ninguno para este entregable; pendiente decidir si se quiere fase 2 para documentar/evolucionar
- **Siguiente paso sugerido:** manager persiste artefacto, actualiza PIPELINE.md y reporta al usuario
