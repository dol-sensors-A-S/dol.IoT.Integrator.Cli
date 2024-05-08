using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class RemoveSensorCommand
{
    public static void AddRemoveSensorFromDevice(
        this CoconaApp app)
    {
        app.AddCommand("remove-sensor", async (
            [Option('m', Description = "device mac address")] string? mac,
            [Option('d', Description = "sensor deveui")] string? devEui,
            IIntegratorApiClient integrationApiClient) =>
        {
            mac ??= AskFor.This("MAC address");
            devEui ??= AskFor.This("Sensor DevEUI");

            var response = await integrationApiClient.RemoveSensorFromDevice(mac, devEui);
            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response}[/]"
                : $"[red]ERR - {response.Response}[/]");
        }).WithIntegratorDescription("Remove sensor from device", hasSideEffects: true);
    }
}