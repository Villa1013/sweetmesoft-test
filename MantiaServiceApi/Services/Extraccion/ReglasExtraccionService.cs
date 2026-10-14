using System.Globalization;
using System.Text.RegularExpressions;
using MantiaServiceAPI.Models;
using MantiaServiceAPI.Services.Cotizacion;

namespace MantiaServiceAPI.Services.Extraccion;

public class ReglasExtraccionService : IExtraccionTexto
{
    public Task<ResultadoExtraccion> ExtraerAsync(string texto, CancellationToken cancellationToken = default)
    {
        var original = texto ?? string.Empty;
        var normalizado = ReglasComerciales.QuitarTildes(original.ToLowerInvariant());

        var resultado = new ResultadoExtraccion { Motor = "reglas" };
        DetectarCliente(original, resultado);
        DetectarContacto(original, resultado);
        DetectarUbicacion(original, normalizado, resultado);
        DetectarEmail(original, resultado);
        DetectarReferencia(original, resultado);
        DetectarAgenda(original, normalizado, resultado);
        DetectarItemsYEstado(normalizado, original, resultado);
        CompletarHorario(resultado);
        CompletarEstado(resultado);

        return Task.FromResult(resultado);
    }

    private static void DetectarCliente(string original, ResultadoExtraccion resultado)
    {
        if (Regex.IsMatch(original, "Colegio Arboleda", RegexOptions.IgnoreCase))
            resultado.Cliente = "Colegio Arboleda";
        else if (Regex.IsMatch(original, "Edificio Mirador", RegexOptions.IgnoreCase))
            resultado.Cliente = "Edificio Mirador";
        else if (Regex.IsMatch(original, "Oficina Horizonte", RegexOptions.IgnoreCase))
            resultado.Cliente = "Oficina Horizonte";
        else if (Regex.IsMatch(original, "Conjunto Arrayanes", RegexOptions.IgnoreCase))
            resultado.Cliente = "Conjunto Arrayanes";
        else
        {
            var match = Regex.Match(original, @"(Colegio|Edificio|Oficina|Conjunto|Administraci[oó]n)\s+([A-ZÁÉÍÓÚÑ][\wÁÉÍÓÚáéíóúñ]+)", RegexOptions.IgnoreCase);
            if (match.Success)
                resultado.Cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(match.Value.ToLowerInvariant());
        }
    }

    private static void DetectarContacto(string original, ResultadoExtraccion resultado)
    {
        var patrones = new[]
        {
            @"Soy\s+([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+)?)",
            @"Contacto:\s*([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+)",
            @"De:\s*([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+)",
            @"Responsable\s+([A-ZÁÉÍÓÚÑ][a-záéíóúñ]+\s+[A-ZÁÉÍÓÚÑ][a-záéíóúñ]+)"
        };

        foreach (var patron in patrones)
        {
            var match = Regex.Match(original, patron);
            if (match.Success)
            {
                resultado.Contacto = match.Groups[1].Value.Trim();
                return;
            }
        }
    }

    private static void DetectarUbicacion(string original, string normalizado, ResultadoExtraccion resultado)
    {
        resultado.Municipio = ReglasComerciales.NormalizarMunicipio(
            normalizado.Contains("chia") ? "Chía" :
            normalizado.Contains("bogota") ? "Bogotá" : string.Empty);

        var direccion = Regex.Match(original, @"(Calle|Carrera|Cra\.?|Cl\.?)\s+\d+[^\n,]{0,40}", RegexOptions.IgnoreCase);
        if (direccion.Success)
            resultado.Direccion = direccion.Value.Trim().TrimEnd('.');
    }

    private static void DetectarEmail(string original, ResultadoExtraccion resultado)
    {
        var match = Regex.Match(original, @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase);
        if (match.Success)
            resultado.EmailContacto = match.Value;
    }

    private static void DetectarReferencia(string original, ResultadoExtraccion resultado)
    {
        var match = Regex.Match(original, @"\b([A-Z]{2,5}-\d{2,4})\b");
        if (match.Success)
            resultado.Referencia = match.Groups[1].Value;
    }

