# Sistema de Ventas e Inventario "SEMANA 11"

## Especificación de Requisitos de Software (SRS) — v1.0

**Tipo de aplicación:** Escritorio Windows (Windows Forms, .NET Framework 4.8)
**Base de datos:** SQL Server (base `mydatabase2`)
**Fecha:** 13 de octubre de 2026
**Versión del documento:** 1.0
**Origen:** Levantamiento por ingeniería inversa del sistema construido — 40 requisitos funcionales (RF-001 a RF-040) y 12 requisitos no funcionales (RNF-001 a RNF-012).

---

# 1. Introducción

## 1.1 Propósito

Este documento describe, en lenguaje sencillo, todo lo que el Sistema de Ventas e Inventario "SEMANA 11" hace o debe hacer. Es una especificación de requisitos de software (SRS) derivada del sistema ya construido: recoge 40 requisitos funcionales (RF) y 12 requisitos no funcionales (RNF) observados directamente en el código fuente.

## 1.2 Alcance

El sistema cubre el ciclo comercial de una pequeña empresa: compra, inventario y venta. Incluye:

- Autenticación de usuarios y recuperación de contraseña por correo.
- Ventana principal con menús y control de acceso por rol.
- Mantenimiento de categorías, artículos (con imagen y código de barras), proveedores, clientes, usuarios y roles.
- Registro de ingresos (compras) y de ventas (facturación con ITBIS y comprobantes NCF).
- Impresión y vista previa de facturas en formato dominicano.
- Panel de indicadores (Dashboard) y módulo de estadísticas con gráficos y exportación.

Fuera de alcance de este documento: el esquema completo de la base de datos y el comportamiento del sistema bajo carga real.

## 1.3 Definiciones

- **RF-xxx**: requisito funcional — una cosa que el sistema debe hacer.
- **RNF-xxx**: requisito no funcional — una cualidad o condición técnica del sistema.
- **Módulo (M-xxx)**: agrupación funcional de requisitos.
- Ver el apartado 5 (Glosario) para los términos técnicos.

# 2. Descripción general del sistema

## 2.1 Qué es

Es un sistema de gestión comercial para una PYME dominicana (la empresa "COMERCIAL EJEMPLO S.R.L.", cuyo nombre, RNC e dirección se imprimen en cada factura). Funciona en Windows, se conecta a una base de datos SQL Server y usa el rol del usuario para decidir qué puede ver y hacer. Es de un solo usuario activo por sesión.

## 2.2 Roles

- **Administrador:** acceso total: todos los módulos, gestión de usuarios, estadísticas globales; puede ver los datos de todos los vendedores o filtrar por uno.
- **Vendedor:** registra y consulta sus propias ventas y clientes; no gestiona el catálogo, no administra usuarios y no ve estadísticas.
- **Almacén:** mantiene el catálogo (categorías y artículos) y registra ingresos (compras) con proveedores; no factura ventas ni ve estadísticas.
- **Usuario no autenticado:** solo puede iniciar sesión o recuperar su contraseña.

## 2.3 Módulos

1. Autenticación y recuperación de contraseña.
2. Ventana principal y autorización por rol.
3. Categorías.
4. Artículos (imagen + código de barras).
5. Proveedores y Clientes.
6. Usuarios y Roles.
7. Ingresos (compras).
8. Ventas (facturación, ITBIS, impresión).
9. Dashboard analítico (panel de indicadores).
10. Estadísticas (6 pestañas con gráficos y exportación).

# 3. Requisitos funcionales

## 3.1 Autenticación y recuperación (M-AUT)

