using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class GetApiToken
{
    public static void AddGetApiToken(
        this CoconaApp app)
    {
        app.AddCommand("api-token", async (ILoginService loginService) =>
        {
            var (token, expires) = await loginService.GetToken();
            AnsiConsole.WriteLine($"Your api token is");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[red]{token}[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[blue]expires {expires}[/]");
        }).WithIntegratorDescription("Get api token to use with the integration API. It expires after an hour", hasSideEffects: false);
    }
}