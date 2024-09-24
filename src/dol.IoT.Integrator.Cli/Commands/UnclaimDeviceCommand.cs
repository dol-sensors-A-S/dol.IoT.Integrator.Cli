using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class UnclaimDeviceCommand
{
    public static void AddUnclaimDevice(this CoconaApp app)
    {
        app.AddCommand("unclaim-device", async (
            [Argument(Description = "device mac address")] string mac,
            IIntegratorApiClient integrationApiClient) =>
        {
            var response = await integrationApiClient.UnclaimDevice(mac);

            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Remove a device claim", hasSideEffects: true);
    }
}