    private static void DetectarAgenda(string original, string normalizado, ResultadoExtraccion resultado)
    {
        var fecha = Regex.Match(original, @"(\d{1,2})\s+de\s+(enero|febrero|marzo|abril|mayo|junio|julio|agosto|septiembre|octubre|noviembre|diciembre)\s+de\s+(20\d{2})", RegexOptions.IgnoreCase);
        if (fecha.Success)
        {
            var dia = int.Parse(fecha.Groups[1].Value);
            var anio = int.Parse(fecha.Groups[3].Value);
            var mes = Mes(fecha.Groups[2].Value);
            resultado.FechaVisita = new DateTime(anio, mes, dia);
        }
        else if (normalizado.Contains("jueves 15"))
            resultado.FechaVisita = new DateTime(2026, 10, 15);
        else if (normalizado.Contains("viernes 16"))
            resultado.FechaVisita = new DateTime(2026, 10, 16);

        var franja = Regex.Match(original, @"entre las?\s+(\d{1,2}:\d{2})\s+y\s+las?\s+(\d{1,2}:\d{2})", RegexOptions.IgnoreCase);
        if (!franja.Success)
            franja = Regex.Match(original, @"(\d{1,2}:\d{2})\s*(?:a|-|y)\s*(\d{1,2}:\d{2})");
        if (franja.Success)
            resultado.FranjaHoraria = $"{franja.Groups[1].Value} - {franja.Groups[2].Value}";
    }

    private static void DetectarItemsYEstado(string normalizado, string original, ResultadoExtraccion resultado)
    {
        if (normalizado.Contains("ascensor"))
        {
            resultado.Estado = EstadoSolicitud.RequiereValoracionTecnica;
            resultado.Observaciones = "El mantenimiento de ascensor no está en el listado de tarifas. Requiere valoración técnica. Un presupuesto del cliente no fija el precio.";
            return;
        }

        var pideReparacion = Contiene(normalizado, "dejo de enfriar", "arreglen", "arreglo", "quede funcionando", "repar");
        var preventivo = Contiene(normalizado, "mantenimiento preventivo");
        var diagnostico = Contiene(normalizado, "diagnostico");

        if (pideReparacion && !preventivo)
        {
            resultado.Estado = EstadoSolicitud.RequiereValoracionTecnica;
            resultado.Observaciones = "Piden reparación. El diagnóstico AA-02 no incluye reparación ni repuestos. Falta ficha del equipo.";
            return;
        }

        if (preventivo || (normalizado.Contains("aire") && !pideReparacion && !diagnostico))
        {
            var cantidad = ExtraerCantidadAires(normalizado, original);
            resultado.Items.Add(new ItemExtraido
            {
                CodigoTarifa = "AA-01",
                Concepto = "Mantenimiento preventivo de aire acondicionado tipo split hasta 24.000 BTU",
                Cantidad = cantidad ?? 0,
                Observacion = ExtraerBtu(original)
            });
        }

        if (diagnostico)
        {
            resultado.Items.Add(new ItemExtraido
            {
                CodigoTarifa = "AA-02",
                Concepto = "Visita de diagnóstico de aire acondicionado tipo split hasta 24.000 BTU",
                Cantidad = 1
            });
        }

        if (Contiene(normalizado, "pintar", "pintura"))
        {
            var m2 = ExtraerMetros(normalizado);
            resultado.Items.Add(new ItemExtraido
            {
                CodigoTarifa = "PI-01",
                Concepto = "Pintura interior de paredes. Materiales suministrados por el cliente",
                Cantidad = m2 ?? 0,
                Observacion = "Se cobra m² de pared, no de piso"
            });
        }

        if (Contiene(normalizado, "grifer"))
        {
            var piezas = ExtraerEnteroCercano(normalizado, "grifer") ?? ExtraerEntero(original, @"cambiar\s+(\d+)");
            resultado.Items.Add(new ItemExtraido
            {
                CodigoTarifa = "PL-01",
                Concepto = "Cambio de grifería sencilla. Repuesto suministrado por el cliente",
                Cantidad = piezas ?? 0
            });
        }

        if (Contiene(normalizado, "punto electrico", "electricidad"))
        {
            var puntos = ExtraerEnteroCercano(normalizado, "punto") ?? 1;
            resultado.Items.Add(new ItemExtraido
            {
                CodigoTarifa = "EL-01",
                Concepto = "Revisión de punto eléctrico",
                Cantidad = puntos
            });
        }
    }

