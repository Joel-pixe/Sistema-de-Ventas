# Fase 3 — Plan concreto: BUG-UI-001 Botones pegados en FrmEstadisticas

**Fecha:** 2026-10-13
**Responsable:** manager
**Alcance aprobado:** `Sistema (SEMANA 11)/WindowsFormsApp1/FrmEstadisticas.cs` (únicamente).

## 1. Diagnóstico (problema detectado)

En `FrmEstadisticas.BuildUI()`, la celda de acciones (columna 9 de `tlp`) monta un `Panel pnlAcciones`
(`Dock = Fill`) con tres controles dockeados: `BtnActualizar` (Top, h=34), `BtnExportar` (Top, h=34,
Margin top=6) y `LblEstado` (Bottom, h=22).

Cálculo del alto realmente disponible para `pnlAcciones`:

| Concepto | px |
|---|---|
| `pnlTop.Height` | 110 |
| − `pnlTop.Padding` (8 arriba + 8 abajo) | −16 → 94 |
| − `Margin` de la celda del `TableLayoutPanel` (2+2) | −4 → 90 |
| − `Label` vacío de `CrearCampo("", …)` (Dock=Top, h=20) | −20 → **70 px** |

Alto exigido por los controles dockeados: 34 (ACTUALIZAR) + 6 (Margin de BtnExportar) + 34 (EXPORTAR)
+ 22 (LblEstado) = **96 px**.

**Resultado:** faltan 26 px. WinForms acomoda los controles dockeados igualmente y estos quedan
encimados/recortados contra `LblEstado` → los botones se ven "muy pegados" y con aspecto bug.
Agravantes:

1. `BtnActualizar` no define `Margin` (default 3 px) y `BtnExportar` usa `Margin = (0,6,0,0)` →
   separación inconsistente (3 px a un lado, 6 al otro, 0 horizontal).
2. El `Label` de `CrearCampo` tiene `Text = ""` pero consume 20 px inútiles.
3. El orden de docking depende del orden de `Controls.Add` (frágil, implícito).

## 2. Objetivo / Criterios de aceptación

- CA-1: ACTUALIZAR y EXPORTAR quedan claramente separados (≥4 px) y sin solaparse con `LblEstado`.
- CA-2: ACTUALIZAR visible arriba, EXPORTAR debajo, `LblEstado` al fondo (orden explícito, no implícito).
- CA-3: El layout no se desborda a alturas/DPI normales: alto exigido ≤ alto disponible con margen.
- CA-4: El proyecto compila sin errores.
- CA-5: No se modifica ningún archivo fuera de `FrmEstadisticas.cs`; no se cambia lógica de negocio
  ni comportamiento de los handlers (`CargarTodoAsync`, `BtnExportar_Click`).

## 3. Estrategia elegida

Reemplazar el docking implícito de `pnlAcciones` por un `TableLayoutPanel` con **filas de altura
explícita** y eliminar el label vacío de la celda; subir 10 px la barra superior como colchón.

### Cambios concretos (en `BuildUI()`)

1. **`pnlTop.Height`: 110 → 120** (colchón de 10 px; es un panel `Dock=Top`, el `TabControl` se adapta).
2. **Celda de acciones sin label:** sustituir `tlp.Controls.Add(CrearCampo("", pnlAcciones), 9, 0)`
   por un panel propio (`Dock=Fill`, `Margin=(4,2,4,2)`, `Padding=(4,0,0,0)`) que contiene el TLP de
   acciones. Recupera los 20 px del label vacío → alto disponible = 120−16−4 = **100 px**.
3. **`pnlAcciones` deja de contener controles dockeados** y pasa a alojar un `TableLayoutPanel`
   `tlpAcciones` (`Dock=Fill`, 1 col %100, 3 filas):
   - Fila 0: `SizeType.Absolute, 34` → `BtnActualizar`
   - Fila 1: `SizeType.Absolute, 34` → `BtnExportar`
   - Fila 2: `SizeType.Percent, 100` (32 px) → `LblEstado`
4. **Botones: `Dock = DockStyle.Fill` en su celda con `Margin = new Padding(0, 2, 0, 2)`**
   → 30 px visibles cada uno y **4 px de separación** entre ambos (2 px del borde inferior de la
   fila 0 + 2 px del borde superior de la fila 1).
5. **`LblEstado`: `Dock = DockStyle.Fill`** en la fila restante (32 px para un label de 22 px, con
   holgura).
6. `pnlAcciones.Controls` deja de usarse para acomodar: se agrega solo `tlpAcciones`.

Presupuesto final: exigido 34+34+32 = 100 px ≤ disponible 100 px, con los márgenes internos del TLP
ya contemplados en cada fila → sin desbordamiento.

## 4. Pasos de ejecución

1. Editar `FrmEstadisticas.cs` aplicando los cambios 1-6 de la sección 3 (solo `BuildUI()`).
2. Compilar `Sistema.Presentacion.csproj` → 0 errores.
3. Revisión visual/estática: verificar presupuesto de píxeles y orden de controles.
4. Persistir `04-implementacion.md` y `05-qa.md`.

## 5. Riesgos

| Riesgo | Mitigación |
|---|---|
| Cambiar `Dock=Top` a `Dock=Fill` altera el alto de los botones | Las filas son absolutas de 34 px; con Margin 2+2 el botón queda en 30 px (legible para "ACTUALIZAR"/"EXPORTAR" a Century Gothic 9 Bold) |
| `BtnActualizar.Text = "Cargando..."` durante la carga | No se toca la lógica; el texto se sigue cambiando en `CargarTodoAsync` |
| Alto disponible justo (100 px) | Colchón de +10 px en `pnlTop` y label de estado con 10 px de holgura |

## 6. Fuera de alcance (NO hacer)

- No tocar `UcDashboard.cs` (mismo patrón pero con un solo botón, no reportado).
- No tocar otros formularios (`FrmArticulo`, `FrmCategoria`, etc.).
- No refactorizar `CrearCampo` ni la barra de filtros más allá de la celda de acciones.
- No cambiar handlers, datos, ni estilos de colores.

⬅ HANDOFF → developer
- **Artefacto:** `.opencode/pipeline/03-plan.md`
- **Resumen ejecutivo:** Bug de desbordamiento de layout en la celda de acciones de `FrmEstadisticas` (96 px exigidos vs 70 px disponibles); plan: filas explícitas en un `TableLayoutPanel`, +10 px de colchón y label vacío eliminado.
- **Decisiones tomadas:** Tabla de filas absolutas 34/34/%100; Margin (0,2,0,2) en ambos botones; solo se toca `FrmEstadisticas.cs`.
- **Alcance / qué NO hacer:** no tocar otros formularios ni lógica de handlers.
- **Bloqueos o dudas abiertas:** ninguno.
- **Siguiente paso sugerido:** developer implementa los cambios 1-6 → `04-implementacion.md`.
