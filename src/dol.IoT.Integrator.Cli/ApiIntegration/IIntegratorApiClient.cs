using dol.IoT.Integrator.Cli.Models;

namespace dol.IoT.Integrator.Cli.ApiIntegration;

public interface IIntegratorApiClient
{
    Task<(bool Success, string Response)> ClaimDevice(ClaimDeviceRequest request);
    Task<(bool Success, string Response)> RemoveDeviceClaim(string mac);
    Task<QueueConnectionInfoResponse?> GetIntegratorServiceBusQueueConnections();
    Task<(bool Success, string Response)> AddSensorToDevice(string mac, AddSensorRequest request);
    Task<(bool Success, string Response)>  RemoveSensorFromDevice(string mac, string devEui);
    Task<string?> GetDevice(string mac);
    Task<string?> GetAllDevices(int page, int pageSize, string? owner = null);

}