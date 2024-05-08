using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class RemoveDeviceClaimCommand
{
    public static void AddRemoveDeviceClaim(
        this CoconaApp app)
    {
        app.AddCommand("remove-device-claim", async (
            [Option('m', Description = "device mac address")] string? mac,
            IIntegratorApiClient integrationApiClient) =>
        {
            mac ??= AskFor.This("MAC address");
            var response = await integrationApiClient.RemoveDeviceClaim(mac);
            
            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Remove a device claim", hasSideEffects: true);
    }
}