using System.Text.Json.Serialization;

namespace dol.IoT.Integrator.Cli.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeviceType
{
    Unknown = 0,
    IDOL63 = 1,
    IDOL65 = 2,
    IDOL64 = 3,
}

public static class DeviceTypeExtensions
{
    public static string ToDeviceTypeString(this DeviceType deviceType)
        => deviceType switch
        {
            DeviceType.IDOL64 => "iDOL64",
            DeviceType.IDOL63 => "iDOL63",
            DeviceType.IDOL65 => "iDOL65",
            _ => ""
        };

    public static DeviceType ToDeviceType(
        this string s)
        => s.ToLower() switch
        {
            "idol64" => DeviceType.IDOL64,
            "idol63" => DeviceType.IDOL63,
            "idol65" => DeviceType.IDOL65,
            _ => DeviceType.Unknown
        };
}
