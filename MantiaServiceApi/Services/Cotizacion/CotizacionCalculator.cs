using MantiaServiceAPI.Models;

namespace MantiaServiceAPI.Services.Cotizacion;

public class CotizacionCalculator
{
    public ResultadoCalculo Calcular(
        Solicitud solicitud,
        IReadOnlyList<Tarifa> tarifas,
        DateTime fechaEmision)
    {
        var errores = new List<string>();

        if (solicitud.Estado == EstadoSolicitud.RequiereValoracionTecnica)
            errores.Add("La solicitud requiere valoración técnica antes de cotizar.");

        if (solicitud.Estado == EstadoSolicitud.FaltaInformacion)
            errores.Add("Falta información para cotizar. Completa los datos o contacta al cliente.");

        if (solicitud.Items.Count == 0)
            errores.Add("La solicitud no tiene ítems de servicio para cotizar.");

        if (!ReglasComerciales.MunicipioCubierto(solicitud.Municipio))
            errores.Add("El municipio no está cubierto automáticamente. Requiere confirmación comercial.");

        var fechaReferenciaTarifa = solicitud.FechaVisita ?? fechaEmision;
        var version = ReglasComerciales.ResolverVersionTarifa(fechaReferenciaTarifa);
        if (version is null)
            errores.Add("La fecha de visita está fuera de la vigencia de las tarifas del caso.");

        if (errores.Count > 0)
            return new ResultadoCalculo { Ok = false, Errores = errores, FechaEmision = fechaEmision };

        var tarifasVersion = tarifas
            .Where(t => t.Version == version && t.Activa)
            .ToList();

        var lineas = new List<LineaCalculada>();
        decimal subtotalServicios = 0;

        foreach (var item in solicitud.Items)
        {
            var tarifa = tarifasVersion.FirstOrDefault(t =>
                t.Codigo.Equals(item.CodigoTarifa, StringComparison.OrdinalIgnoreCase)
                && t.Tipo == TipoTarifa.Servicio);

            if (tarifa is null)
            {
                errores.Add($"No hay tarifa vigente para el código {item.CodigoTarifa} en {version}.");
                continue;
            }

            if (item.Cantidad <= 0)
            {
                errores.Add($"La cantidad de {item.CodigoTarifa} debe ser mayor a cero.");
                continue;
            }

            var valor = ReglasComerciales.RedondearCop(item.Cantidad * tarifa.PrecioUnitarioCOP);
            subtotalServicios += valor;
            lineas.Add(new LineaCalculada
            {
                Codigo = tarifa.Codigo,
                Concepto = tarifa.Concepto,
                Cantidad = item.Cantidad,
                PrecioUnitarioCOP = tarifa.PrecioUnitarioCOP,
                ValorLineaCOP = valor,
                Tipo = TipoTarifa.Servicio
            });
        }

        if (errores.Count > 0)
            return new ResultadoCalculo { Ok = false, Errores = errores, FechaEmision = fechaEmision };

        if (ReglasComerciales.AplicaRecargo(solicitud.FinDeSemana, solicitud.FueraDeHorario))
        {
            var recargo = ReglasComerciales.CalcularRecargoServicios(subtotalServicios);
            lineas.Add(new LineaCalculada
            {
                Codigo = ReglasComerciales.CodigoRecargo,
                Concepto = ReglasComerciales.ConceptoRecargo,
                Cantidad = 1,
                PrecioUnitarioCOP = recargo,
                ValorLineaCOP = recargo,
                Tipo = TipoTarifa.Recargo
            });
        }

        var codigoDesplazamiento = ReglasComerciales.CodigoDesplazamiento(solicitud.Municipio)!;
        var tarifaDesplazamiento = tarifasVersion.FirstOrDefault(t => t.Codigo == codigoDesplazamiento);
        if (tarifaDesplazamiento is null)
        {
            return new ResultadoCalculo
            {
                Ok = false,
                Errores = [$"No hay tarifa de desplazamiento {codigoDesplazamiento} en {version}."],
                FechaEmision = fechaEmision
            };
        }

        lineas.Add(new LineaCalculada
        {
            Codigo = tarifaDesplazamiento.Codigo,
            Concepto = tarifaDesplazamiento.Concepto,
            Cantidad = 1,
            PrecioUnitarioCOP = tarifaDesplazamiento.PrecioUnitarioCOP,
            ValorLineaCOP = tarifaDesplazamiento.PrecioUnitarioCOP,
            Tipo = TipoTarifa.Desplazamiento
        });

        var subtotal = lineas.Sum(l => l.ValorLineaCOP);
        var iva = ReglasComerciales.CalcularIva(subtotal);
        var total = subtotal + iva;

        return new ResultadoCalculo
        {
            Ok = true,
            VersionTarifa = version!,
            FechaEmision = fechaEmision,
            ValidaHasta = fechaEmision.Date.AddDays(ReglasComerciales.DiasValidez),
            Subtotal = subtotal,
            IVA = iva,
            Total = total,
            Lineas = lineas
        };
    }
}
