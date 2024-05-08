using System.Text.Json.Serialization;

namespace dol.IoT.Integrator.Cli.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoraSensorType
{
    Unknown = 0,
    DOL16 = 1, // Light
    DOL53 = 2, // Ammonia
    DOL90 = 3, // Water
    iDOL139 = 4 // Temp, Hum, CO2
}