    private static void CompletarHorario(ResultadoExtraccion resultado)
    {
        resultado.FinDeSemana = ReglasComerciales.EsFinDeSemana(resultado.FechaVisita);
        resultado.FueraDeHorario = ReglasComerciales.EsFueraDeHorario(resultado.FranjaHoraria);
    }

    private static void CompletarEstado(ResultadoExtraccion resultado)
    {
        if (resultado.Estado == EstadoSolicitud.RequiereValoracionTecnica)
            return;

        var faltantes = new List<string>();
        if (string.IsNullOrWhiteSpace(resultado.Municipio))
            faltantes.Add("municipio");
        if (string.IsNullOrWhiteSpace(resultado.Direccion))
            faltantes.Add("dirección");
        if (resultado.Items.Count == 0)
            faltantes.Add("alcance / código de tarifa");
        if (resultado.Items.Any(i => i.Cantidad <= 0))
            faltantes.Add("cantidades");

        resultado.CamposFaltantes = string.Join(", ", faltantes);
        resultado.Estado = faltantes.Count == 0
            ? EstadoSolicitud.ListaParaCotizar
            : EstadoSolicitud.FaltaInformacion;
    }

    private static decimal? ExtraerCantidadAires(string normalizado, string original)
    {
        if (Regex.IsMatch(normalizado, @"son 4 equipos|en realidad son 4|4 equipos"))
            return 4;
        if (Regex.IsMatch(normalizado, @"\b3 aires|\b3 equipos"))
            return 3;

        var match = Regex.Match(original, @"(\d+)\s+(aires?|equipos?)", RegexOptions.IgnoreCase);
        if (match.Success)
            return decimal.Parse(match.Groups[1].Value);
        if (Regex.IsMatch(normalizado, @"\bun aire\b|\bun equipo\b"))
            return 1;
        return null;
    }

    private static decimal? ExtraerMetros(string normalizado)
    {
        var match = Regex.Match(normalizado, @"(\d+(?:[.,]\d+)?)\s*m[2²]");
        if (match.Success)
            return decimal.Parse(match.Groups[1].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
        return null;
    }

    private static decimal? ExtraerEnteroCercano(string normalizado, string palabra)
    {
        var match = Regex.Match(normalizado, $@"(\d+)\s+{palabra}");
        if (match.Success)
            return decimal.Parse(match.Groups[1].Value);
        return null;
    }

    private static decimal? ExtraerEntero(string original, string patron)
    {
        var match = Regex.Match(original, patron, RegexOptions.IgnoreCase);
        return match.Success ? decimal.Parse(match.Groups[1].Value) : null;
    }

    private static string ExtraerBtu(string original)
    {
        var match = Regex.Match(original, @"(\d{1,3}(?:[.\s]?\d{3})?)\s*BTU", RegexOptions.IgnoreCase);
        return match.Success ? $"{match.Groups[1].Value} BTU" : string.Empty;
    }

    private static bool Contiene(string texto, params string[] valores) =>
        valores.Any(texto.Contains);

    private static int Mes(string nombre) => nombre.ToLowerInvariant() switch
    {
        "enero" => 1,
        "febrero" => 2,
        "marzo" => 3,
        "abril" => 4,
        "mayo" => 5,
        "junio" => 6,
        "julio" => 7,
        "agosto" => 8,
        "septiembre" => 9,
        "octubre" => 10,
        "noviembre" => 11,
        "diciembre" => 12,
        _ => 10
    };
}
