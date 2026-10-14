<div align="center">
![SweetMeSoft](/MantiaServiceApp/public/logo.png)
</div>

# Mantia Servicios — Prototipo de cotización asistida

Prototipo funcional para la **prueba técnica de SweetMeSoft**. Automatiza la lectura de
solicitudes de mantenimiento (correos, mensajería y documentos), su interpretación, el
cálculo de la cotización según las tarifas vigentes y el seguimiento del estado. Reduce el
tiempo de Laura (coordinadora comercial) y los errores de alcance y valores.

- **Backend:** .NET 10 (ASP.NET Core Web API) + Entity Framework Core + SQLite.
- **Frontend:** Angular 20 + TypeScript, PrimeNG 20 + Tailwind CSS 4, `pdfjs-dist`.
- **IA (opcional):** Google Gemini para extracción de datos; si no hay API key o falla,
  la API sigue funcionando con un **motor de reglas local**. Nunca se rompe la funcionalidad.

> Documento de explicación y defensa de la solución: [`docs/Solucion_Mantia.md`](docs/Solucion_Mantia.md)
> (y su versión PDF [`docs/Solucion_Mantia.pdf`](docs/Solucion_Mantia.pdf)).
> Historial de interacción con la IA: [`docs/Historial_IA.md`](docs/Historial_IA.md).

---

## Índice

