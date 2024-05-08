using System.Net;
using System.Net.Http.Json;
using dol.IoT.Integrator.Cli.Models;
using Spectre.Console;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace dol.IoT.Integrator.Cli.ApiIntegration;

public class LoginService : ILoginService
{
    public static readonly string FileName = $"{AppContext.BaseDirectory}/_token";
    private readonly Config _config;
    private readonly HttpClient _client;

    public LoginService(Config config)
    {
        _config = config;
        
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        var uriString = config.Env.BaseUrl();
        httpClient.BaseAddress = new Uri(uriString);
        _client = httpClient;
    }

    public async Task<(string Token, string Expires)> GetToken()
    {
        if (File.Exists(FileName))
        {
            var text = await File.ReadAllTextAsync(FileName);
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(text)!;
            if (DateTime.UtcNow.AddMinutes(5) < loginResponse.Expires)
            {
                return (loginResponse.accessToken, loginResponse.Expires.ToLocalTime().ToString("HH:mm:ss"));
            }

            return await RefreshToken(loginResponse.refreshToken);
        }

        return await Login();
    }
    
    private async Task<(string Token, string Expires)> Login()
    {
        var email = _config.Email;

        if (string.IsNullOrWhiteSpace(email))
        {
            email = AnsiConsole.Prompt(new TextPrompt<string>("email").PromptStyle("red"));
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]email {_config.Email}[/]");
        }

        _config.Email = email;
        await _config.Save();
        
        var password = AnsiConsole.Prompt(new TextPrompt<string>("password").Secret().PromptStyle("red"));
        var loginResponse = await Login(email, password);
        await File.WriteAllBytesAsync(FileName, JsonSerializer.SerializeToUtf8Bytes(loginResponse));

        return (loginResponse.accessToken, loginResponse.Expires.ToLocalTime().ToString("HH:mm:ss"));
    }

    private async Task<LoginResponse> Login(string userEmail, string password)
    {
        var request = new LoginRequest
        {
            Email = userEmail, 
            Password = password
        };

        var response = await _client.PostAsJsonAsync("api/auth/login", request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            loginResponse!.Expires = DateTime.UtcNow.AddSeconds(loginResponse.expiresIn);
            return loginResponse;
        }

        throw new Exception($"Could not authenticate to dol integration api {_client.BaseAddress}");
    }
    
    private async Task<(string Token, string Expires)> RefreshToken(string refreshToken)
    {
        var request = new { refreshToken };

        var response = await _client.PostAsJsonAsync("api/auth/refresh", request);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            loginResponse!.Expires = DateTime.UtcNow.AddSeconds(loginResponse.expiresIn);
            await File.WriteAllBytesAsync(FileName, JsonSerializer.SerializeToUtf8Bytes(loginResponse));
            return (loginResponse.accessToken, loginResponse.Expires.ToLocalTime().ToString("HH:mm:ss"));
        }

        return await Login();
    }
}