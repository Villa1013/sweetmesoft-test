# Historial de interacción con la IA — Prueba técnica Mantia

**Herramienta principal (diseño y validación):** Grok (xAI)
**Apoyo de implementación en el repositorio:** opencode — big-pickle
**Fecha de referencia del caso:** 13 de octubre de 2026

> Solo se incluyen interacciones relevantes de esta prueba, en orden cronológico.
>

**Herramientas y modelos usados en esta prueba:**

| Herramienta | Modelo | Uso principal |
|-------------|--------|---------------|
| Grok | xAI | Ideación, validación de alcance/arquitectura, alternativas y redacción |
| opencode | big-pickle | Implementación y refactorización del código en el repositorio |

---

## Sesiones

### [1] — 2026-10-13 18:00 — Grok — xAI — Diagnóstico y alcance
**Contexto aportado:** `Documentation/00_Prueba_Tecnica.pdf`, `01_Contexto_Empresa.pdf`,
`Documentation/solicitudes/*`, `Documentation/tarifas/*`, `02_Historial_Cotizaciones.xlsx`.

**Prompt (original):**
> Leí el caso completo de Mantia. Mi propuesta de alcance es la siguiente:
> - Prototipo local, sin integraciones reales de correo ni mensajería, y sin envío
>   automático de cotizaciones.
> - Backend en .NET (ASP.NET Core Web API + EF Core + SQLite).
> - Frontend en Angular.
> - Motor de extracción **dual**: Google Gemini (opcional) y un motor de reglas locales como
>   respaldo obligatorio, para que nunca se rompa la funcionalidad.
> - El cálculo de precios debe ser **100% determinista**: la IA nunca calcula importes.
> - Laura debe poder revisar y editar antes de marcar una cotización como enviada.
>
> Valídame si este alcance alcanza para demostrar solución al problema y ayúdame a bajarlo a
> entidades y endpoints principales.

**Respuesta (resumen):**
> Lo validó como adecuado para el núcleo del problema y propuso la estructura de entidades
> (Solicitud, ItemSolicitud, Cotizacion, Tarifa) y los endpoints REST básicos.

**Decisión:** aceptado
**Verificación / ajuste:** Mantuve la decisión de **no** usar microservicios ni
autenticación, y confirmé que el cálculo quedaría fuera del alcance de la IA. Contrasté la
propuesta con los PDFs de tarifas e historial antes de fijar el alcance (documentado en
`docs/Solucion_Mantia.md` §2–3).

---

### [2] — 2026-10-13 19:30 — Grok — xAI — Arquitectura y stack
**Contexto aportado:** Notas propias del caso + decisiones de la sesión [1].

**Prompt (original):**
> Quiero una arquitectura simple y auditable, no un sistema distribuido. Mi propuesta:
> - Monolito en **dos capas**: API REST (`MantiaServiceAPI`) y SPA (`MantiaServiceApp`).
> - Backend **.NET 10** + EF Core + **SQLite** (cero instalación, arranca con un comando).
> - Frontend **Angular 20** con **PrimeNG + Tailwind** para una UI clara y rápida de armar.
> - La extracción de texto de PDF se hace en el **cliente** con `pdfjs-dist`; la API solo
>   recibe texto (evita dependencias nativas en el servidor).
> - Descarto microservicios, colas y motores de base de datos pesados (Postgres/SQL Server).
>
> Ayúdame a organizar el proyecto y a justificar estas decisiones para la defensa.

**Respuesta (resumen):**
> Confirmó el enfoque de dos capas y propuso la organización por *features* en Angular y la
> separación `Controllers → Services → Data` en la API, además de un borrador de
> justificación técnica.

**Decisión:** aceptado
**Verificación / ajuste:** Verifiqué que todo corre en local con dos procesos (`dotnet run` +
`ng serve`) y el proxy `/api` (`MantiaServiceApp/proxy.conf.json`). La justificación del stack
quedó en `README.md` (sección *Herramientas usadas*) y en `docs/Solucion_Mantia.md` §9.

