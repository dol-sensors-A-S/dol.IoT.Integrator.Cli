namespace dol.IoT.Integrator.Cli.Models;

public class LoginRequest
{
    public required string Email { get; init; } 
    public required string Password { get; init; }
}