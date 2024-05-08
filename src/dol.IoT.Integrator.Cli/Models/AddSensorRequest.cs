namespace dol.IoT.Integrator.Cli.Models;

public class AddSensorRequest
{
    public string DevEui { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int SampleRate { get; set; }
}