---

### [3] — 2026-10-13 21:00 — Grok — xAI — Modelo de datos, DTOs y endpoints
**Contexto aportado:** Entidades que propuse + reglas comerciales preliminares.

**Prompt (original):**
> Modelé el dominio así y quiero que me ayudes a detallarlo:
> - `Solicitud` (1)—(N) `ItemSolicitud`: datos interpretados, estado y campos faltantes.
> - `Solicitud` (1)—(N) `Cotizacion`: una solicitud puede tener revisiones.
> - `Cotizacion` (1)—(N) `LineaCotizacion`: líneas con precio y valor.
> - `Tarifa`: catálogo por `Version` (2026-01 / 2026-10) y `Codigo`, con `Tipo`
>   (Servicio/Recargo/Desplazamiento) y `PrecioUnitarioCOP`.
> - Montos con `decimal(18,2)`; borrado en cascada para ítems/líneas y `Restrict` en
>   cotización→solicitud para conservar historial.
>
> Propón los DTOs y los endpoints para solicitudes, cotizaciones y tarifas.

**Respuesta (resumen):**
> Entregó el borrador de DTOs y el mapa de endpoints REST (`analizar`, registrar,
> actualizar, generar cotización, marcar enviada, listar tarifas).

**Decisión:** modificado
**Verificación / ajuste:** Ajusté nombres de campos y relaciones para que coincidieran con
los datos reales del caso. Quedó implementado en `Models/`, `DTOs/` y las migraciones
`20261014040921_Initial` y `20261014052729_AgregarCamposAdicionales`.

---

### [4] — 2026-10-13 22:30 — Grok — xAI — Reglas comerciales y cálculo determinista
**Contexto aportado:** `Documentation/tarifas/*.pdf` + `02_Historial_Cotizaciones.xlsx`.

**Prompt (original):**
> Las reglas de negocio las derivo yo de los documentos, y quiero que el cálculo sea
> determinista y auditable (nada de IA para los importes):
> - Dos vigencias de tarifa según la **fecha de visita**: `2026-01` y `2026-10`.
> - **IVA 19%**.
> - **Recargo del 25%** solo sobre los **servicios** (nunca sobre el desplazamiento) cuando
>   la atención es fin de semana o fuera del horario 8:00–18:00.
> - **Desplazamiento** fijo por municipio (Bogotá / Chía), **un solo cobro** por visita.
> - La palabra "**urgente**" **no** genera recargo.
> - **Redondeo** a peso entero y **validez de 15 días**.
> - Visitas fuera de vigencia de tarifas no se cotizan.
>
> Ayúdame a escribir `ReglasComerciales` y `CotizacionCalculator` con exactamente estas
> reglas, para que los totales coincidan al peso con el historial.

**Respuesta (resumen):**
> Generó el primer borrador de `ReglasComerciales` y `CotizacionCalculator`.

**Decisión:** modificado
**Verificación / ajuste:** Ajusté los precios unitarios y el orden de aplicación del recargo
y el IVA, y validé manualmente los 6 totales del historial (`COT-001`…`COT-006`) contra el
XLSX hasta que coincidieran. Confirmé que el recargo no aplica al desplazamiento y que
"urgente" no genera recargo (a propósito).

---

### [5] — 2026-10-14 01:00 — opencode — big-pickle — Motor de extracción dual
**Contexto aportado:** `MantiaServiceAPI/` (dominio y tarifas) + las 4 comunicaciones del caso.

**Prompt (original):**
> Voy a implementar el motor de extracción con esta estrategia:
> 1. Una interfaz `IExtraccionTexto` (y `IExtraccionService`).
> 2. Implementación con **Gemini** (opcional, si hay API key).
> 3. Implementación con **reglas locales** (obligatoria como respaldo).
> 4. Un orquestador que intente Gemini y, si falla o no hay API key, use reglas.
>
> Reglas de negocio que el motor debe respetar:
> - Ascensor u otro trabajo no listado → `RequiereValoracionTecnica`.
> - Reparación de aire sin ficha técnica → `RequiereValoracionTecnica`.
> - Si hay corrección de cantidad ("son 4, no 3"), usar la **última**.
> - El presupuesto indicado por el cliente **nunca** fija el precio.
> - Clasificar en `ListaParaCotizar` / `FaltaInformacion` / `RequiereValoracionTecnica`.

