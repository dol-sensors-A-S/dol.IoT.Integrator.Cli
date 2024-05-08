namespace dol.IoT.Integrator.Cli.Models;

public class LoginResponse
{
    public required string tokenType { get; set; }
    public required string accessToken { get; set; }
    public required int expiresIn { get; set; }
    public required string refreshToken { get; set; }
    public DateTime Expires { get; set; }
}