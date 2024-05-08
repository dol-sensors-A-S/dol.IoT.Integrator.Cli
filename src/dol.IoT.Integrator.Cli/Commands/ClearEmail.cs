using Cocona;

namespace dol.IoT.Integrator.Cli.Commands;

public static class ClearEmail
{
    public static void AddClearEmail(this CoconaApp app)
    {
        app.AddCommand("clear-email", async (Config config) =>
        {
            config.Email = "";
            await config.Save();
        }).WithDescription("clears the saved email (used for login)");
    }
}