using dol.IoT.Integrator.Cli.ApiIntegration;
using static System.Text.Json.JsonSerializer;

namespace dol.IoT.Integrator.Cli;

public class Config
{
    public Env Env { get; set; }
    public string Email { get; set; } = "";

    public async Task Save()
    {
        await File.WriteAllTextAsync(ConfigFileName, Serialize(this));
    }
    
    private static readonly string ConfigFileName = $"{AppContext.BaseDirectory}/appsettings.json";
}