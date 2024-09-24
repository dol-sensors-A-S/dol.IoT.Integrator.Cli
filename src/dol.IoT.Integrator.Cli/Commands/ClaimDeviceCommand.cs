using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using dol.IoT.Models.Public.DeviceApi;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class ClaimDeviceCommand
{
    public static void AddClaimDevice(
        this CoconaApp app)
    {
        app.AddCommand("claim-device", async (
            [Option('m', Description = "device mac address")] string? mac,
            [Option('k', Description = "key")] string? key,
            [Option('t', Description = "device type")] string? deviceType,
            [Option('n', Description = "device name")] string? deviceName,
            [Option('o', Description = "owner")] string? owner,
            IIntegratorApiClient integrationApiClient) =>
        {

            if (!Enum.TryParse(deviceType, true, out DeviceType deviceTypeEnum))
            {
                AnsiConsole.MarkupLine($"Unknown device type {deviceType}");
                return;
            }

            var claimDeviceRequest = new ClaimDeviceRequest(

                MacAddress: mac ?? AskFor.This("MAC address"),
                Key: key ?? AskFor.This("Key"),
                DeviceType: deviceTypeEnum,
                Owner: owner,
                DeviceName: deviceName ?? AskFor.This("Device name"));

            var response = await integrationApiClient.ClaimDevice(claimDeviceRequest);

            AnsiConsole.MarkupLine(response.Success
                ? $"[green]OK - {response.Response.EscapeMarkup()}[/]"
                : $"[red]ERR - {response.Response.EscapeMarkup()}[/]");
        }).WithIntegratorDescription("Claim a device as yours", hasSideEffects: true);
    }
}