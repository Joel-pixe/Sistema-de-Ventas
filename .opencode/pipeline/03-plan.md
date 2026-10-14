# Fase 3 — Plan: LOGO-BN-001 — Logo del dashboard en blanco y negro

**Fecha:** 2026-10-13
**Responsable:** manager
**Solicitante:** usuario
**Artefactos aguas arriba:** ninguno (fases 1 y 2 omitidas justificadas — ver `PIPELINE.md`)
**Tarea anterior:** BUG-UI-001 cerrada; sus artefactos quedaron en `.opencode/pipeline/archivo/BUG-UI-001/`

---

## 1. Requisito (confirmado con el usuario el 2026-10-13)

- Cambiar el logo del dashboard (menú lateral visible con el módulo de Estadísticas abierto) por una **versión en blanco y negro**.
- **Aspecto elegido por el usuario:** *"Grises puro (sin recuadro)"* — escala de grises real que neutraliza el tinte azul/cian actual, **sin cambiar el layout** (mismo tamaño, misma posición, mismo panel `DodgerBlue`).
- **Alcance elegido por el usuario:** *"Solo ese logo"* — únicamente `PicLogo`. No tocar iconos de los botones del menú, iconos de la barra de título ni otros formularios.
- El usuario fue informado de que el cambio es **sutil** (el logo actual ya es casi blanco) y aceptó esta opción.

## 2. Diagnóstico (fase 0 — ejecutada por el manager con lectura directa del código y del PNG)

