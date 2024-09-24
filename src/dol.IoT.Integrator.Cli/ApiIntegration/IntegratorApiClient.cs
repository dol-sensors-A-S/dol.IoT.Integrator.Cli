using System.Net.Http.Json;
using dol.IoT.Models.Public.DeviceApi;
using dol.IoT.Models.Public.ManagementApi;

namespace dol.IoT.Integrator.Cli.ApiIntegration;

public class IntegratorApiClient : IIntegratorApiClient
{
    private readonly HttpClient _client;

    public IntegratorApiClient(HttpClient httpClient, Config config)
    {
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        var uriString = config.Env.BaseUrl();
        httpClient.BaseAddress = new Uri(uriString);
        _client = httpClient;
    }

    public async Task<(bool Success, string Response)> ClaimDevice(ClaimDeviceRequest request)
    {
        var result = await _client.PostAsJsonAsync("api/devices/claim", request);

        return (result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string Response)> RemoveDeviceClaim(string mac)
    {
        var result = await _client.DeleteAsync($"api/devices/claim/{mac}");

        return (result.IsSuccessStatusCode, await result.Content.ReadAsStringAsync());
    }

    public async Task<QueueConnectionInfoResponse?> GetIntegratorServiceBusQueueConnections()
    {
        var result = await _client.GetFromJsonAsync<QueueConnectionInfoResponse>("api/management/queue");
        return result;
    }

    public async Task<(bool Success, string Response)> AddSensorToDevice(string mac, AddSensorToDeviceRequest request)
    {
        var response = await _client.PostAsJsonAsync($"api/devices/{mac}/sensor", request);
        var content = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, content);
    }

    public async Task<(bool Success, string Response)> RemoveSensorFromDevice(
        string mac,
        string devEui)
    {
        var response = await _client.DeleteAsync($"api/devices/{mac}/sensor/{devEui}");
        var content = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, content);
    }

    public async Task<string?> GetDevice(
        string mac)
    {
        var resp = await _client.GetAsync($"api/devices/{mac}");
        if (resp.IsSuccessStatusCode)
            return await resp.Content.ReadAsStringAsync();

        return null;
    }

    public async Task<string?> GetAllDevices(
        int page,
        int pageSize,
        string? owner = null)
    {
        var result = await _client.GetAsync($"api/devices?page={page}&{pageSize}");
        if (result.IsSuccessStatusCode)
            return await result.Content.ReadAsStringAsync();

        return null;
    }

    public async Task<(bool Success, string Response)> UnclaimDevice(
        string mac)
    {
        var result = await _client.DeleteAsync($"/api/devices/claim/{mac}");
        var content = await result.Content.ReadAsStringAsync();
        return (result.IsSuccessStatusCode, content);
    }

    public async Task<(bool Success, string Response)> UpdateWiredSensors(string mac, UpdateWiredSensorsRequest request)
    {
        var result = await _client.PutAsJsonAsync($"/api/devices/{mac}/wiredSensor", request);
        var content = await result.Content.ReadAsStringAsync();
        return (result.IsSuccessStatusCode, content);
    }
}