- **RF-001** — El sistema debe permitir iniciar sesión con correo electrónico y contraseña, validando antes de consultar la base de datos que el correo tenga formato válido y que la contraseña tenga al menos 8 caracteres, un dígito y un carácter especial.
- **RF-002** — El sistema debe rechazar el acceso con mensajes diferentes según el caso: "credenciales incorrectas" cuando el correo o la clave no coinciden, y "usuario inactivo" cuando la cuenta está desactivada. En ningún caso se abre la ventana principal.
- **RF-003** — El sistema debe mostrar u ocultar la contraseña mientras se escribe mediante un botón que solo la revela mientras se mantiene presionado, y ofrecer textos guía ("Usuario" y "Clave") en los campos vacíos.
- **RF-004** — El sistema debe permitir recuperar la contraseña en tres pasos: (1) enviar un código de 6 dígitos al correo registrado, (2) verificar ese código (expira a los 10 minutos) y (3) establecer una nueva contraseña con confirmación y la misma política de fortaleza del RF-001.
- **RF-005** — El sistema debe guardar las contraseñas de forma protegida (con hash en la base de datos), nunca en texto plano legible.

## 3.2 Ventana principal y autorización (M-SHELL)

- **RF-006** — El sistema debe mostrar una ventana principal con barra de título propia (cerrar, maximizar, minimizar, restaurar y arrastre con el mouse) que aloja un solo módulo a la vez en el área de contenido.
- **RF-007** — El sistema debe mostrar a cada rol exactamente sus módulos: Administrador ve todos (incluido Estadísticas); Vendedor ve Ingresos y Clientes; Almacén ve Categorías, Artículos, Ingresos y Proveedores. Estadísticas es exclusiva del Administrador.
- **RF-008** — El sistema debe mostrar una vista inicial distinta según el rol: el Administrador entra al módulo de Estadísticas; los demás entran al panel de indicadores con un saludo de bienvenida.
- **RF-009** — El sistema conserva una ventana principal antigua (tipo MDI, con menús Almacén/Ingresos/Ventas/Acceso/Consultas) con su propia matriz de permisos y confirmación de salida, pero la ruta de entrada real es la ventana nueva (RF-006).
- **RF-010** — El sistema debe mantener en memoria los datos de la sesión (usuario, rol, nombre) y registrarlos en cada operación; los usuarios no administradores ven en los análisis solo sus propios datos.

## 3.3 Categorías (M-CAT)

- **RF-011** — El sistema debe permitir crear, editar y buscar categorías (nombre obligatorio y descripción) en pestañas de listado y mantenimiento, con búsqueda mientras se se escribe, conteo de registros y edición por doble clic en una fila.
- **RF-012** — El sistema debe permitir activar, desactivar o eliminar varias categorías a la vez seleccionando sus casillas, siempre pidiendo confirmación antes de ejecutar y refrescando el listado después.

## 3.4 Artículos (M-ART)

- **RF-013** — El sistema debe permitir gestionar artículos con categoría (solo categorías activas en el selector), código, nombre, precio de venta, stock, descripción e imagen; no se guarda si faltan los campos obligatorios y al editar sin elegir imagen nueva se conserva la anterior.
- **RF-014** — El sistema debe generar una vista previa del código de barras (estándar CODE128) del código del artículo y permitir guardarlo como imagen PNG en la carpeta que elija el usuario.
- **RF-015** — El sistema debe permitir cargar la imagen del artículo (formatos jpeg, jpe o png) con vista previa, y activar, desactivar o eliminar artículos en lote con confirmación. Si la imagen ya no existe en disco, la vista previa queda vacía sin impedir editar el artículo.
- **RF-016** — El sistema debe buscar artículos por código de barras exacto al agregarlos a un ingreso o a una venta, mostrando un mensaje claro cuando el código no corresponde a ningún artículo.

## 3.5 Proveedores y Clientes (M-PER)

- **RF-017** — El sistema debe gestionar proveedores y clientes con nombre y documento obligatorios, validando documentos dominicanos (cédula o DNI de 11 dígitos; pasaporte: una letra y 7 dígitos), teléfono con prefijo 809, 829 o 849, correo electrónico válido y dirección de 5 a 150 caracteres; los datos inválidos bloquean el guardado con mensaje específico y foco en el campo.
- **RF-018** — El sistema debe buscar proveedores y clientes mientras se escribe, contar los registros, permitir la edición por doble clic y eliminar en lote con confirmación.
- **RF-019** — El sistema debe ofrecer cuadros de diálogo para elegir un proveedor o un cliente desde los formularios de ingresos y ventas, rellenando automáticamente sus datos al hacer doble clic.
- **RF-020** — El sistema debe ofrecer un catálogo de artículos activos para elegir productos desde ingresos y ventas, devolviendo al documento el identificador, código, nombre y precio del artículo elegido.

