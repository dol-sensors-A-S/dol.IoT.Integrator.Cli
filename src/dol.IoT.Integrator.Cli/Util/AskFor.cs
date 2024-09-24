using dol.IoT.Models.Public.DeviceApi;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Util;

public static class AskFor
{
    public static T This<T>(
        string question)
    {
        return AnsiConsole.Ask<T>(question);
    }

    public static string This(
        string question)
    {
        return AnsiConsole.Ask<string>(question);
    }

    public static string GetSensorType()
    {
        var sensorType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select sensor type[/]")
                .AddChoices(nameof(SensorType.DOL16),
                    nameof(SensorType.DOL53),
                    nameof(SensorType.DOL90),
                    nameof(SensorType.IDOL139)));

        return sensorType;
    }

    public static string? GetDeviceType()
    {
        var deviceType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select device type[/]")
                .AddChoices(nameof(DeviceType.IDOL65),
                    nameof(DeviceType.IDOL64),
                    nameof(DeviceType.IDOL63), "no device type", "any"));

        if (deviceType.Equals("any"))
            return null;

        Enum.TryParse(deviceType, true, out DeviceType deviceTypeEnum);
        return deviceTypeEnum.ToString();
    }

    public static string? ConnectionState()
    {
        var connectionState = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select connection state[/]")
                .AddChoices("Connected", "Disconnected", "any"));

        if (connectionState.Equals("any"))
            return null;

        return connectionState;
    }

    public static string? Prefix()
    {
        var result = AnsiConsole.Ask<string?>("Device Id [green]prefix[/]?", "");
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    public static string? Postfix()
    {
        var result = AnsiConsole.Ask<string?>("Device Id [green]postfix[/]?", "");
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    public static string? SystemVersion()
    {
        var result = AnsiConsole.Ask<string?>("[green]SystemVersion[/]?", "");
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    public static bool NoSystemVersion()
    {
        return AnsiConsole.Confirm("[green]Search devices with no system version[/]", false);
    }
}