**Respuesta (resumen):**
> Implementó la estructura dual (interfaz + dos implementaciones + orquestador
> `ExtraccionService`) respetando las guardas indicadas.

**Decisión:** aceptado
**Verificación / ajuste:** Probé el flujo con y sin API key: sin key siempre resuelve el
motor de reglas; con key, si Gemini falla, cae a reglas sin romper la respuesta. Código en
`Services/Extraccion/*`.

---

### [6] — 2026-10-14 01:45 — opencode — big-pickle — Prompt de Gemini y reglas locales por patrones
**Contexto aportado:** `GeminiExtraccionService` y `ReglasExtraccionService` en desarrollo.

**Prompt (original):**
> Para Gemini quiero **JSON estricto** y baja variabilidad: fuerza
> `responseMimeType: application/json` y `temperature: 0`, y dale el catálogo de tarifas
> (AA-01, AA-02, PI-01, PL-01, EL-01) dentro del prompt. Para el motor local, normaliza el
> texto (quitar tildes, pasar a minúsculas) y detecta cliente, contacto, ubicación, email,
> referencia, agenda e ítems/estado por patrones.

**Respuesta (resumen):**
> Entregó el prompt base con salida JSON y el conjunto de expresiones regulares para el motor
> local.

**Decisión:** modificado
**Verificación / ajuste:** Ajusté las reglas de fecha y de cantidad de aires para los
casos reales. Documenté la limitación conocida: si el texto trae varias fechas (encabezado
del correo + fecha solicitada), la regla puede tomar la primera; Gemini lo mitiga y el
campo es editable.

---

### [7] — 2026-10-14 02:15 — opencode — big-pickle — Verificación del cálculo
**Contexto aportado:** API en ejecución + `Documentation/02_Historial_Cotizaciones.xlsx`.

**Prompt (original):**
> Calculé a mano los importes esperados de cada solicitud y de los casos de borde. Ayúdame a
> compararlos contra las respuestas reales de la API y a dejar la evidencia en los documentos.

**Respuesta (resumen):**
> Consistió los resultados y dejó la tabla de verificación (subtotal, IVA, total) para cada
> solicitud y los casos de recargo.

**Decisión:** aceptado
**Verificación / ajuste:** Los totales coinciden con el caso: SOL-001 **487.900**, SOL-002
**2.225.300**; SOL-003 y SOL-004 devuelven **HTTP 422** con motivos (valoración técnica).
Recargo fin de semana → **600.950**; recargo fuera de horario → **2.763.775**. Evidencia en
`README.md` (sección *Verificación*) y `docs/Solucion_Mantia.md` §8.

---

### [8] — 2026-10-14 02:45 — opencode — big-pickle — Seed de datos de ejemplo
**Contexto aportado:** Solicitudes originales + historial XLSX + tarifas.

**Prompt (original):**
> Necesito el `DataSeeder` con:
> - Las **4 solicitudes** del caso ya interpretadas según mis reglas (incluida la corrección
>   de 3 a 4 equipos en Colegio Arboleda).
> - Las **6 cotizaciones históricas** con los importes exactos del Excel.
> - Las **14 tarifas** de ambas vigencias.
>
> Al levantar la API los datos de prueba deben estar listos y coincidir con el caso.

**Respuesta (resumen):**
> Generó el `DataSeeder` completo (tarifas, solicitudes y cotizaciones históricas).

**Decisión:** aceptado
**Verificación / ajuste:** Ejecuté el seed y contrasté los totales con
`02_Historial_Cotizaciones.xlsx`: coinciden. Código en `Data/DataSeeder.cs`.

---

