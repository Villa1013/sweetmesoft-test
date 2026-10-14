namespace MantiaServiceAPI.Configuration;

public static class EnvFile
{
    public static IReadOnlyList<string> Cargar(params string[] rutas)
    {
        var candidatas = rutas.Length > 0
            ? rutas
            : new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(AppContext.BaseDirectory, ".env")
            };

        var cargados = new List<string>();
        foreach (var ruta in candidatas)
        {
            if (File.Exists(ruta))
            {
                CargarArchivo(ruta);
                cargados.Add(ruta);
            }
        }

        return cargados;
    }

    private static void CargarArchivo(string ruta)
    {
        foreach (var linea in File.ReadAllLines(ruta))
        {
            var limpia = linea.Trim();
            if (limpia.Length == 0 || limpia.StartsWith('#'))
                continue;

            var separador = limpia.IndexOf('=');
            if (separador <= 0)
                continue;

            var clave = limpia[..separador].Trim();
            var valor = limpia[(separador + 1)..].Trim().Trim('"', '\'');

            if (clave.Length == 0 || Environment.GetEnvironmentVariable(clave) is not null)
                continue;

            Environment.SetEnvironmentVariable(clave, valor);
        }
    }
}
