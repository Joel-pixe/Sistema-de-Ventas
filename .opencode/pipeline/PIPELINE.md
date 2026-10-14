# PIPELINE — Sistema SEMANA 11

**Índice de estado vivo del ciclo de vida.** Único archivo de gestión: lo escribe solo el `manager`.

## Fase activa

- **Tarea actual (sigue en curso):** LOGO-BN-001 — Cambiar el logo del dashboard (menú lateral, módulo de Estadísticas) por una versión en blanco y negro (escala de grises pura).
- **Estado LOGO-BN-001:** 🔁 **Fase 5 en revisión** — QA inicial APROBADO 7/7; el usuario facilitó la credencial del Administrador para **repetir el CA-6 con login real (E2E)**
- **Tarea nueva (atendida en este turno):** DOC-REQ-001 — Listar todos los requerimientos funcionales y no funcionales identificados del sistema ya realizado (ingeniería inversa).
- **Estado DOC-REQ-001:** ✅ **Fase 1 completada** — artefacto `01-requisitos.md` persistido (40 RF + 12 RNF). No hay cambio funcional propuesto; no avanza a fase 2 salvo que el usuario lo pida.
- **Tarea nueva (este turno):** DOC-SRS-001 — SRS sencillo + PDF `requisitos.pdf` en raíz.
- **Estado DOC-SRS-001:** ✅ **Fase 4+5 completadas y archivadas** — `Docs/Requisitos_SRS.md` (20 KB) + `requisitos.pdf` (10 pág, 101 KB), QA 4/4 PASS. Artefactos de pipeline archivados en `archivo/DOC-SRS-001/` para no pisar los `03/04/05-*.md` de LOGO-BN-001 (sigue en Fase 5 revisión §8 E2E pendiente).
- **Tarea nueva (este turno):** GIT-PUSH-001 — Subir todo el proyecto al repo `https://github.com/Joel-pixe/Sistema-de-Ventas.git`.
- **Estado GIT-PUSH-001:** ✅ **completado** — remoto `origin` creado, commit `622dedc` (17 archivos), rama local renombrada a `main`, `git push -u origin main` OK (nueva rama `main -> main`). Se respeta `.gitignore`: `Sistema (SEMANA 11).zip` (118 MB) y `mydatabase.bak` quedan fuera del push.
- **Fecha:** 2026-10-14

## Estado de fases

| Fase | Artefacto | Estado | Fecha |
|------|-----------|--------|-------|
| 0 — Diagnóstico | `00-diagnostico.md` | ⬜ omitida justificada: el manager hizo el diagnóstico directamente (búsqueda de logo + análisis de píxeles del PNG); no era necesario un barrido del sistema | 2026-10-13 |
| 1 — Requisitos | `01-requisitos.md` | ✅ completado DOC-REQ-001 (2026-10-13): 40 RF + 12 RNF por ingeniería inversa. Nota histórica LOGO-BN-001: para esa tarea la fase 1 se omitió justificadamente (requisito único y claro, ver §Decisiones) | 2026-10-13 |
| 2 — Arquitectura | `02-arquitectura.md` | ⬜ omitida justificada: es el cambio de un asset + 1 línea de resx; no hay arquitectura que diseñar. Las 4 alternativas de implementación con trade-offs están analizadas en `03-plan.md` §3 | 2026-10-13 |
| 3 — Plan concreto | `03-plan.md` | ✅ completado (alternativa B elegida) | 2026-10-13 |
| 4 — Implementación | `04-implementacion.md` | ✅ completado (build 0 errores; CA-1/2/3/4/5/7 PASS) | 2026-10-13 |
| 5 — QA | `05-qa.md` | 🔁 en revisión (pase 1: APROBADO 7/7; pase 2 E2E con login real en curso) | 2026-10-13 |

**Tarea anterior (cerrada):** BUG-UI-001 — botones ACTUALIZAR/EXPORTAR en `FrmEstadisticas`. QA 100% verde. Sus artefactos fueron archivados en `.opencode/pipeline/archivo/BUG-UI-001/` (`03-plan.md`, `04-implementacion.md`, `05-qa.md`) para liberar los nombres estándar del pipeline.

## Decisiones del usuario

- 2026-10-13: **LOGO-BN-001** — El usuario pide cambiar el logo del dashboard del módulo de estadísticas por una versión en blanco y negro.
  - **Aspecto elegido:** *"Grises puro (sin recuadro)"* — escala de grises real (quita el tinte azul/cian actual), **sin recuadro y sin cambiar layout**. Descartadas: "negro sobre fondo blanco" y "blanco sobre fondo negro".
  - **Alcance elegido:** *"Solo ese logo"* (`PicLogo` en `FrmDashBoard`, el único logo de la app). **No** se convierten los iconos del menú ni de la barra de título.
  - El usuario fue informado de que el cambio es sutil (el logo actual ya es casi blanco: promedio R219/G226/B229) y lo aceptó.
