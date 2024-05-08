namespace dol.IoT.Integrator.Cli.ApiIntegration;

public enum Env
{
    Qa = 0,
    Production = 1,
}

public static class EnvEnumExtensions
{
    public static string BaseUrl(this Env env)
    {
        return env switch
        {
            Env.Production => "https://iot.dol-sensors.com",
            Env.Qa => "https://dol-iot-api-qa.azurewebsites.net/",
            _ => throw new ArgumentException($"unknown env: {env}")
        };
    }
}