## 3.6 Usuarios y Roles (M-USU)

- **RF-021** — El sistema debe gestionar usuarios con rol (de la lista de roles), nombre, tipo y número de documento, dirección, teléfono, correo y contraseña; aplica las mismas validaciones dominicanas que el RF-017, y la contraseña es obligatoria al crear y opcional al editar (vacío = conservar la actual).
- **RF-022** — El sistema debe impedir registrar dos usuarios con el mismo correo electrónico, avisando "ya existe un usuario con ese email"; al editar solo se verifica si el correo cambió.
- **RF-023** — El sistema debe permitir activar, desactivar o eliminar usuarios en lote con confirmación, mostrando el estado de cada usuario en el listado.
- **RF-024** — El sistema debe mostrar la lista de roles (con conteo de registros) en modo solo lectura: el formulario de roles no permite agregar, modificar ni eliminar roles.

## 3.7 Ingresos / compras (M-ING)

- **RF-025** — El sistema debe registrar ingresos (compras) con proveedor elegido con selector, usuario de la sesión, tipo, serie y número de comprobante, impuesto, total y detalle de artículos (cantidad entera, precio e importe = precio × cantidad), sin guardar si faltan datos obligatorios y advirtiendo si la cantidad no es entera.
- **RF-026** — El sistema debe agregar los artículos al ingreso escaneando el código de barras (tecla Enter) o eligiéndolos del catálogo, rechazando con mensaje los artículos duplicados.
- **RF-027** — El sistema debe listar y buscar ingresos, ver el detalle por doble clic (con subtotal, ITBIS y total recalculados) y anular ingresos en lote con confirmación; lo anulado se marca como "Anulado" (no se borra) y queda fuera de los reportes.

## 3.8 Ventas y facturación (M-VEN)

- **RF-028** — El sistema debe registrar ventas con cliente elegido con selector, usuario de la sesión, comprobante, impuesto, total y detalle con descuento por línea (importe = precio × cantidad − descuento); valida descuento entre 0 y el importe y cantidad entera, y calcula subtotal, ITBIS y total redondeado a 2 decimales en pesos (RD$).
- **RF-029** — El sistema debe agregar líneas de venta por código de barras o catálogo sin duplicados, ver el detalle por doble clic y anular ventas en lote con confirmación; las anuladas quedan excluidas de los análisis.
- **RF-030** — El sistema debe imprimir o reimprimir facturas, tickets y boletas, pero no guías (las guías no generan factura y el botón queda bloqueado con su mensaje); toda impresión se previsualiza antes y la reimpresión lleva el sello "REIMPRESIÓN".
- **RF-031** — El sistema debe emitir la factura con formato dominicano: encabezado con nombre, RNC y dirección de la empresa; título según tipo (FACTURA = "Comprobante de Crédito Fiscal"; TICKET y BOLETA = "Comprobante de Consumo"); caja con el NCF (serie + número); fecha, vendedor, cliente y documento del cliente (etiqueta "Cédula" si tiene 11 dígitos, "RNC" si tiene 9); tabla de productos; subtotal, ITBIS % y total; leyenda fiscal, firma del vendedor y "Gracias por su compra", en moneda RD$ y papel tamaño Carta.
- **RF-032** — El sistema debe usar los datos de la empresa (RNC, nombre y dirección) como constantes editables por el programador; cambiarlas aplica a todas las facturas futuras.

## 3.9 Dashboard analítico (M-DASH)

- **RF-033** — El sistema debe ofrecer en el panel de inicio filtros de fecha (por defecto: últimos 30 días), vendedor, categoría, granularidad (día o mes) y métrica (monto o cantidad), con actualización automática opcional (apagada, 5, 15, 30 o 60 minutos), botón de actualizar, marca de "última actualización" e indicadores de carga mientras se recuperan los datos.
- **RF-034** — El sistema debe mostrar en el panel cuatro gráficos: tarjetas de indicadores (KPI), tendencia de ventas, productos más vendidos y distribución por categoría; en el panel actual solo las tarjetas KPI están activas, y el análisis completo con los cuatro gráficos vive en el módulo de Estadísticas.
- **RF-035** — El sistema debe limitar el panel al alcance del rol: los no administradores solo ven sus propios datos y no pueden habilitar el filtro de vendedor; el Administrador puede elegir "Todos" o un vendedor.