- 2026-10-13: **LOGO-BN-001** — El usuario facilita la **contraseña del Administrador** para que el QA repita el CA-6 con **login real (E2E)**, en lugar del harness de arranque usado en el pase 1. La credencial **no se persiste** en ningún artefacto ni archivo del repo.
- 2026-10-13 *(precedente, tarea cerrada)*: BUG-UI-001 — arreglar botones pegados de `FrmEstadisticas`; alcance solo `FrmEstadisticas.cs`.
- 2026-10-13: **DOC-REQ-001** — El usuario pide listar todos los RF y RNF identificables del sistema ya realizado. Se resuelve por Fase 1 (analista, lectura estática, sin ejecución). Sin decisión de diseño ni cambio de código.
- 2026-10-13: **DOC-SRS-001** — El usuario pide SRS sencillo + PDF `requisitos.pdf`. Plan manager: analista reescribe SRS legible → developer genera `Docs/Requisitos_SRS.md` + `requisitos.pdf` (fpdf2 2.8.9 + Arial, aprobado) → tester QA 4/4. Sin tocar `.cs`/`.csproj`/`.resx`/`App.config`. Estándares `03/04/05-*.md` siguen siendo de LOGO-BN-001; lo de DOC-SRS-001 se archiva en `archivo/DOC-SRS-001/`.
- 2026-10-14: **GIT-PUSH-001** — El usuario pide subir todo el proyecto a `https://github.com/Joel-pixe/Sistema-de-Ventas.git`. Decisiones del usuario: rama destino = **`main`** (local renombrada con `branch -M`); **se excluyen** `*.zip`/`*.bak` según `.gitignore` vigente (el zip de 118 MB supera el límite de 100 MB de GitHub). Tarea operativa: se justifican fases 1-2 omitidas (sin requisitos ni arquitectura que diseñar); ejecución directa del manager (commit + push), sin artefacto `01/02/03/04/05` nuevo para no pisar los de LOGO-BN-001.

## Bloqueos

- Ninguno.

## Registro de handoffs

| Fecha | De → A | Artefacto | Resumen |
|-------|--------|-----------|---------|
| 2026-10-13 | usuario → manager | — | Petición: logo del dashboard del módulo de estadísticas en blanco y negro |
| 2026-10-13 | manager (diagnóstico) | `03-plan.md` | Único logo = `PicLogo` en `FrmDashBoard.Designer.cs:349`; PNG 192×173 con alfa, blanco con tinte cian sobre panel `DodgerBlue`; 1 solo uso en todo el repo |
| 2026-10-13 | usuario → manager | — | Decisiones: aspecto = "grises puro (sin recuadro)"; alcance = "solo ese logo" |
| 2026-10-13 | manager | `03-plan.md` | Plan LOGO-BN-001: alternativa **B** (PNG `-bn` nuevo + ruta en resx + `<None>` en csproj; ningún `.cs` se toca). 7 CA definidos |
| 2026-10-13 | manager → developer | `03-plan.md` → `04-implementacion.md` | PNG `-bn` generado (grises puros, alfa idéntico), resx y csproj ajustados; build **0 errores**; CA-1/2/3/4/5/7 PASS; cero `.cs` tocados |
| 2026-10-13 | manager → tester | `03-plan.md` + `04-implementacion.md` → `05-qa.md` | QA: re-verificar CA-1..5/7 y el **CA-6 visual** (logo gris neutro sobre DodgerBlue, sin recuadro, mismo layout) |
| 2026-10-13 | tester → manager | `05-qa.md` | **PASE 1: APROBADO 7/7** — CA-6 con harness de arranque (sin credenciales); 0 bugs atribuibles; repo limpio |
| 2026-10-13 | usuario → manager → tester | `05-qa.md` (§8) | Credencial del Administrador entregada para **repetir CA-6 con login real E2E**; credencial no persistida en disco |
| 2026-10-13 | *(histórico)* manager → developer → tester | `archivo/BUG-UI-001/*` | BUG-UI-001 cerrado: build 0 errores, QA aprobado 100% verde |
| 2026-10-13 | usuario → manager → analista | `01-requisitos.md` | DOC-REQ-001: petición de inventario RF/RNF del sistema construido; analista hace ingeniería inversa (solo lectura) |
| 2026-10-13 | analista → manager | `01-requisitos.md` | DOC-REQ-001 entregado: 40 RF (RF-001..RF-040) + 12 RNF (RNF-001..RNF-012), roles, historias, trazabilidad; artefacto persistido, listo para reporte |
| 2026-10-13 | usuario → manager | — | DOC-SRS-001: pide SRS sencillo + PDF `requisitos` en el proyecto |
| 2026-10-13 | manager → analista | `01-requisitos.md` | DOC-SRS-001: reescribir 40 RF + 12 RNF como SRS sencillo "El sistema debe…" sin tecnicismos + glosario |
| 2026-10-13 | manager → developer | `01-requisitos.md` → `Docs/Requisitos_SRS.md` + `requisitos.pdf` | DOC-SRS-001 Fase 4: MD 20 KB + PDF 10 pág 101 KB (fpdf2 + Arial), cero código tocado |
| 2026-10-13 | manager → tester | `Docs/` + `requisitos.pdf` | DOC-SRS-001 Fase 5 QA documental (4 chequeos) |
| 2026-10-13 | tester → manager | `archivo/DOC-SRS-001/05-qa.md` | DOC-SRS-001 **APROBADO 4/4**: MD 40/40+12/12, PDF 10 pág sin tofu, pie OK, cero código tocado |
| 2026-10-14 | usuario → manager | — | GIT-PUSH-001: petición de subir todo el proyecto a `https://github.com/Joel-pixe/Sistema-de-Ventas.git` |
| 2026-10-14 | manager | — | GIT-PUSH-001: `remote add origin` + `add -A` + commit `622dedc` (17 archivos, 1094+/39-) + `branch -M main` + `push -u origin main` OK; `PIPELINE.md` actualizado |
