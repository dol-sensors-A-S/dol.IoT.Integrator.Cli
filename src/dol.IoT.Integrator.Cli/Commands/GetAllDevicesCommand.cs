using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class GetAllDevicesCommand
{
    public static void AddGetAllDevices(
        this CoconaApp app)
    {
        app.AddCommand("all-device-info", async (
            [Option('p', Description = "page number")] int? page,
            [Option('s', Description = "page size")] int? pageSize,
            IIntegratorApiClient integrationApiClient) =>
        {
            page ??= 1;
            pageSize ??= 50;
            var resp = await integrationApiClient.GetAllDevices(page.Value, pageSize.Value);
            if (resp is not null)
            {
                AnsiConsoleExt.WriteJson(resp, $"[Devices page={page} pageSize={pageSize}]".EscapeMarkup());
            }
            else
            {
                AnsiConsole.Write("Device not found in api");
            }
        }).WithIntegratorDescription("Get device info", hasSideEffects: false);
    }
}