# DOC-SRS-001 — Implementación (archivada para no pisar LOGO-BN-001)

**Fecha:** 2026-10-13
**Tarea:** Crear SRS sencillo + PDF `requisitos.pdf` en raíz.

## Entregables (fuente de verdad en disco, no aquí)
- `Docs/Requisitos_SRS.md` (20.2 KB) — SRS legible completo: portada, introducción, descripción general, RF-001..RF-040, RNF-001..RNF-012, glosario, supuestos.
- `requisitos.pdf` (raíz, minúsculas, 103.531 bytes, 10 páginas, %PDF-1.3) — mismo contenido con portada, encabezados por módulo, pie "Sistema SEMANA 11 — SRS v1.0".

## Cómo se generó
- Python 3.14.5 vía `py`, `fpdf2 2.8.9` (pip, aprobado en el prompt), fuente Arial TTF del sistema (DejaVu no instalada).
- Script en `%TEMP%\opencode\gen_srs_pdf.py` (fuera del repo); PDF generado desde el .md como fuente única.
- Verificación developer: header %PDF-1.3, 10 páginas, >10 KB, sin warnings de glifos, contenido 1:1 con 01-requisitos.md, cero `.cs`/`.csproj`/`.resx`/config tocados.

## Decisiones
- fpdf2 + Arial en vez de reportlab / PdfSharp (no disponibles).
- Markdown como fuente única.

### ⬅ HANDOFF → manager (fase 4)
- **Artefacto:** `.opencode/pipeline/archivo/DOC-SRS-001/04-implementacion.md` (archivado aquí para no pisar `04-implementacion.md` de LOGO-BN-001)
- **Resumen ejecutivo:** DOC-SRS-001 completado; Docs/Requisitos_SRS.md + requisitos.pdf (10 pág, 101 KB); Unicode OK; cero código tocado
- **Decisiones tomadas:** fpdf2 + Arial; fuente única MD
- **Alcance / qué NO hacer:** solo Docs + PDF; no tocar código ni pipeline
- **Bloqueos o dudas abiertas:** ninguno
- **Siguiente paso sugerido:** QA verifica PDF
