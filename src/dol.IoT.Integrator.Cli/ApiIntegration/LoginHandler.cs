using System.Net.Http.Headers;

namespace dol.IoT.Integrator.Cli.ApiIntegration;

public class LoginHandler(ILoginService loginService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var (token, expires) = await loginService.GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}