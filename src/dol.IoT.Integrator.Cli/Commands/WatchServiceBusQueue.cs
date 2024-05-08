using Azure.Messaging.ServiceBus;
using Cocona;
using dol.IoT.Integrator.Cli.ApiIntegration;
using dol.IoT.Integrator.Cli.Models;
using dol.IoT.Integrator.Cli.Util;
using Spectre.Console;

namespace dol.IoT.Integrator.Cli.Commands;

public static class WatchServiceBusQueue
{
    public static void AddWatchServiceBusQueue(
        this CoconaApp app,
        CancellationTokenSource cts)
    {
        app.AddCommand("watch-queue", async (
            [Option('q', Description = "queue type")] QueueType? queueType,
            IIntegratorApiClient integrationApiClient) =>
        {
            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            var connections = await integrationApiClient.GetIntegratorServiceBusQueueConnections();
            if (connections is null)
            {
                AnsiConsole.MarkupLine("Make sure your user is connected to an integrator account");
                return;
            }

            if (queueType is null or QueueType.Data)
            {
                await PeakDataQueue(connections, clientOptions, cts);
            }
            else
            {
                await PeakStatusQueue(connections, clientOptions, cts);
            }
        }).WithIntegratorDescription(
            "Peek data queue of your connected integrator, writes a new message each second to console. The messages is not consumed from the queue.",
            hasSideEffects: false);
    }

    private static async Task PeakStatusQueue(
        QueueConnectionInfoResponse connections,
        ServiceBusClientOptions clientOptions,
        CancellationTokenSource cts)
    {
        AnsiConsole.WriteLine($"{connections.StatusQueueName} has {connections.StatusQueueMessageCount} messages not yet consumed");
        AnsiConsole.WriteLine($"{connections.StatusQueueName} has {connections.StatusQueueDeadLetterCount} deadletter messages");

        await using var serviceBusClient = new ServiceBusClient(connections.StatusQueueConnection, clientOptions);
        await using var receiver = serviceBusClient.CreateReceiver(connections.StatusQueueName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        await PeekInQueue(cts, receiver);
    }

    private static async Task PeakDataQueue(QueueConnectionInfoResponse connections, ServiceBusClientOptions clientOptions, CancellationTokenSource cts)
    {
        AnsiConsole.WriteLine($"{connections.DataQueueName} has {connections.DataQueueMessageCount} messages not yet consumed");
        AnsiConsole.WriteLine($"{connections.DataQueueName} has {connections.DataQueueDeadLetterCount} deadletter messages");

        await using var serviceBusClient = new ServiceBusClient(connections.DataQueueConnection, clientOptions);
        await using var receiver = serviceBusClient.CreateReceiver(connections.DataQueueName, new ServiceBusReceiverOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });

        await PeekInQueue(cts, receiver);
    }

    private static async Task PeekInQueue(
        CancellationTokenSource cts,
        ServiceBusReceiver receiver)
    {
        try
        {
            while (!cts.IsCancellationRequested)
            {
                var msg = await receiver.PeekMessageAsync(cancellationToken: cts.Token);
                if (msg is null)
                {
                    AnsiConsole.WriteLine("No messages in queue, waiting 30 sec...");
                    await Task.Delay(30_000, cts.Token);
                }
                else
                {
                    AnsiConsoleExt.WriteJson(msg.Body.ToString());
                    await Task.Delay(1000, cts.Token);
                }
            }
        }
        catch (TaskCanceledException)
        {
            AnsiConsole.WriteLine("exiting...");
        }
        catch (Exception e)
        {
            AnsiConsole.WriteLine(e.Message);
        }
    }

    public enum QueueType
    {
        Data, Status
    }
}