## 3.10 Estadísticas (M-EST)

- **RF-036** — El sistema debe mostrar el módulo de Estadísticas con 6 pestañas —KPIs (ventas totales, ingresos totales, ticket promedio, número de transacciones, variación vs. período anterior y margen bruto), Tendencia, Top Productos, Distribución, Ventas por Período y Stock Bajo— cargándolas en paralelo y de forma independiente: un fallo en una pestaña no tumba las demás, y el botón de recarga indica "Cargando…" mientras dura la carga.
- **RF-037** — El sistema debe filtrar las estadísticas por rango de fechas con presets (Hoy, Esta semana, Este mes, Este año y Personalizado), categoría, vendedor, granularidad (día, semana, mes o año) y métrica (monto o cantidad); rechaza los rangos cuya fecha final es anterior a la inicial y normaliza valores inválidos.
- **RF-038** — El sistema debe alertar los artículos con stock bajo (umbral por defecto 5 unidades; un umbral negativo se corrige a 5), con filtro opcional por categoría y estado "Bajo" o "Crítico" por artículo.
- **RF-039** — El sistema debe permitir exportar los gráficos a imagen PNG o documento PDF con nombre sugerido que incluye la pestaña y la fecha/hora; el PDF incluye los filtros aplicados; donde aún no hay gráficos exportables, informa la limitación en lugar de fallar.
- **RF-040** — El sistema debe calcular las métricas solo con documentos no anulados, con series de tiempo continuas (sin huecos de días o meses), top 10 de productos ordenable por monto o cantidad, distribución porcentual por categoría sobre las ventas del período, ticket promedio, variación contra el período anterior del mismo tamaño y margen bruto (venta menos costo promedio de compra).

# 4. Requisitos no funcionales

- **RNF-001 (plataforma)** — El sistema debe ejecutarse como aplicación de escritorio de Windows sobre .NET Framework 4.8 (Windows Forms), con todas sus bibliotecas incluidas en el proyecto (no requiere instalar dependencias externas en la máquina).
- **RNF-002 (base de datos)** — El sistema debe usar SQL Server 2016 o superior con la base `mydatabase2`, autenticación de Windows por defecto y conexión de confianza al servidor local.
- **RNF-003 (rendimiento)** — El sistema debe responder sin bloquear la interfaz: todo el acceso a datos es asíncrono, los reportes y filtros se cargan en paralelo, y se muestran indicadores de carga (esqueletos y botón "Cargando…") mientras se esperan los datos.
- **RNF-004 (usabilidad)** — Toda la interfaz debe estar en español dominicano, con tipografía Century Gothic, colores corporativos consistentes, layout responsivo, conteos de registros, mensajes uniformes y confirmaciones antes de operaciones destructivas.
- **RNF-005 (accesibilidad y UX)** — El sistema debe facilitar el uso: ventanas sin borde arrastrables, revelado temporal de la contraseña, edición por doble clic, foco automático en el campo con error y mensajes amigables ante errores de captura.
- **RNF-006 (seguridad implementada)** — El sistema debe controlar el acceso por rol en la interfaz y en los datos, bloquear usuarios inactivos, impedir correos duplicados, proteger las claves con hash, exigir claves fuertes y usar códigos de recuperación de 6 dígitos con expiración de 10 minutos enviados por correo con cifrado SSL.
- **RNF-007 (debilidades observadas — riesgos, no requisitos)** — Se documentan como riesgos: credencial de correo electrónico guardada en el código fuente, una rama de conexión con usuario `sa` sin contraseña, sesión global sin tiempo de expiración y mensajes de error que muestran detalles técnicos al usuario.
- **RNF-008 (moneda y localización)** — El sistema debe manejar pesos dominicanos (RD$), cultura es-DO, fechas en formato dd/MM/yyyy, importes redondeados a 2 decimales y el ITBIS como impuesto parametrizable por documento.
- **RNF-009 (mantenibilidad)** — El código debe mantenerse en tres capas (Datos, Negocios, Entidad) más la presentación, con fachadas por módulo, componentes reutilizables y los procedimientos almacenados versionados en la carpeta de scripts SQL.
- **RNF-010 (fiabilidad)** — El sistema debe validar en las tres capas (interfaz, negocio y base de datos), corregir de forma defensiva datos inválidos (fechas invertidas, granularidad o métrica inválida, umbral negativo), aislar los errores por pestaña y liberar los recursos de impresión y temporizadores.
- **RNF-011 (impresión)** — El sistema debe imprimir en papel tamaño Carta con márgenes de 40 unidades, paginación real y encabezado de la empresa repetido desde la segunda página.
- **RNF-012 (identidad visual)** — El sistema debe mostrar el logo del panel principal en escala de grises puros sobre el panel azul (tarea LOGO-BN-001, cerrada con QA aprobado).

