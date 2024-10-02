using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using dol.IoT.Models.Public.ManagementApi;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class RequeueDataCommand
{
    public static void AddRequeueData(this CoconaApp app)
    {
        app.AddCommand("requeue-data", async (
            [Argument(Description = "device mac address")] string mac,
            [Argument(Description = "start time utc")] DateTime startTimeUtc,
            [Argument(Description = "end time utc")] DateTime? endTimeUtc,
            IIntegratorApiClient integrationApiClient) =>
        {
            var request = new RequeueDeviceDataRequest(mac, startTimeUtc, endTimeUtc);
            var response = await integrationApiClient.RequeueData(request);
            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Requeue data from a given device in a given timeframe", hasSideEffects: true);
    }
}