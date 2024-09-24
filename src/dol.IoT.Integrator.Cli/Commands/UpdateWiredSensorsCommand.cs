using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using dol.IoT.Models.Public.DeviceApi;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class UpdateWiredSensorsCommand
{
    public static void AddPatchWiredSensors(this CoconaApp app)
    {
        app.AddCommand("wired-sensors", async (
            [Argument(Description = "device mac address")] string mac,
            [Option("port-1-type", Description= "")] WiredSensorType? type1,
            [Option("port-2-type", Description= "")] WiredSensorType? type2,
            [Option("port-3-type", Description= "")] WiredSensorType? type3,
            [Option("port-4-type", Description= "")] WiredSensorType? type4,
            IIntegratorApiClient integrationApiClient) =>
        {
            List<WiredSensorRequest> list = [];
            if (type1.HasValue)
                list.Add(new WiredSensorRequest(1, type1.Value, 60));
            if (type2.HasValue)
                list.Add(new WiredSensorRequest(2, type2.Value, 60));
            if (type3.HasValue)
                list.Add(new WiredSensorRequest(3, type3.Value, 60));
            if (type4.HasValue)
                list.Add(new WiredSensorRequest(4, type4.Value, 60));

            var updateWiredSensorsRequest = new UpdateWiredSensorsRequest(list.ToArray());

            var response = await integrationApiClient.UpdateWiredSensors(mac, updateWiredSensorsRequest);

            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Update wired sensors", hasSideEffects: true);
    }
}