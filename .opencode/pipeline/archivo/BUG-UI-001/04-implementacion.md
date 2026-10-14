# Fase 4 — Implementación: BUG-UI-001 Botones pegados en FrmEstadisticas

**Fecha:** 2026-10-13
**Responsable:** developer
**Plan de referencia:** `.opencode/pipeline/03-plan.md` (sección 3, cambios 1-6)
**Alcance:** `Sistema (SEMANA 11)/WindowsFormsApp1/FrmEstadisticas.cs` (únicamente), solo dentro del método `BuildUI()`.

## 1. Archivos modificados

| Archivo | Cambio |
|---|---|
| `Sistema (SEMANA 11)/WindowsFormsApp1/FrmEstadisticas.cs` | Único archivo tocado. Solo el método `BuildUI()`. |

**No se modificó ningún otro archivo.** No se tocaron handlers, helpers (`CrearCampo`, `CrearFecha`, `CrearCombo`, `CrearSkeleton`), pestañas del `TabControl` ni otros formularios.

## 2. Cambios aplicados (según plan §3)

1. **`pnlTop.Height` 110 → 120** (línea ~74). Se conservó intacto el `Padding = new Padding(10, 8, 10, 8)`.

2. **Celda de acciones sin label:** se eliminó `tlp.Controls.Add(CrearCampo("", pnlAcciones), 9, 0)` (que consumía 20 px con un label vacío) y se sustituyó por un panel propio (líneas ~191-198):
   ```csharp
   Panel celdaAcciones = new Panel
   {
       Dock = DockStyle.Fill,
       Margin = new Padding(4, 2, 4, 2),
       Padding = new Padding(4, 0, 0, 0)
   };
   celdaAcciones.Controls.Add(pnlAcciones);
   tlp.Controls.Add(celdaAcciones, 9, 0);
   ```

3. **`pnlAcciones` con `TableLayoutPanel` de filas explícitas** (líneas ~132-189): `pnlAcciones` ahora es `Dock = Fill` sin controles dockeados directos; aloja `tlpAcciones` (1 col %100, 3 filas: Absolute 34 / Absolute 34 / Percent 100, `Margin` y `Padding` = `Empty`).
   - `BtnActualizar`: `Dock=Fill`, `Margin=(0,2,0,2)` en fila 0. Conserva texto, colores (DodgerBlue/blanco), `FlatStyle.Flat`, `BorderSize=0`, fuente Century Gothic 9 Bold, `Cursor=Hand` y evento `Click += async (s,e) => await CargarTodoAsync()`.
   - `BtnExportar`: `Dock=Fill`, `Margin=(0,2,0,2)` en fila 1. Conserva todo lo visual (MediumSeaGreen/blanco, etc.) y `Click += BtnExportar_Click`. Se eliminó el `Margin=(0,6,0,0)` y `Height=34` originales.
   - `LblEstado`: pasa de `Dock=Bottom, Height=22` a `Dock=Fill` en fila 2. Conserva texto "Listo", fuente 7.5F, ForeColor Gray, `TextAlign=MiddleLeft`.
   - Se eliminaron las tres líneas `pnlAcciones.Controls.Add(LblEstado/BtnExportar/BtnActualizar)`; ahora solo `pnlAcciones.Controls.Add(tlpAcciones);`.

## 3. Verificación ejecutada

**(a) Sintaxis C# válida — ✅ confirmado por compilación real.** Releído `BuildUI()` completo: llaves y paréntesis balanceados; los `using` necesarios (`System.Drawing` línea 6, `System.Windows.Forms` línea 8) ya estaban presentes; no se añadieron dependencias.

**(b) Sin referencias huérfanas — ✅ confirmado por grep sobre el archivo:**
- Cero coincidencias de `CrearCampo("", …)`.
- La única coincidencia de `pnlAcciones.Controls.Add` es la nueva `…(tlpAcciones)`.
- Las dos únicas coincidencias de `DockStyle.Top` son el `pnlTop` (correcto, debe seguir Top) y el `Label` interno de `CrearCampo` (intacto).
- Cero coincidencias de `Height = 110`.

