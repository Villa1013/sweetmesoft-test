using System.Text.Json.Serialization;
using MantiaServiceAPI.Configuration;
using MantiaServiceAPI.Data;
using MantiaServiceAPI.Services;
using MantiaServiceAPI.Services.Cotizacion;
using MantiaServiceAPI.Services.Extraccion;
using Microsoft.EntityFrameworkCore;

var envCargados = EnvFile.Cargar();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "MantiaServiceAPI", Version = "v1" });
    options.UseInlineDefinitionsForEnums();
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=mantia.db"));

var geminiTimeoutSegundos = builder.Configuration.GetValue<int?>("Gemini:TimeoutSeconds") ?? 8;
builder.Services.AddHttpClient<GeminiExtraccionService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(geminiTimeoutSegundos);
});
builder.Services.AddSingleton<CotizacionCalculator>();
builder.Services.AddScoped<ReglasExtraccionService>();
builder.Services.AddScoped<IExtraccionService, ExtraccionService>();
builder.Services.AddScoped<SolicitudService>();
builder.Services.AddScoped<CotizacionService>();

var app = builder.Build();

app.Logger.LogInformation(
    "Archivos .env cargados: {Archivos}",
    envCargados.Count > 0 ? string.Join(", ", envCargados) : "(ninguno)");

var gemini = app.Services.GetRequiredService<GeminiExtraccionService>();
app.Logger.LogInformation(
    "Motor de extracción: {Motor}",
    gemini.TieneApiKey
        ? $"Gemini (modelo {app.Configuration["Gemini:Model"]}, timeout {geminiTimeoutSegundos}s)"
        : $"reglas locales (API key no configurada; timeout configurado {geminiTimeoutSegundos}s)");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MantiaServiceAPI v1");
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
