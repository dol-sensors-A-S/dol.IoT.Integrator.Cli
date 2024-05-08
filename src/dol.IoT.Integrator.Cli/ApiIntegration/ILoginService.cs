namespace dol.IoT.Integrator.Cli.ApiIntegration;

public interface ILoginService
{
    Task<(string Token, string Expires)> GetToken();
}