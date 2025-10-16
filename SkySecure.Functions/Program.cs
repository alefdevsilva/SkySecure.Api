using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkySecure.Api.Services.Interfaces;
using SkySecure.Api.Services;
using SkySecure.Functions.Services.Interfaces;
using SkySecure.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddScoped<IPdfGenerator, LocalPdfGenerator>();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IInventoryService, InventoryService>();

        services.AddHttpClient();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();



host.Run();
