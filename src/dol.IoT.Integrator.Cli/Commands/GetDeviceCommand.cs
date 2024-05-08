using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class GetDeviceCommand
{
    public static void AddGetDevice(
        this CoconaApp app)
    {
        app.AddCommand("device-info", async (
            [Option('m', Description = "device mac address")]
            string? mac,
            IIntegratorApiClient integrationApiClient) =>
        {
            var resp = await integrationApiClient.GetDevice(mac ?? AskFor.This("Device MAC address"));
            if (resp is not null)
            {
                AnsiConsoleExt.WriteJson(resp, $"[Device {mac}]".EscapeMarkup());
            }
            else
            {
                AnsiConsole.Write("Device not found in api");
            }
        }).WithIntegratorDescription("Get device info", hasSideEffects: false);
    }
}