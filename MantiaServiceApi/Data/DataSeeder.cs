using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services.Cotizacion;

namespace MantiaServiceAPI.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!context.Tarifas.Any())
        {
            await context.Tarifas.AddRangeAsync(Tarifas());
            await context.SaveChangesAsync();
        }

        if (!context.Solicitudes.Any())
        {
            await context.Solicitudes.AddRangeAsync(SolicitudesPrueba());
            await context.SaveChangesAsync();
        }

        if (!context.Cotizaciones.Any())
        {
            await context.Cotizaciones.AddRangeAsync(HistorialCotizaciones());
            await context.SaveChangesAsync();
        }
    }

    private static List<Tarifa> Tarifas() =>
    [
        // Versión 2026-01 — vigencia 01/01/2026 al 30/09/2026
        Servicio("2026-01", "AA-01", "Mantenimiento preventivo de aire acondicionado tipo split hasta 24.000 BTU", "Equipo", 90_000m),
        Servicio("2026-01", "AA-02", "Visita de diagnóstico de aire acondicionado tipo split hasta 24.000 BTU", "Equipo", 70_000m),
        Servicio("2026-01", "PI-01", "Pintura interior de paredes. Materiales suministrados por el cliente", "m² de pared", 13_000m),
        Servicio("2026-01", "PL-01", "Cambio de grifería sencilla. Repuesto suministrado por el cliente", "Pieza", 60_000m),
        Servicio("2026-01", "EL-01", "Revisión de punto eléctrico", "Punto", 45_000m),
        Desplazamiento("2026-01", ReglasComerciales.CodigoDesplazamientoBogota, "Desplazamiento Bogotá", 25_000m),
        Desplazamiento("2026-01", ReglasComerciales.CodigoDesplazamientoChia, "Desplazamiento Chía", 50_000m),

        // Versión 2026-10 — vigencia 01/10/2026 al 31/12/2026
        Servicio("2026-10", "AA-01", "Mantenimiento preventivo de aire acondicionado tipo split hasta 24.000 BTU", "Equipo", 95_000m),
        Servicio("2026-10", "AA-02", "Visita de diagnóstico de aire acondicionado tipo split hasta 24.000 BTU", "Equipo", 75_000m),
        Servicio("2026-10", "PI-01", "Pintura interior de paredes. Materiales suministrados por el cliente", "m² de pared", 14_000m),
        Servicio("2026-10", "PL-01", "Cambio de grifería sencilla. Repuesto suministrado por el cliente", "Pieza", 65_000m),
        Servicio("2026-10", "EL-01", "Revisión de punto eléctrico", "Punto", 50_000m),
        Desplazamiento("2026-10", ReglasComerciales.CodigoDesplazamientoBogota, "Desplazamiento Bogotá", 30_000m),
        Desplazamiento("2026-10", ReglasComerciales.CodigoDesplazamientoChia, "Desplazamiento Chía", 60_000m),
    ];

    private static List<Solicitud> SolicitudesPrueba() =>
    [
        new()
        {
            Numero = "SOL-001",
            FechaRecepcion = new DateTime(2026, 10, 12, 8, 15, 0),
            Cliente = "Colegio Arboleda",
            Contacto = "Ana Beltrán",
            Municipio = "Bogotá",
            Direccion = "Calle 120 # 15-20",
            EmailContacto = "administracion@arboleda.example",
            Origen = OrigenSolicitud.Correo,
            TextoOriginal = """
                Correo 12/10/2026 08:15 + mensaje 13/10/2026 09:10.
                Mantenimiento preventivo de aires split 18.000 BTU en sede principal Bogotá, Calle 120 # 15-20.
                Visita jueves 15 de octubre de 2026, 9:00-12:00. Acceso autorizado.
                Actualización: son 4 equipos, no 3. Misma dirección y misma visita. Discrimina IVA y desplazamiento.
                """,
            Estado = EstadoSolicitud.ListaParaCotizar,
            Observaciones = "El mensaje del 13/10 corrige la cantidad de 3 a 4 equipos. Se cotiza la cantidad actualizada.",
            FechaVisita = new DateTime(2026, 10, 15),
            FranjaHoraria = "09:00 - 12:00",
            FinDeSemana = false,
            FueraDeHorario = false,
            Items =
            [
                new ItemSolicitud
                {
                    CodigoTarifa = "AA-01",
                    Concepto = "Mantenimiento preventivo split hasta 24.000 BTU",
                    Cantidad = 4,
                    Observacion = "Split 18.000 BTU"
                }
            ]
        },
        new()
        {
            Numero = "SOL-002",
            FechaRecepcion = new DateTime(2026, 10, 12, 10, 10, 0),
            Cliente = "Edificio Mirador",
            Contacto = "Santiago Molina",
            Municipio = "Chía",
            Direccion = "Carrera 6 # 8-15",
            EmailContacto = "administracion@mirador.example",
            Origen = OrigenSolicitud.Correo,
            Referencia = "SML-018",
            TextoOriginal = """
                Correo 12/10/2026 10:10 y PDF de soporte SML-018 (10:20).
                Pintar 120 m² de pared interior del salón comunal y cambiar 2 griferías sencillas.
                El edificio suministra pintura y griferías. Una sola visita el 16/10/2026, 8:00-17:00.
                """,
            Estado = EstadoSolicitud.ListaParaCotizar,
            Observaciones = "El PDF SML-018 es soporte de la misma solicitud, no una solicitud nueva.",
            FechaVisita = new DateTime(2026, 10, 16),
            FranjaHoraria = "08:00 - 17:00",
            FinDeSemana = false,
            FueraDeHorario = false,
            Items =
            [
                new ItemSolicitud
                {
                    CodigoTarifa = "PI-01",
                    Concepto = "Pintura interior de paredes. Materiales suministrados por el cliente",
                    Cantidad = 120,
                    Observacion = "120 m² de pared, no de piso"
                },
                new ItemSolicitud
                {
                    CodigoTarifa = "PL-01",
                    Concepto = "Cambio de grifería sencilla. Repuesto suministrado por el cliente",
                    Cantidad = 2,
                    Observacion = "Lavamanos"
                }
            ]
        },
        new()
        {
            Numero = "SOL-003",
            FechaRecepcion = new DateTime(2026, 10, 12, 14, 30, 0),
            Cliente = "Oficina Horizonte",
            Contacto = "Paula Méndez",
            Origen = OrigenSolicitud.Mensajeria,
            TextoOriginal = """
                Mensajería 12/10/2026 14:30. El aire dejó de enfriar y piden que quede funcionando.
                No hay ficha del equipo, ni marca/modelo/BTU, ni dirección de la sede.
                """,
            Estado = EstadoSolicitud.RequiereValoracionTecnica,
            CamposFaltantes = "dirección, municipio, ficha del equipo (tipo, BTU), alcance (diagnóstico vs reparación)",
            Observaciones = "La reparación no está en tarifa. AA-02 es diagnóstico y no incluye reparación ni repuestos. 'Urgente' no genera recargo.",
            FinDeSemana = false,
            FueraDeHorario = false
        },
        new()
        {
            Numero = "SOL-004",
            FechaRecepcion = new DateTime(2026, 10, 13, 8, 40, 0),
            Cliente = "Conjunto Arrayanes",
            Contacto = "Juliana Torres",
            Municipio = "Bogotá",
            Direccion = "Carrera 70 # 52-30",
            EmailContacto = "administracion@arrayanes.example",
            Origen = OrigenSolicitud.PDF,
            Referencia = "ARR-042",
            TextoOriginal = """
                PDF 13/10/2026 08:40. Mantenimiento preventivo de un ascensor de pasajeros (6 pisos, 8 personas).
                Visita viernes 16/10/2026, 9:00-15:00. Presupuesto del cliente: $300.000 antes de IVA.
                """,
            Estado = EstadoSolicitud.RequiereValoracionTecnica,
            Observaciones = "El ascensor no está en el listado de tarifas. El presupuesto de $300.000 no fija precio. Requiere valoración técnica.",
            FechaVisita = new DateTime(2026, 10, 16),
            FranjaHoraria = "09:00 - 15:00",
            FinDeSemana = false,
            FueraDeHorario = false
        }
    ];

    private static List<Cotizacion> HistorialCotizaciones() =>
    [
        Historica("COT-001", new DateTime(2026, 9, 10), "Colegio Arboleda", "Bogotá", "2026-01", EstadoCotizacion.Aceptada,
        [
            Linea("AA-01", "Mantenimiento preventivo split hasta 24.000 BTU", 3, 90_000m),
            Linea("DES-BOG", "Desplazamiento Bogotá", 1, 25_000m)
        ]),
        Historica("COT-002", new DateTime(2026, 9, 18), "Edificio Mirador", "Chía", "2026-01", EstadoCotizacion.Aceptada,
        [
            Linea("PI-01", "Pintura de pared interior; materiales del cliente", 80, 13_000m),
            Linea("PL-01", "Cambio de grifería; repuesto del cliente", 1, 60_000m),
            Linea("DES-CHI", "Desplazamiento Chía", 1, 50_000m)
        ]),
        Historica("COT-003", new DateTime(2026, 9, 25), "Oficina Aurora", "Bogotá", "2026-01", EstadoCotizacion.Enviada,
        [
            Linea("EL-01", "Revisión de punto eléctrico", 4, 45_000m),
            Linea("DES-BOG", "Desplazamiento Bogotá", 1, 25_000m)
        ]),
        Historica("COT-004", new DateTime(2026, 10, 2), "Consultorio Cedro", "Bogotá", "2026-10", EstadoCotizacion.Aceptada,
        [
            Linea("AA-02", "Visita de diagnóstico split hasta 24.000 BTU", 1, 75_000m),
            Linea("DES-BOG", "Desplazamiento Bogotá", 1, 30_000m)
        ]),
        Historica("COT-005", new DateTime(2026, 10, 5), "Restaurante Limón", "Bogotá", "2026-10", EstadoCotizacion.Enviada,
        [
            Linea("PL-01", "Cambio de grifería; repuesto del cliente", 2, 65_000m),
            Linea("EL-01", "Revisión de punto eléctrico", 3, 50_000m),
            Linea("DES-BOG", "Desplazamiento Bogotá", 1, 30_000m)
        ]),
        Historica("COT-006", new DateTime(2026, 10, 8), "Bodega Norte", "Chía", "2026-10", EstadoCotizacion.Aceptada,
        [
            Linea("AA-01", "Mantenimiento preventivo split hasta 24.000 BTU", 2, 95_000m),
            Linea("REC-25", "Recargo de atención fuera de horario (sábado)", 1, 47_500m),
            Linea("DES-CHI", "Desplazamiento Chía", 1, 60_000m)
        ])
    ];

    private static Tarifa Servicio(string version, string codigo, string concepto, string unidad, decimal precio) =>
        new()
        {
            Version = version,
            Codigo = codigo,
            Concepto = concepto,
            Unidad = unidad,
            PrecioUnitarioCOP = precio,
            Tipo = TipoTarifa.Servicio,
            Activa = true
        };

    private static Tarifa Desplazamiento(string version, string codigo, string concepto, decimal precio) =>
        new()
        {
            Version = version,
            Codigo = codigo,
            Concepto = concepto,
            Unidad = "Visita",
            PrecioUnitarioCOP = precio,
            Tipo = TipoTarifa.Desplazamiento,
            Activa = true
        };

    private static Cotizacion Historica(
        string numero,
        DateTime fecha,
        string cliente,
        string municipio,
        string version,
        EstadoCotizacion estado,
        List<LineaCotizacion> lineas)
    {
        var subtotal = lineas.Sum(l => l.ValorLineaCOP);
        var iva = ReglasComerciales.CalcularIva(subtotal);
        return new Cotizacion
        {
            Numero = numero,
            FechaEmision = fecha,
            ValidaHasta = fecha.AddDays(ReglasComerciales.DiasValidez),
            Cliente = cliente,
            Municipio = municipio,
            VersionTarifa = version,
            Estado = estado,
            Subtotal = subtotal,
            IVA = iva,
            Total = subtotal + iva,
            PorcentajeIVA = ReglasComerciales.PorcentajeIva,
            EsHistorica = true,
            Observaciones = "Importes conservados del historial entregado (fecha original).",
            Lineas = lineas
        };
    }

    private static LineaCotizacion Linea(string codigo, string concepto, decimal cantidad, decimal precio) =>
        new()
        {
            Codigo = codigo,
            Concepto = concepto,
            Cantidad = cantidad,
            PrecioUnitarioCOP = precio,
            ValorLineaCOP = ReglasComerciales.RedondearCop(cantidad * precio)
        };
}