| Elemento | Dato |
|---|---|
| Control | `PicLogo` (`System.Windows.Forms.PictureBox`) en `FrmDashBoard` — `Sistema (SEMANA 11)/WindowsFormsApp1/FrmDashBoard.Designer.cs:349` |
| Recurso usado | `global::WindowsFormsApp1.Properties.Resources.Screenshot_2026_07_17_192320_removebg_preview` |
| Definición en resx | `Properties/Resources.resx:133-134` → `..\Resources\Screenshot_2026-07-17_192320-removebg-preview.png;System.Drawing.Bitmap, ...` |
| Declaración en csproj | `Sistema.Presentacion.csproj:339` → `<None Include="Resources\Screenshot_2026-07-17_192320-removebg-preview.png" />` |
| Archivo fuente | `Sistema (SEMANA 11)/WindowsFormsApp1/Resources/Screenshot_2026-07-17_192320-removebg-preview.png` |
| Imagen | 192×173 px, `Format32bppArgb` (fondo transparente, logo tipo silueta "Sistema de Ventas") |
| Colores actuales | Píxeles opacos promedio **R219 / G226 / B229** → blanco con tinte cian; existen regiones cian claro (G,B ≥ 252 y R 160-251) |
| Fondo del panel | `PnlMenuVertical.BackColor = System.Drawing.Color.DodgerBlue` (#1E90FF) — `FrmDashBoard.Designer.cs:63` |
| Usos en todo el repo | **1 solo** (la línea 349 de `FrmDashBoard.Designer.cs`); verificado por grep de `Screenshot_...removebg_preview` |
| Imágenes del proyecto | Solo 5 PNG: este logo + 4 iconos de la barra de título. **No existe ninguna versión B/N en el repo** |

**Hallazgo clave:** como el logo es casi blanco, una conversión literal a grises produce un cambio *sutil* (blanco → gris neutro ≈224 y desaparece el tinte azul). El usuario lo fue informado y eligió igualmente esta opción (frente a "negro sobre fondo blanco" y "blanco sobre fondo negro", que sí requerirían un recuadro y tocar layout).

## 3. Alternativas de implementación evaluadas

| # | Alternativa | Archivos tocados | Pros | Contras |
|---|---|---|---|---|
| A | Sobrescribir el PNG original con su versión grayscale | 1 binario | Mínimo riesgo, cero ediciones de texto | El original en color desaparece del working tree (solo recuperable vía git) |
| **B — ELEGIDA** | **Nuevo PNG `...-preview-bn.png` + apuntar el `<value>` del resx al archivo nuevo** | 1 binario nuevo + 1 línea en `Resources.resx` + 1 línea en `.csproj` | **Conserva el original**; el *nombre del recurso no cambia* → `Resources.Designer.cs` y `FrmDashBoard.Designer.cs` **no se tocan** | El nombre físico del archivo deja de coincidir con el nombre del recurso |
| C | Recurso nuevo con nombre nuevo (`..._bn`) | `Resources.resx` + `Resources.Designer.cs` + `FrmDashBoard.Designer.cs` | Nombre explícito y auto-documentado | Toca un archivo **auto-generado** (VS lo regenera) y el Designer del form: más riesgo por menos beneficio |
| D | Aplicar grayscale en runtime con `ColorMatrix` | 1-2 archivos `.cs` | Sin tocar assets | Código extra, costo de render, y el diseñador WinForms no refleja el efecto |

**Decisión del manager: alternativa B.** Es la única que cumple a la vez: conservar el asset original en color, no modificar ningún `.cs`, y no tocar archivos auto-generados. La alternativa A queda como **plan de respaldo** (mismo resultado visual) si la edición del resx presentara algún problema.

> No se consultó al usuario sobre este punto por ser un detalle técnico sin impacto visual (ambas conservan el resultado vía git); la decisión de diseño relevante —aspecto y alcance— sí fue tomada por el usuario en la §1.

## 4. Pasos concretos (ejecuta `developer`)

1. **Generar el asset en escala de grises** con PowerShell + `System.Drawing` (sin instalar dependencias):
   - Abrir el PNG original (`Format32bppArgb`).
   - Por cada píxel: `g = round(0.299·R + 0.587·G + 0.114·B)` (luminancia Rec.601) y escribir `Color.FromArgb(A_original, g, g, g)`.
   - **El canal alfa se conserva píxel a píxel** (es lo que mantiene la silueta sobre el panel azul).
   - Guardar como **PNG** 32bppArgb en:
     `Sistema (SEMANA 11)/WindowsFormsApp1/Resources/Screenshot_2026-07-17_192320-removebg-preview-bn.png`
   - **Prohibido JPEG** (destruiría la transparencia).
2. **Editar `Properties/Resources.resx`** — solo la línea 134 (`<value>` del data `Screenshot_2026-07-17_192320-removebg-preview`): la ruta pasa a `..\Resources\Screenshot_2026-07-17_192320-removebg-preview-bn.png`, **conservando intacto** el sufijo de tipo `;System.Drawing.Bitmap, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`.
3. **Editar `Sistema.Presentacion.csproj`** — junto a la línea 339, agregar `<None Include="Resources\Screenshot_2026-07-17_192320-removebg-preview-bn.png" />`. El `<None>` del original **se conserva** (el archivo sigue en disco).
4. **NO tocar ningún `.cs`**: ni `Resources.Designer.cs`, ni `FrmDashBoard.Designer.cs`, ni `FrmDashBoard.cs`, ni nada más.
5. **Compilar:**
   ```
   MSBuild.exe "Sistema (SEMANA 11)\WindowsFormsApp1\Sistema.Presentacion.csproj" /t:Build /p:Configuration=Debug
   ```
   Esperado: **0 errores** (los 4 warnings CS0169 preexistentes en `FrmIngresos.cs`/`FrmVenta.cs` se ignoran: son ajenos).

## 5. Riesgos y mitigación

| Riesgo | Mitigación |
|---|---|
| Perder la transparencia → logo con fondo blanco sobre el panel | Conservar alfa píxel a píxel y **verificar** que el histograma de alfa del nuevo PNG idéntico al del original |
| Romper el XML de `Resources.resx` / `.csproj` | Editar solo el valor del `<value>` y agregar un `<None>`; el build es la verificación (resgen fallaría si la ruta está mal) |
| Modificar sin querer un archivo auto-generado | CA-4 verifica con `git status` que **cero `.cs`** fueron modificados |
| Cambiar el original por accidente | CA-5 verifica por hash que el PNG original sigue byte a byte idéntico |

## 6. Criterios de aceptación (CA)

- **CA-1:** Existe `Resources/Screenshot_2026-07-17_192320-removebg-preview-bn.png`, 192×173 px, y **todos** sus píxeles cumplen `R = G = B` (escala de grises pura); su canal alfa es **idéntico** píxel a píxel al del original.
- **CA-2:** `Properties/Resources.resx` apunta al archivo `-bn` y `Sistema.Presentacion.csproj` lo declara como `<None Include=...>`; el nombre del recurso y `Resources.Designer.cs` **no cambiaron**.
- **CA-3:** Compilación MSBuild con **0 errores**.
- **CA-4 (alcance):** `git status` confirma como únicos cambios: PNG nuevo + `Resources.resx` + `Sistema.Presentacion.csproj`. **Cero archivos `.cs` modificados.**
- **CA-5 (conservación):** El PNG original en color sigue en disco con el **mismo hash** que antes del cambio.
- **CA-6 (visual):** Al abrir el dashboard, `PicLogo` muestra el logo en **gris neutro** (sin tinte azul) sobre el panel DodgerBlue, con idéntico tamaño (250×207 del control, imagen 192×173) y posición — **sin recuadro y sin cambios de layout**.
- **CA-7 (verificación fuerte, opcional):** Cargar el `WindowsFormsApp1.exe` compilado por reflexión y leer el recurso `Screenshot_2026_07_17_192320_removebg_preview`, confirmando que el bitmap embebido tiene `R=G=B`. Esto prueba de extremo a extremo que el runtime recibirá la versión B/N.

## 7. Fuera de alcance (NO hacer)

- Iconos del `imageList1` (`estadisticas.png`, `ventas.png`, `Clientes.png`, etc.) e iconos de la barra de título (`icon cerrar2`, `icon maximizar`, `icon minimizar`, `icon restaurar`).
- `FrmLogin`, `FrmArticulo`, `FrmDashboardHost`, `FrmEstadisticas` y cualquier otro formulario.
- Cualquier cambio de layout, color de panel, tamaño de control o lógica C#.
- Instalación de dependencias o nuevas librerías.
