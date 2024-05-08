using Spectre.Console;
using Spectre.Console.Json;

namespace dol.IoT.Integrator.Cli.Util;

public static class AnsiConsoleExt
{
    public static void WriteJson(
        string json,
        string header = "")
    {
        var jsonText = new JsonText(json)
            .BracesColor(Color.Red)
            .BracketColor(Color.Green)
            .ColonColor(Color.Blue)
            .CommaColor(Color.Red)
            .StringColor(Color.Green)
            .NumberColor(Color.Blue)
            .BooleanColor(Color.Red)
            .NullColor(Color.Green);

        if (string.IsNullOrWhiteSpace(header))
        {
            AnsiConsole.Write(
                new Panel(jsonText)
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));
        }
        else
        {
            AnsiConsole.Write(
                new Panel(jsonText)
                    .Header(header)
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));
        }
    }
}