namespace dol.IoT.Integrator.Cli.Models;

public class ClaimDeviceRequest
{
    public string MacAddress { get; set; } = "";
    public string Key { get; set; } = "";
    public string DeviceType { get; set; } = "";
    public string? Owner { get; set; }
    public string DeviceName { get; set; } = "";
}