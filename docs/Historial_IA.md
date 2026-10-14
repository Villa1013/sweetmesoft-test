# Historial de interacción con la IA — Prueba técnica Mantia

> **Estado:** plantilla + instrucciones. Completa cada entrada con tus mensajes reales de
> esta prueba (prompts y respuestas), **en orden cronológico**. La prueba pide el historial
> original; este archivo solo lo estructura para que sea legible en el repositorio.

## Reglas de esta entrega

1. **Solo esta prueba.** No incluyas conversaciones de otros trabajos.
2. **Orden cronológico** de prompts y respuestas, incluidas iteraciones y correcciones.
3. **Identifica** la herramienta y el modelo cuando estén disponibles
   (p. ej. "opencode — big-pickle", "ChatGPT — GPT-5", "Claude — Sonnet 4.5").
4. **Adjunta contexto** cuando sea necesario entender la interacción (archivo o fragmento
   aportado). Puedes pegar el texto o referenciar archivos del repo.
5. **Oculta credenciales.** Nunca pegues API keys, tokens ni contraseñas.
6. Si una herramienta **no permite exportar**, conserva los mensajes visibles en texto o
   capturas ordenadas e **identifica qué parte no pudiste registrar**.
7. Cierra con un resumen de qué aportes **aceptaste, modificaste o descartaste** y cómo los
   **verificaste** (esto se resume también en `Solucion_Mantia.md`, sección 12).

### Cómo completar cada entrada

Usa este bloque por cada turno:

```
### [N] — <fecha/hora> — <herramienta> — <modelo>
**Contexto aportado:** <archivos o fragmentos que pasaste>
**Prompt (original):**
> <texto literal del prompt>
**Respuesta (original):**
> <texto literal de la respuesta, o resumen fiel indicándolo>
**Decisión:** aceptado | modificado | descartado
**Verificación / ajuste:** <qué comprobaste y qué cambiaste>
```

---

## Sesiones

> Sustituye los ejemplos siguientes por tus turnos reales. Dejé 3 entradas de muestra con
> la forma esperada; bórralas o reemplázalas.

### [1] — 2026-10-13 21:00 — opencode — big-pickle
**Contexto aportado:** `Documentation/00_Prueba_Tecnica.pdf`, `01_Contexto_Empresa.pdf`,
`Documentation/tarifas/*.pdf`, `Documentation/solicitudes/*`, `02_Historial_Cotizaciones.xlsx`.
**Prompt (original):**
> Resume el caso de Mantia, identifica el problema central y propone un alcance de prototipo
> que se pueda implementar y justificar.
**Respuesta (original):**
> <pegar aquí la respuesta recibida, o un resumen fiel indicándolo>
**Decisión:** aceptado
**Verificación / ajuste:** contrasté la propuesta con los documentos de tarifas y con el
historial de cotizaciones antes de fijar el alcance.

### [2] — 2026-10-13 22:30 — opencode — big-pickle
**Contexto aportado:** tarifas 2026-01 y 2026-10, historial de cotizaciones.
**Prompt (original):**
> Deriva las reglas comerciales (IVA, desplazamiento, recargo, versión de tarifa, validez) y
> propón cómo modelarlas en código determinista.
**Respuesta (original):**
> <pegar aquí la respuesta recibida>
**Decisión:** modificado
**Verificación / ajuste:** recalculé a mano COT-001…COT-006 y los comparé con el XLSX; ajusté
el recargo para que no aplicara al desplazamiento y el "urgente" para que no generara recargo.

### [3] — 2026-10-14 00:15 — opencode — big-pickle
**Contexto aportado:** `MantiaServiceApi/`, `MantiaServiceApp/`.
**Prompt (original):**
> Revisa la extracción de las 6 comunicaciones y verifica los cálculos de las cotizaciones.
**Respuesta (original):**
> <pegar aquí la respuesta recibida>
**Decisión:** modificado
**Verificación / ajuste:** detecté que el motor de reglas toma la fecha del encabezado del
correo en lugar de la fecha de visita; lo documenté como limitación y dejé Gemini como
motor principal cuando hay API key.

---

## Resumen de aportes (completar)

| # | Aporte | Decisión | Cómo se verificó |
|---|--------|----------|------------------|
| 1 | Alcance del prototipo | Aceptado | Contraste con PDFs del caso |
| 2 | Reglas comerciales | Modificado | Recalculo vs. historial XLSX |
| 3 | Extracción de solicitudes | Modificado | Pruebas por API con los 6 archivos |
| … | … | … | … |

## Herramientas y modelos usados en la prueba (completar)

| Herramienta | Modelo | Uso principal |
|-------------|--------|---------------|
| opencode | big-pickle | Diseño, código y documentación |
| … | … | … |

> Si **no** usaste IA, deja una única línea indicándolo expresamente aquí y elimina las
> secciones de sesiones.
