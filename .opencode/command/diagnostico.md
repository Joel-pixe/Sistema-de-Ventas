---
description: Ejecuta una auditoría completa del sistema de ventas con el subagente experto-sistema-ventas
agent: build
---

Ejecuta una auditoría completa del proyecto actual invocando al subagente `experto-sistema-ventas` con la siguiente tarea:

$ARGUMENTS

Si el usuario no especificó nada, realiza una auditoría general: compila la solución, re-verifica todos los fallos conocidos listados en tu base de conocimiento y busca fallos nuevos. Devuelve el informe en el formato obligatorio (sección 4 de tu prompt), clasificado por severidad 🔴🟠🟡 con `archivo.cs:línea`. No edites ningún archivo.

Al terminar, resume en el chat principal los hallazgos más graves (máximo 10 líneas) y ofrece al usuario escalar los fixes planificados.
