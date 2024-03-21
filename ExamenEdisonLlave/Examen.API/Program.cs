using Examen.API.Contratos.Repositorios;
using Examen.API.Implementacion.Repositorios;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IRepositorio, Repositorio>((s) =>
        {
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            string cadenaConexion = configuration.GetConnectionString("cadena");
            return new Repositorio(cadenaConexion);
        });
    })
    .Build();

host.Run();
