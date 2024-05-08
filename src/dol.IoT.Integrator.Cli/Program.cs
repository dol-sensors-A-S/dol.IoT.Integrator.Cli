using Cocona;
using dol.IoT.Integrator.Cli;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();

var config = builder.Configuration
    .AddJsonFile($"{AppContext.BaseDirectory}/appsettings.json")
    .Build()
    .Get<Config>()!;

builder.Logging.ClearProviders();
builder.Services.AddSingleton(config);
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddSingleton<LoginHandler>();

builder.Services.AddHttpClient<IIntegratorApiClient, IntegratorApiClient>()
    .AddHttpMessageHandler<LoginHandler>();

var app = builder.Build();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) =>
{
    // ReSharper disable once AccessToDisposedClosure
    cts.Cancel();
};

app.AddGetAllDevices();
app.AddSensorToDevice();
app.AddClaimDevice();
app.AddGetApiToken();
app.AddGetDevice();
app.AddGraphData(cts);
app.AddReadFromServiceBusQueue(cts);
app.AddRemoveDeviceClaim();
app.AddRemoveSensorFromDevice();
app.AddWatchServiceBusQueue(cts);
app.AddEnvSwitcher();
app.AddClearEmail();

app.Run();
