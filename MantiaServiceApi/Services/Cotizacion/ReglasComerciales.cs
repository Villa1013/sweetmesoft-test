using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.Services.Cotizacion;

public static class ReglasComerciales
{
    public const decimal PorcentajeIva = 19m;
    public const decimal PorcentajeRecargo = 25m;
    public const int DiasValidez = 15;
    public const int HoraInicioOrdinaria = 8;
    public const int HoraFinOrdinaria = 18;
    public const string CodigoRecargo = "REC-25";
    public const string ConceptoRecargo = "Recargo de atención fuera de horario o fin de semana (25% sobre servicios)";
    public const string CodigoDesplazamientoBogota = "DES-BOG";
    public const string CodigoDesplazamientoChia = "DES-CHI";

    public static decimal RedondearCop(decimal valor) =>
        Math.Round(valor, 0, MidpointRounding.AwayFromZero);

    public static decimal CalcularIva(decimal subtotal) =>
        RedondearCop(subtotal * (PorcentajeIva / 100m));

    public static decimal CalcularRecargoServicios(decimal subtotalServicios) =>
        RedondearCop(subtotalServicios * (PorcentajeRecargo / 100m));

    public static string? ResolverVersionTarifa(DateTime fechaReferencia)
    {
        var fecha = fechaReferencia.Date;
        if (fecha >= new DateTime(2026, 1, 1) && fecha <= new DateTime(2026, 9, 30))
            return "2026-01";
        if (fecha >= new DateTime(2026, 10, 1) && fecha <= new DateTime(2026, 12, 31))
            return "2026-10";
        return null;
    }

    public static string NormalizarMunicipio(string? municipio)
    {
        var valor = QuitarTildes((municipio ?? string.Empty).Trim().ToLowerInvariant());
        if (valor.Contains("bogota"))
            return "Bogotá";
        if (valor.Contains("chia"))
            return "Chía";
        return (municipio ?? string.Empty).Trim();
    }

    public static string? CodigoDesplazamiento(string municipio)
    {
        return NormalizarMunicipio(municipio) switch
        {
            "Bogotá" => CodigoDesplazamientoBogota,
            "Chía" => CodigoDesplazamientoChia,
            _ => null
        };
    }

    public static bool MunicipioCubierto(string municipio)
    {
        var normalizado = NormalizarMunicipio(municipio);
        return normalizado is "Bogotá" or "Chía";
    }

    public static bool EsFinDeSemana(DateTime? fechaVisita) =>
        fechaVisita is { } fecha &&
        (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday);

    public static bool EsFueraDeHorario(string? franjaHoraria)
    {
        var horas = ExtraerHoras(franjaHoraria);
        if (horas.Count == 0)
            return false;

        var inicio = horas[0];
        var fin = horas[^1];
        return inicio < HoraInicioOrdinaria || fin > HoraFinOrdinaria;
    }

    public static bool AplicaRecargo(bool finDeSemana, bool fueraDeHorario) =>
        finDeSemana || fueraDeHorario;

    public static List<int> ExtraerHoras(string? franja)
    {
        if (string.IsNullOrWhiteSpace(franja))
            return [];

        var horas = new List<int>();
        foreach (Match match in Regex.Matches(franja, @"\b(\d{1,2})(?::(\d{2}))?\b"))
        {
            if (!int.TryParse(match.Groups[1].Value, out var hora) || hora > 23)
                continue;
            horas.Add(hora);
        }

        return horas;
    }

    public static string QuitarTildes(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
