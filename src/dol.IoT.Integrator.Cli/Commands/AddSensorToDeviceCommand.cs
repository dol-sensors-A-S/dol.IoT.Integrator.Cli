using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Models;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class AddSensorToDeviceCommand
{
    public static void AddSensorToDevice(
        this CoconaApp app)
    {
        app.AddCommand("add-sensor", async (
            [Option('m', Description = "device mac address")]
            string? mac,
            [Option('n', Description = "sensor name")]
            string? name,
            [Option('d', Description = "sensor deveui")]
            string? devEui,
            [Option('t', Description = "sensor type")]
            string? sensorType,
            [Option('s', Description = "sample rate")]
            int? sampleRate,
            IIntegratorApiClient integrationApiClient) =>
        {
            mac ??= AskFor.This("MAC address");

            var addSensorRequest = new AddSensorRequest
            {
                Name = name ?? AskFor.This("Sensor name"),
                DevEui = devEui ?? AskFor.This("Sensor DevEUI"),
                Type = sensorType ?? AskFor.SensorType(),
                SampleRate = sampleRate ?? AskFor.This<int>("Sample rate (seconds)")
            };

            var response = await integrationApiClient.AddSensorToDevice(mac, addSensorRequest);
            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Add a lora sensor to gateway device", hasSideEffects: true);
    }
}