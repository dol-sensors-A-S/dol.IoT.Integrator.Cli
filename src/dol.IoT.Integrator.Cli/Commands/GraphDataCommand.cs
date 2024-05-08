using System.Globalization;
using System.Text.Json;
using Cocona;
using CsvHelper;
using dol.IoT.Integrator.Cli.Util;
using ScottPlot;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace dol.IoT.Integrator.Cli.Commands;

public static class GraphDataCommand
{
    private static JsonSerializerOptions _options = new() { AllowTrailingCommas = true };

    public static void AddGraphData(this CoconaApp app, CancellationTokenSource cts)
    {
        app.AddCommand("graph-data", async (
            [Argument(Description = "File name of your data results")]
            string resultFileName) =>
        {
            var jsonResult = await ReadResultsFromFile(resultFileName);

            var listDictionary = new Dictionary<string, List<DataPoint>>();
            foreach (var r in jsonResult!.results)
            {
                var key = $"{r.deviceId}-{r.sensorId}-{r.type}";
                if (!listDictionary.ContainsKey(key))
                {
                    listDictionary.Add(key, []);
                }

                listDictionary[key].Add(new DataPoint(r.deviceId, r.sensorId, r.sensorName, r.value, r.unit, r.type, r.timestamp));
            }

            foreach (var data in listDictionary.Values)
            {
                var fst = data.First();
                var key = $"{fst.deviceId}-{fst.sensorName}-{fst.type}";

                var plot = new Plot();
                var dateTimeOffsets = data.Select(x => DateTimeOffset.FromUnixTimeSeconds(x.timestamp).LocalDateTime).ToArray();
                var dataPoints = data.Select(x => x.value).ToArray();
                var scatter = plot.Add.Scatter(dateTimeOffsets, dataPoints);
                scatter.Label = $"{fst.type} {fst.unit}";
                scatter.LineWidth = 3;
                scatter.Smooth = true;
                scatter.MarkerStyle = MarkerStyle.None;

                plot.ShowLegend(Alignment.UpperRight);

                plot.Axes.DateTimeTicksBottom();
                plot.XLabel("Time");
                plot.YLabel(fst.unit);
                plot.Title(key);

                var imgPath = $"{key}.png";
                var res = plot.SavePng(imgPath, 1200, 900);
            }
        }).WithIntegratorDescription("Creates graphs over a given data file", hasSideEffects: false);
    }

    private static async Task<JsonResults?> ReadResultsFromFile(string resultFileName)
    {
        if (resultFileName.EndsWith(".json"))
        {
            await using var fileStream = File.OpenRead(resultFileName);
            var jsonResult = await JsonSerializer.DeserializeAsync<JsonResults>(fileStream, _options);
            return jsonResult!;
        }

        if (resultFileName.EndsWith(".csv"))
        {
            using var streamReader = new StreamReader(resultFileName);
            using var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<SensorDataCosmosMap>();
            var records = csv.GetRecords<SensorDataCosmos>().ToArray();
            return new JsonResults { results = records };
        }

        throw new ArgumentException($"Unknown file type {resultFileName}");
    }
}