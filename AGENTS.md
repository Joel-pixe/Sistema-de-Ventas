# Protocolo de integración del equipo — Ciclo de vida lineal

Este proyecto trabaja con un **pipeline lineal por artefactos compartidos**. El resultado de cada fase **no se pierde** en la sesión del subagente (que arranca con contexto fresco): se persiste en disco y el agente siguiente **lee el documento original**, no un resumen del manager.

## Pipeline (orden del ciclo de vida)

| Fase | Agente | Artefacto | El resultado llega a |
|------|--------|-----------|----------------------|
| 0 — Diagnóstico (opcional) | `experto-sistema-ventas` | `.opencode/pipeline/00-diagnostico.md` | manager + analista |
| 1 — Requisitos | `analista` | `.opencode/pipeline/01-requisitos.md` | manager + arquitecto |
| 2 — Arquitectura | `arquitecto` | `.opencode/pipeline/02-arquitectura.md` | manager + developer |
| 3 — Plan concreto | `manager` | `.opencode/pipeline/03-plan.md` | manager + developer |
| 4 — Implementación | `developer` | `.opencode/pipeline/04-implementacion.md` | manager + tester |
| 5 — QA | `tester` | `.opencode/pipeline/05-qa.md` | manager (reporte final al usuario) |

**Índice de estado vivo:** `.opencode/pipeline/PIPELINE.md`
`explore` y `general` son de apoyo puntual: **no** generan artefacto ni actualizan el pipeline.

## Reglas de ENTRADA (todos los agentes, incluidos subagentes)

1. Al empezar una tarea, lee `.opencode/pipeline/PIPELINE.md` (fase activa, decisiones ya tomadas por el usuario, bloqueos).
2. Lee los artefactos de las fases anteriores que apliquen, **en orden del ciclo de vida** (`00 → 01 → 02 → 03 → 04 → 05`). Son la **fuente de verdad**: el prompt que te pasa el manager solo encuadra la tarea.
3. Si un artefacto falta o contradice otra fuente, **dilo explícitamente y detente** antes de trabajar. Nunca supongas el contenido de una fase anterior.

## Reglas de SALIDA (todos los agentes)

1. Tu respuesta final debe contener **tu entregable completo** (no un resumen), listo para guardarse en tu artefacto, y terminar con este bloque, con esta forma exacta:

```markdown
### ⬅ HANDOFF → <agente o fase siguiente>
- **Artefacto:** `.opencode/pipeline/NN-nombre.md`
- **Resumen ejecutivo:** 3-6 bullets
- **Decisiones tomadas:**
- **Alcance / qué NO hacer:**
- **Bloqueos o dudas abiertas:** ninguno | lista
- **Siguiente paso sugerido:**
```

2. **Ningún subagente escribe en `.opencode/pipeline/`.** Quien persiste es el **manager** (único con permiso de edición sobre el pipeline) y él actualiza `PIPELINE.md`.

## Responsabilidades del manager

- Persistir el entregable recibido en el artefacto de su fase.
- Actualizar `PIPELINE.md`: estado (⬜ pendiente / 🟡 en curso / ✅ completado / 🔁 en revisión / ❌ bloqueado), fecha, decisiones del usuario y registro de handoffs.
- **Incluir siempre en el prompt de cada `subagent` la ruta de los artefactos aguas arriba** que el agente debe leer. Ejemplo:

> Lee `.opencode/pipeline/PIPELINE.md` y `.opencode/pipeline/01-requisitos.md`. Diseña 2-3 alternativas respetando esos requisitos y el alcance. Termina con el bloque HANDOFF.

## Reglas estrictas del pipeline

- El orden es **lineal**: no se salta de fase sin que el manager lo justifique en `PIPELINE.md`.
- Todo cambio de alcance o decisión del usuario se anota en `PIPELINE.md` **antes** de continuar.
- Ningún trabajo se da por terminado sin: artefacto persistido + `PIPELINE.md` actualizado + verificación (build/tests cuando aplique).
