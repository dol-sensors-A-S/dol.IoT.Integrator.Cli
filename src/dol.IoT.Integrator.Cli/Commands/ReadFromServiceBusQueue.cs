using System.Globalization;
using Azure.Messaging.ServiceBus;
using Cocona;
using CsvHelper;
using CsvHelper.Configuration;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class ReadFromServiceBusQueue
{
    public static void AddReadFromServiceBusQueue(this CoconaApp app, CancellationTokenSource cts)
    {
        app.AddCommand("read-queue", async (
            [Argument(Description = "Saves the read data in this file. Supports .json and .csv files")]
            string resultFileName,
            IIntegratorApiClient integrationApiClient) =>
        {
            var connections = await integrationApiClient.GetIntegratorServiceBusQueueConnections();
            if (connections is null)
            {
                AnsiConsole.MarkupLine("Make sure your user is connected to an integrator account");
                return;
            }

            var fileType = GetFileType(resultFileName);
            AnsiConsole.WriteLine($"Reading from service bus queue {connections.DataQueueName}, saving data to {fileType} file {resultFileName}");

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            await using var serviceBusClient = new ServiceBusClient(connections.DataQueueConnection, clientOptions);
            await using var receiver = serviceBusClient.CreateReceiver(connections.DataQueueName, new ServiceBusReceiverOptions
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });

            if (fileType.Equals("json"))
            {
                await WriteJson(receiver, resultFileName, cts);
            }
            else
            {
                await WriteCsv(receiver, resultFileName, cts);
            }
        }).WithIntegratorDescription("Consume data from your connected data queue and write to file", hasSideEffects: true);
    }

    private static async Task WriteCsv(ServiceBusReceiver receiver, string resultFileName, CancellationTokenSource cts)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            Mode = CsvMode.Escape,
            NewLine = Environment.NewLine
        };

        try
        {
            await using var writer = new StreamWriter(resultFileName);
            await using var csv = new CsvWriter(writer, config);
            csv.Context.RegisterClassMap<SensorDataCosmosMap>();

            csv.WriteHeader<SensorDataCosmos>();
            csv.NextRecord();

            while (!cts.IsCancellationRequested)
            {
                var msgs = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(5), cancellationToken: cts.Token);
                AnsiConsole.MarkupLine($"Received data x{msgs.Count}");
                if (msgs.Count == 0)
                {
                    return;
                }

                List<SensorDataCosmos> messages = [];
                foreach (var msg in msgs)
                {
                    var message = msg.Body.ToObjectFromJson<SensorDataCosmos>();

                    messages.Add(message);
                }

                await csv.WriteRecordsAsync(messages, cts.Token);
            }
        }
        finally
        {
            AnsiConsole.WriteLine("data export done");
        }
    }

    private static async Task WriteJson(ServiceBusReceiver receiver, string resultFileName, CancellationTokenSource cts)
    {
        await using var fileStream = File.Create(resultFileName);
        fileStream.Write("""{ "results": ["""u8);
        var comma = ","u8.ToArray();
        var addComma = false;

        try
        {
            while (!cts.IsCancellationRequested)
            {
                var msgs = await receiver.ReceiveMessagesAsync(maxMessages: 100, maxWaitTime: TimeSpan.FromSeconds(2), cancellationToken: cts.Token);
                AnsiConsole.MarkupLine($"Received data x{msgs.Count}");
                if (msgs.Count == 0)
                {
                    return;
                }

                foreach (var msg in msgs)
                {
                    if (addComma)
                    {
                        fileStream.Write(comma);
                    }

                    addComma = true;
                    fileStream.Write(msg.Body);
                }
            }
        }
        finally
        {
            fileStream.Write(""" ]}"""u8);
        }
    }

    private static string GetFileType(string resultFileName)
    {
        return resultFileName switch
        {
            _ when resultFileName.EndsWith(".csv") => "csv",
            _ when resultFileName.EndsWith(".json") => "json",
            _ => throw new ArgumentException($"Unknown file type in filename {resultFileName}"),
        };
    }
}

public sealed class SensorDataCosmosMap : ClassMap<SensorDataCosmos>
{
    public SensorDataCosmosMap()
    {
        Map(m => m.id).Index(0).Name("id");
        Map(m => m.deviceId).Index(1).Name("deviceId");
        Map(m => m.sensorId).Index(2).Name("sensorId");
        Map(m => m.sensorName).Index(3).Name("sensorName");
        Map(m => m.value).Index(4).Name("value");
        Map(m => m.type).Index(5).Name("type");
        Map(m => m.unit).Index(6).Name("unit");
        Map(m => m.timestamp).Index(7).Name("timestamp");
        Map(m => m.count).Index(8).Name("count");
        Map(m => m.minWeight).Index(9).Name("minWeight");
        Map(m => m.maxWeight).Index(10).Name("maxWeight");
        Map(m => m.timespan).Index(11).Name("timespan");
        Map(m => m.sd).Index(12).Name("sd");
        Map(m => m.skewness).Index(13).Name("skewness");
        Map(m => m.lastCycleCount).Index(14).Name("lastCycleCount");
        Map(m => m.CountDelta).Index(15).Name("countDelta");
        Map(m => m.lastCycleMeanWeight).Index(16).Name("lastCycleMeanWeight");
        Map(m => m.lastCycleMinWeight).Index(17).Name("lastCycleMinWeight");
        Map(m => m.lastCycleMaxWeight).Index(18).Name("lastCycleMaxWeight");
        Map(m => m.lastCycleStandardDeviation).Index(19).Name("lastCycleStandardDeviation");
        Map(m => m.lastCycleSkewness).Index(20).Name("lastCycleSkewness");
    }
}

public class SensorDataCosmos
{
    public string id { get; set; } = "";
    public string deviceId { get; set; } = "";
    public string sensorId { get; set; } = "";
    public string sensorName { get; set; } = "";
    public decimal value { get; set; }
    public string type { get; set; } = "";
    public string unit { get; set; } = "";
    public long timestamp { get; set; }

    public int? count { get; set; }
    public double? minWeight { get; set; }
    public double? maxWeight { get; set; }
    public long? timespan { get; set; }
    public double? sd { get; set; }
    public double? skewness { get; set; }
    public int? lastCycleCount { get; set; }
    public int? CountDelta { get; set; }
    public decimal? lastCycleMeanWeight { get; set; }
    public double? lastCycleMinWeight { get; set; }
    public double? lastCycleMaxWeight { get; set; }
    public double? lastCycleStandardDeviation { get; set; }
    public double? lastCycleSkewness { get; set; }
}

public class JsonResults
{
    public SensorDataCosmos[] results { get; set; } = [];
}

public record DataPoint(string deviceId, string sensorId, string sensorName, decimal value, string unit, string type, long timestamp);