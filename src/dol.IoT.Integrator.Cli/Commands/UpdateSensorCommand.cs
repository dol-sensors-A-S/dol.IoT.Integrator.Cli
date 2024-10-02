using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using dol.IoT.Models.Public.DeviceApi;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class UpdateSensorCommand
{
    public static void AddUpdateSensor(
        this CoconaApp app)
    {
        app.AddCommand("update-sensor", async (
            [Option('m', Description = "device mac address")]
            string? mac,
            [Option('n', Description = "sensor name")]
            string? name,
            [Option('d', Description = "sensor deveui")]
            string? devEui,
            [Option('s', Description = "sample rate")]
            int? sampleRate,
            IIntegratorApiClient integrationApiClient) =>
        {

            mac ??= AskFor.This("MAC address");
            name ??= AskFor.This("Sensor name");
            devEui ??= AskFor.This("Sensor DevEUI");
            sampleRate ??= AskFor.This<int>("Sample rate (seconds)");

            var updateSensorRequest = new UpdateSensorToDeviceRequest(devEui, name, sampleRate);
            var response = await integrationApiClient.UpdateSensorToDevice(mac, updateSensorRequest);
            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");

        }).WithIntegratorDescription("Update sensor on gateway device", hasSideEffects: true);
    }
}