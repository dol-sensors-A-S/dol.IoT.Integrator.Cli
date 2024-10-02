using dol.IoT.Models.Public.DeviceApi;
using dol.IoT.Models.Public.ManagementApi;

namespace dol.IoT.Integrator.Cli.ApiIntegration;

public interface IIntegratorApiClient
{
    Task<(bool Success, string Response)> ClaimDevice(ClaimDeviceRequest request);
    Task<QueueConnectionInfoResponse?> GetIntegratorServiceBusQueueConnections();
    Task<(bool Success, string Response)> AddSensorToDevice(string mac, AddSensorToDeviceRequest request);
    Task<(bool Success, string Response)>  RemoveSensorFromDevice(string mac, string devEui);
    Task<string?> GetDevice(string mac);
    Task<string?> GetAllDevices(int page, int pageSize, string? owner = null);
    Task<(bool Success, string Response)> UnclaimDevice(string mac);
    Task<(bool Success, string Response)> UpdateWiredSensors(string mac, UpdateWiredSensorsRequest updateWiredSensorsRequest);
    Task<(bool Success, string Response)> RequeueData(RequeueDeviceDataRequest request);
    Task<(bool Success, string Response)> UpdateSensorToDevice(string mac, UpdateSensorToDeviceRequest updateSensorRequest);
}