### [9] — 2026-10-14 03:15 — opencode — big-pickle — Frontend Angular e ingesta de archivos
**Contexto aportado:** Backend funcionando + estructura de features que definí.

**Prompt (original):**
> Para el frontend quiero esta estructura:
> - **Panel** de pendientes con indicadores.
> - **Listado y detalle de solicitudes** (con edición de estado/observaciones).
> - **Listado y detalle de cotizaciones** (con impresión y marcado como enviada).
> - **Consulta de tarifas** por versión.
> - **Carga de texto/archivo**: `.txt` y `.pdf`, extrayendo el texto del PDF en el cliente
>   con `pdfjs-dist`.

**Respuesta (resumen):**
> Propuso la organización por *features* y los servicios HTTP (`SolicitudesService`,
> `CotizacionesService`, `TarifasService`, `PdfService`).

**Decisión:** aceptado con ajustes
**Verificación / ajuste:** Ajusté la UI y los estados de carga. `npx ng build` compila
correctamente. El canal PDF se resuelve en el navegador (`core/services/pdf.service.ts`) y la
API solo recibe texto.

---

### [10] — 2026-10-14 03:45 — Grok — xAI — Alternativas, limitaciones y cierre
**Contexto aportado:** Código actual + `docs/Solucion_Mantia.md` en borrador.

**Prompt (original):**
> Ya tengo el backend y el frontend funcionando. Ayúdame a redactar las secciones de
> **"Alternativas consideradas y descartadas"** y **"Limitaciones"** del documento de defensa,
> manteniendo el tono de que **yo** definí el alcance y descarté explícitamente: cálculo por
> IA, microservicios/colas y extracción de PDF en el servidor.

**Respuesta (resumen):**
> Entregó un borrador de ambas secciones y un cierre para el documento.

**Decisión:** modificado
**Verificación / ajuste:** Adapté la redacción para reflejar con claridad mis decisiones de
diseño y mis descartes. Quedó en `docs/Solucion_Mantia.md` §10–11.

---

## Resumen de aportes de la IA

| # | Aporte | Decisión | Cómo se verificó |
|---|--------|----------|------------------|
| 1 | Validación del alcance y del prototipo local | Aceptado | Contraste con los PDFs del caso |
| 2 | Arquitectura de dos capas y stack (.NET 10 / Angular 20 / SQLite) | Aceptado | Ejecución local con proxy `/api` |
| 3 | Entidades, DTOs y endpoints REST | Modificado | Ajuste a los datos reales + migraciones EF Core |
| 4 | `ReglasComerciales` y `CotizacionCalculator` | Modificado | Recálculo manual vs. `02_Historial_Cotizaciones.xlsx` |
| 5 | Motor de extracción dual con respaldo por reglas | Aceptado | Pruebas con y sin API key |
| 6 | Prompt de Gemini (JSON estricto, `temperature 0`) y reglas por patrones | Modificado | Ajuste de reglas de fecha/cantidad |
| 7 | Verificación de totales y casos de borde | Aceptado | Coincidencia al peso + HTTP 422 por valoración |
| 8 | `DataSeeder` (tarifas, 4 solicitudes, 6 cotizaciones) | Aceptado | Contraste contra el XLSX |
| 9 | Organización de features Angular e ingesta `.txt`/PDF | Aceptado con ajustes | `ng build` correcto; PDF en cliente |
| 10 | Texto de alternativas y limitaciones | Modificado | Redactado para reflejar mis decisiones |
| — | Cálculo de precios por IA | Descartado | Decisión mía desde el inicio (cálculo determinista) |
| — | Microservicios / colas | Descartado | Sobredimensionado para el alcance |
| — | Extracción de PDF en el servidor | Descartado | `pdfjs-dist` en cliente es suficiente |
| — | Autenticación/roles | Descartado | Uso local por una sola persona |

**Nota final.** La IA se usó como apoyo técnico para implementar y refinar decisiones que yo
tomé sobre alcance, arquitectura, reglas de negocio y criterios de calidad. No se delegó el
diseño de la solución ni el cálculo de los importes.