1. [Arquitectura](#arquitectura)
2. [Requisitos](#requisitos)
3. [Instalación y ejecución](#instalación-y-ejecución)
4. [Variables de entorno](#variables-de-entorno)
5. [Uso del prototipo](#uso-del-prototipo)
6. [API (endpoints)](#api-endpoints)
7. [Reglas de negocio implementadas](#reglas-de-negocio-implementadas)
8. [Datos de ejemplo (seed)](#datos-de-ejemplo-seed)
9. [Verificación](#verificación)
10. [Alcance funcional](#alcance-funcional)
11. [Pendientes y fuera de alcance](#pendientes-y-fuera-de-alcance)
12. [Estructura del repositorio](#estructura-del-repositorio)
13. [Herramientas usadas](#herramientas-usadas)

---

## Arquitectura

```
┌──────────────────────────────┐        HTTP/JSON        ┌──────────────────────────────┐
│  MantiaServiceApp (Angular)  │  /api/**  (proxy dev)   │  MantiaServiceApi (.NET 10)  │
│                              │ ──────────────────────► │                              │
│  Panel / Solicitudes /       │                          │  Controllers                 │
│  Cotizaciones / Tarifas      │ ◄────────────────────── │   ├─ SolicitudesController   │
│                              │                          │   ├─ CotizacionesController  │
│  PDF: pdfjs-dist (cliente)   │                          │   └─ TarifasController       │
└──────────────────────────────┘                          │                              │
        http://localhost:4200                             │  Services                    │
                                                          │   ├─ ExtraccionService       │
                                                          │   │   ├─ GeminiExtraccion... │
                                                          │   │   └─ ReglasExtraccion... │
                                                          │   ├─ CotizacionCalculator    │
                                                          │   └─ ReglasComerciales       │
                                                          │                              │
                                                          │  EF Core ─► SQLite (mantia.db)│
                                                          └──────────────────────────────┘
                                                                 http://localhost:5256
```

- El frontend llama a `/api/**`; en desarrollo Angular lo **proxifica** a la API
  (`MantiaServiceApp/proxy.conf.json` → `http://localhost:5256`).
- La base de datos SQLite (`mantia.db`) **se crea y se siembra automáticamente** al
  arrancar la API (migraciones EF Core + `DataSeeder`). No requiere pasos manuales.
- El canal **PDF** se resolvió en el cliente con `pdfjs-dist`: el archivo se convierte a
  texto en el navegador y ese texto se envía a la API. La API solo recibe texto.

## Requisitos

| Herramienta | Versión usada | Notas |
|-------------|---------------|-------|
| .NET SDK | **10.0** | Compila y ejecuta `MantiaServiceApi` |
| Node.js | **20 LTS o superior** | Usado: 22.x |
| npm | 10+ | Gestor del frontend |

Comprobar:

```bash
dotnet --version   # 10.x
node --version
npm --version
```

## Instalación y ejecución

Se levantan dos procesos: la **API** y la **app Angular**. En dos terminales.

### 1) Terminal A — Backend (API)

```bash
cd MantiaServiceApi
dotnet restore          # restaura paquetes NuGet
dotnet run              # levanta la API en http://localhost:5256
```

Al arrancar:

- Se aplican las migraciones y se **crea `mantia.db`** si no existe.
- Se cargan los **datos de ejemplo** (tarifas, 4 solicitudes y 6 cotizaciones históricas).
- Swagger queda disponible en <http://localhost:5256/swagger> (entorno Development).

> El perfil por defecto (`Properties/launchSettings.json`) usa **http://localhost:5256**,
> que es el puerto al que apunta el proxy del frontend. Si cambias el puerto, actualiza
> también `MantiaServiceApp/proxy.conf.json`.

### 2) Terminal B — Frontend (Angular)

```bash
cd MantiaServiceApp
npm install             # instala dependencias
npm start               # ng serve -> http://localhost:4200
```

Abrir <http://localhost:4200>. El proxy envía `/api/**` a la API en `localhost:5256`.

### Reiniciar con base de datos limpia

`mantia.db` está en `.gitignore`. Para regenerar los datos de ejemplo desde cero:

```bash
# con la API detenida
rm -f MantiaServiceApi/mantia.db MantiaServiceApi/mantia.db-shm MantiaServiceApi/mantia.db-wal
cd MantiaServiceApi && dotnet run
```

## Variables de entorno

La API lee un archivo **`.env` opcional** en `MantiaServiceApi/` (ver
`MantiaServiceApi/.env.example`). Es útil solo si quieres activar Gemini.

```bash
cd MantiaServiceApi
cp .env.example .env
```

| Variable | Descripción | Obligatoria |
|----------|-------------|-------------|
| `GEMINI__APIKEY` | API key de Google AI Studio. Si está vacía, se usa el motor de reglas local. | No |
| `GEMINI__MODEL` | Modelo de Gemini (por defecto `gemini-3.8-flash`). | No |
| `GEMINI__TIMEOUTSECONDS` | Segundos antes de caer al motor de reglas (por defecto 8). | No |

- **No se sube `.env` al repositorio** (está ignorado). No incluyas claves reales en el repo.
- Sin API key, la aplicación funciona igual con el motor de reglas; en los logs verás
  `Motor de extracción: reglas locales`.

## Uso del prototipo

1. **Nueva solicitud** (`/solicitudes/nueva`): pega el texto de un correo/mensaje o carga
   un archivo (`.txt` o `.pdf` según el canal), pulsa **Analizar** y revisa el resultado
   (cliente, ubicación, fecha, ítems y estado propuesto).
   - Si faltan datos, la solicitud queda en *Falta información*.
   - Si es un trabajo no tarifado (p. ej. un ascensor) o una reparación sin ficha, queda en
     *Requiere valoración técnica*.
2. **Registrar** la solicitud para guardarla.
3. En el detalle (**Solicitud**), edita estado/observaciones y pulsa **Generar cotización**.
   La API calcula líneas, subtotal, IVA (19%) y total, y crea un **borrador**.
4. En **Cotizaciones** puedes revisar el detalle, **imprimir** o marcar como **Enviada**
   (no hay envío real al cliente, por diseño).

## API (endpoints)

Base: `http://localhost:5256/api`

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/tarifas?version=` | Lista tarifas activas (opcional filtro por versión). |
| GET | `/solicitudes?estado=` | Lista solicitudes (opcional filtro por estado). |
| GET | `/solicitudes/{id}` | Detalle de una solicitud. |
| POST | `/solicitudes/analizar` | Extrae datos de un texto **sin** guardar. |
| POST | `/solicitudes` | Extrae y **registra** la solicitud. |
| PUT | `/solicitudes/{id}` | Actualiza datos/estado/ítems. |
| GET | `/cotizaciones?solicitudId=` | Lista cotizaciones. |
| GET | `/cotizaciones/{id}` | Detalle de una cotización. |
| POST | `/cotizaciones/solicitudes/{solicitudId}` | Genera un borrador de cotización. |
| POST | `/cotizaciones/{id}/marcar-enviada` | Marca una cotización como enviada. |

Ejemplo:

```bash
curl -s http://localhost:5256/api/tarifas
curl -s -X POST http://localhost:5256/api/solicitudes/analizar \
  -H "Content-Type: application/json" \
  -d '{"texto":"Mantenimiento preventivo de un aire split de 18000 BTU en Bogotá, Calle 120 # 15-20","origen":"Correo"}'
```

## Reglas de negocio implementadas

Derivadas de los documentos de tarifas y del historial de cotizaciones:

- **IVA 19%** sobre servicios, recargos y desplazamiento. La cotización muestra subtotal,
  IVA y total.
- **Desplazamiento por visita a la sede:** Bogotá $30.000 y Chía $60.000 (versión 2026-10).
  Una misma visita tiene un **solo** cobro de desplazamiento. Otros municipios requieren
  confirmación comercial.
- **Recargo del 25%** sobre el valor de los *servicios* cuando la atención es **fin de
  semana** o **fuera del horario** 8:00–18:00. El recargo **no** aplica al desplazamiento.
  La palabra "urgente" **no** genera recargo por sí sola.
- **Versión de tarifas según la fecha de la visita:** `2026-01` (01/01–30/09/2026) o
  `2026-10` (01/10–31/12/2026). Visitas fuera de esas vigencias no se cotizan.
- **Validez de la cotización:** 15 días calendario desde la emisión.
- **Guardas de cotización:** no se cotiza si falta información, si requiere valoración
  técnica, si no hay ítems, o si el municipio no está cubierto.
- El **diagnóstico** no incluye reparación ni repuestos; equipos de otro tipo/capacidad y
  trabajos no listados requieren valoración técnica (p. ej. ascensores).
- Un **presupuesto indicado por el cliente** no fija el precio.

## Datos de ejemplo (seed)

Al primer arranque se cargan:

- **14 tarifas** (5 servicios + desplazamientos por versión `2026-01` y `2026-10`).
- **4 solicitudes** representativas de los canales (correo, mensajería, PDF):
  - `SOL-001` Colegio Arboleda — 4 aires split (corrige de 3 a 4 según mensaje del 13/10).
  - `SOL-002` Edificio Mirador — 120 m² de pintura + 2 griferías (ref. SML-018).
  - `SOL-003` Oficina Horizonte — reparación de aire sin ficha → valoración técnica.
  - `SOL-004` Conjunto Arrayanes — ascensor → valoración técnica.
- **6 cotizaciones históricas** (`COT-001`…`COT-006`) con los importes del historial
  entregado (se conservan a su fecha original).

## Verificación

La solución se verificó con los datos reales del caso. Resultados (motor de reglas,
fecha de emisión 13/10/2026, tarifa `2026-10`):

| Solicitud | Cálculo | Subtotal | IVA | Total |
|-----------|---------|----------|-----|-------|
| SOL-001 Colegio Arboleda | 4 × AA-01 $95.000 + desplazamiento Bogotá $30.000 | 410.000 | 77.900 | **487.900** |
| SOL-002 Edificio Mirador | 120 × PI-01 $14.000 + 2 × PL-01 $65.000 + desplazamiento Chía $60.000 | 1.870.000 | 355.300 | **2.225.300** |
| SOL-003 Oficina Horizonte | Requiere valoración técnica → no cotiza (HTTP 422) | — | — | — |
| SOL-004 Conjunto Arrayanes | Ascensor fuera de tarifas → no cotiza (HTTP 422) | — | — | — |

Casos adicionales comprobados:

- **Recargo fin de semana:** SOL-001 movida al sábado → servicios 380.000, recargo 25%
  95.000, desplazamiento 30.000 → subtotal 505.000 / IVA 95.950 / **total 600.950**.
- **Recargo fuera de horario (19:00–21:00):** servicios 1.810.000, recargo 452.500,
  desplazamiento 60.000 → subtotal 2.322.500 / IVA 441.275 / **total 2.763.775**.
- **Fuera de vigencia de tarifas** (visita en 2027): rechazada (HTTP 422).
- Las 6 cotizaciones históricas del seed **coinciden** con `02_Historial_Cotizaciones.xlsx`.

Comandos útiles de verificación:

```bash
# Build del backend
dotnet build MantiaServiceApi/MantiaServiceAPI.csproj

# Build del frontend
cd MantiaServiceApp && npx ng build
```

## Alcance funcional

- Ingesta de solicitudes desde **texto pegado** o **archivo** (`.txt` y `.pdf`).
- **Extracción** de cliente, contacto, ubicación, referencia, fecha, franja, ítems y estado
  (motor Gemini o reglas locales).
- **Clasificación** en *Lista para cotizar*, *Falta información* o *Requiere valoración
  técnica*, con campos faltantes y observaciones.
- **Registro, listado, filtrado y edición** de solicitudes.
- **Cálculo automático** de la cotización (líneas, IVA, total, versión de tarifa, validez)
  y generación de **borradores**.
- **Listado/detalle de cotizaciones**, historial y marcado como *Enviada*.
- **Panel** con indicadores (listas para cotizar, requieren acción, valor cotizado,
  recientes).
- **Consulta de tarifas** por versión.

## Pendientes y fuera de alcance

- **Integraciones reales** de correo/mensajería: no se conectan cuentas ni se reciben
  mensajes; la entrada es manual (pegar/cargar archivo).
- **Envío real de cotizaciones** al cliente: solo se marca como *Enviada*. "Mantia no
  realiza envíos automáticos sin revisión de Laura".
- **Agenda de técnicos** y **facturación/cobro**: fuera del alcance de la prueba.
- **Motor de reglas:** algunos campos dependen de heurísticas simples. Por ejemplo, la
  **fecha de visita** puede confundirse con la fecha del encabezado del correo cuando
  ambas aparecen (limitación identificada y documentada; el motor Gemini mitiga esto).
- **Autenticación/roles:** el prototipo es de uso local por Laura, sin login.
- **Cobertura de pruebas automatizadas** (unitarias/e2e): no incluida; la verificación se
  hizo por API con los datos del caso.

## Estructura del repositorio

```
.
├── README.md                     # este archivo
├── Documentation/                # materiales de la prueba (entrada ficticia)
├── docs/                         # entregables: solución (md + pdf) e historial de IA
│   ├── Solucion_Mantia.md
│   ├── Solucion_Mantia.pdf
│   └── Historial_IA.md
├── MantiaServiceApi/             # Backend .NET 10
│   ├── Controllers/              # Endpoints REST
│   ├── Services/                 # Extracción y cálculo de cotizaciones
│   │   ├── Extraccion/           # Gemini + reglas locales
│   │   └── Cotizacion/           # CotizacionCalculator + ReglasComerciales
│   ├── Models/  DTOs/  Data/     # Entidades, DTOs, EF Core + DataSeeder
│   ├── Migrations/               # Migraciones EF Core
│   └── .env.example              # Plantilla de variables (Gemini opcional)
└── MantiaServiceApp/             # Frontend Angular 20
    ├── src/app/core/             # Modelos, servicios HTTP, utilidades de UI
    ├── src/app/features/         # Panel, solicitudes, cotizaciones, tarifas
    ├── src/app/shared/           # Componentes/pipes reutilizables
    └── proxy.conf.json           # Proxy /api -> http://localhost:5256
```

## Herramientas usadas

- **.NET 10**, ASP.NET Core, Entity Framework Core (SQLite), Swashbuckle (Swagger).
- **Angular 20**, TypeScript, PrimeNG 20, Tailwind CSS 4, `pdfjs-dist`.
- **Google Gemini** (`v1beta generateContent`) como motor de extracción opcional.
- **IA de apoyo** para desarrollo y redacción (ver `docs/Historial_IA.md`).

> Todos los nombres, comunicaciones y valores son ficticios (caso de prueba de
> SweetMeSoft). No se incluyen credenciales ni claves privadas en el repositorio.
