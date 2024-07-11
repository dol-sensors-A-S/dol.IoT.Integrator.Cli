using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using Spectre.Console;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace dol.IoT.Integrator.Cli.Commands;

public static class EnvSwitcher
{
    public static void AddEnvSwitcher(
        this CoconaApp app)
    {
        app.AddCommand("env", async (
            [Option('s')] string? selectEnv) =>
        {
            var path = $"{AppContext.BaseDirectory}/appsettings.json";
            var readAllText = await File.ReadAllTextAsync(path);
            var config = JsonSerializer.Deserialize<Config>(readAllText)!;

            AnsiConsole.MarkupLine($"Currently using [red]{config.Env}[/]");

            if (string.IsNullOrWhiteSpace(selectEnv))
            {
                var envToUse = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Select env to use[/]")
                        .AddChoices("Production", "Qa"));

                config.Env = Enum.Parse<Env>(envToUse, true);
                await File.WriteAllTextAsync(path, JsonSerializer.Serialize(config));

                AnsiConsole.Markup($"Now using [red]{config.Env}[/]");
                File.Delete(LoginService.FileName);
            }
            else
            {
                if (Enum.TryParse(selectEnv!, true, out Env env))
                {
                    config.Env = env;
                    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(config));
                    AnsiConsole.Markup($"Now using [red]{config.Env}[/]");
                    File.Delete(LoginService.FileName);
                }
                else
                {
                    AnsiConsole.WriteLine($"Unknown env {selectEnv}, choices are {Env.Production}, {Env.Qa}");
                }
            }
        }).WithDescription("Switches between which integration-api environment to use");
    }
}