**(c) Presupuesto de píxeles — ✅:**
- Disponible: `120 − 16 (Padding 8+8) − 4 (Margin celda 2+2) = 100 px` (el label vacío de 20 px ya no resta porque la celda usa `celdaAcciones`, no `CrearCampo`).
- Exigido: `34 (fila ACTUALIZAR) + 34 (fila EXPORTAR) + %100 (fila estado = 32) = 100 px`.
- 100 ≤ 100 → sin desbordamiento. Separación real entre botones = 2+2 = **4 px** (CA-1); orden ACTUALIZAR arriba / EXPORTAR debajo / LblEstado al fondo vía índice de fila explícito (CA-2).

**(d) CA-4 (compilación): ✅ — MSBuild ejecutado con éxito.**
```
MSBuild.exe "Sistema (SEMANA 11)\WindowsFormsApp1\Sistema.Presentacion.csproj" /t:Build /p:Configuration=Debug
→ Sistema.Presentacion -> ...\bin\Debug\WindowsFormsApp1.exe
→ 0 errores, 4 warnings CS0169 preexistentes en FrmIngresos.cs/FrmVenta.cs (ajenos, no introducidos por este cambio)
```

**(e) CA-5 (alcance): ✅** — `git status`/búsqueda de edición confirman que solo se editó `FrmEstadisticas.cs`, únicamente en `BuildUI()`; handlers y lógica de negocio sin cambios.

## 4. Limitaciones / pendientes

- La compilación estática y el presupuesto de píxeles están verificados, pero **la validación visual en runtime (CA-1/CA-2/CA-3 a distintos DPI) queda para QA**: no se ejecutó la aplicación en este entorno. El tester debe abrir el módulo Estadísticas y confirmar el aspecto de los botones y `LblEstado`.
- Los 4 warnings CS0169 son preexistentes y fuera de alcance.

### ⬅ HANDOFF → tester
- **Artefacto:** `.opencode/pipeline/04-implementacion.md`
- **Resumen ejecutivo:**
  - BUG-UI-001 implementado en `FrmEstadisticas.cs` → solo cambió `BuildUI()`: `pnlTop.Height` 110→120, celda de acciones sin label vacío (`celdaAcciones`) y `tlpAcciones` con filas explícitas 34/34/%100.
  - Botones ahora `Dock=Fill` con `Margin=(0,2,0,2)` → 4 px de separación; `LblEstado` al fondo con `Dock=Fill`.
  - Presupuesto de píxeles: 100 px disponibles = 100 px exigidos (antes: 70 vs 96).
  - Compilación con MSBuild VS 18 → **0 errores** (`WindowsFormsApp1.exe` generada); 4 warnings CS0169 preexistentes en otros archivos.
  - Verificado sin referencias huérfanas (`CrearCampo("")`, `Controls.Add` viejos y `Height=110` eliminados).
- **Decisiones tomadas:** implementación literal de `03-plan.md` §3 (cambios 1-6); colores, fuentes, FlatStyle, BorderSize y handlers conservados idénticos; `Padding=(4,0,0,0)` movido de `pnlAcciones` a `celdaAcciones` tal como indica el plan.
- **Alcance / qué NO hacer:** no tocar otros formularios, `UcDashboard.cs`, helpers (`CrearCampo`, etc.) ni handlers; no refactorizar más allá de la celda de acciones; no cambiar estilos/colores.
- **Bloqueos o dudas abiertas:** ninguno.
- **Siguiente paso sugerido:** QA en `05-qa.md`: verificación visual en runtime (separación de botones, `LblEstado` visible, sin solapamiento, a DPI normales) contra CA-1/CA-2/CA-3.