# 5. Glosario

- **BD:** base de datos (SQL Server en este sistema).
- **Cédula:** documento de identidad dominicano de 11 dígitos (formato 000-0000000-0).
- **CODE128:** estándar de código de barras usado para identificar artículos.
- **CRUD:** crear, leer, actualizar y eliminar registros.
- **Dashboard:** panel principal con indicadores y gráficos de gestión.
- **Dona:** tipo de gráfico circular usado para mostrar distribución porcentual.
- **Hash:** técnica que transforma la contraseña en un código irreversible para guardarla segura en la base de datos.
- **ITBIS:** impuesto dominicano sobre el consumo aplicado a las ventas (parametrizable por documento).
- **KPI:** indicador clave de desempeño (por ejemplo, ventas totales o ticket promedio).
- **MDI:** estilo antiguo de ventana principal con varios formularios hijos dentro (interface legada).
- **NCF:** Numérico de Control Fiscal; comprobante fiscal dominicano con serie y número (por ejemplo, FACTURA, TICKET, BOLETA, GUÍA).
- **PDF / PNG:** formatos de exportación de gráficos e informes.
- **Período anterior:** el rango inmediatamente anterior al seleccionado, del mismo tamaño, usado para calcular variaciones.
- **RNC:** Registro Nacional de Contribuyente; identificador fiscal de 9 dígitos de empresas (impreso en la factura).
- **SMTP:** protocolo de correo usado para enviar el código de recuperación (servidor Gmail con SSL).
- **SP (procedimiento almacenado):** consulta guardada en la base de datos; todo el acceso a datos del sistema pasa por SPs.
- **Stock:** cantidad disponible de un artículo.
- **Ticket promedio:** venta total dividida entre número de transacciones.
- **WinForms:** tecnología de ventanas de escritorio de .NET usada por el sistema.

# 6. Supuestos y limitaciones

1. Este SRS se derivó de la **lectura estática del código fuente** (ingeniería inversa): no se ejecutó la aplicación, por lo que los criterios de aceptación están inferidos y no verificados de extremo a extremo, salvo la identidad visual (LOGO-BN-001, QA aprobado 7/7).
2. Existen **dos ventanas principales**: la nueva (RF-006) es la vigente y es la que abre el login; la antigua tipo MDI (RF-009) se conserva con permisos ligeramente distintos; en caso de duda manda la ventana nueva.
3. En el panel Dashboard (RF-034) solo las tarjetas KPI están activas hoy; los otros tres gráficos están cableados en el módulo de Estadísticas.
4. La exportación a PNG de estadísticas es actualmente un marcador de posición (dibuja texto indicativo, no el gráfico real); la exportación a PDF sí genera un documento con los filtros aplicados.
5. El rol "Almacén" aparece en el código sin tilde ("Almacen"): se respeta tal cual.
6. La RNF-007 describe debilidades de seguridad observadas (secretos en claro, sesión sin expiración): son **riesgos documentados**, no comportamiento a imitar.
