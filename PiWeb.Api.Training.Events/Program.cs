#region copyright

/* * * * * * * * * * * * * * * * * * * * * * * * * */
/* Carl Zeiss Industrielle Messtechnik GmbH        */
/* Softwaresystem PiWeb                            */
/* (c) Carl Zeiss 2024                             */
/* * * * * * * * * * * * * * * * * * * * * * * * * */

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PiWeb.Api.Training.Events.EventDtos;

namespace PiWeb.Api.Training.Events;

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("PiWeb Server Events - Training Client");
        Console.WriteLine("=====================================");

        Console.WriteLine();
        ConsoleUI.WriteInfo("Find PiWeb Server (1/4)");

        // Get the Server URL from user input, e.g. copied from the PiWeb Server UI "Autodiscovery address"
        if (!TryGetServerUrl(out var serverUrl))
            return;

        Console.WriteLine();
        ConsoleUI.WriteInfo($"Contacting {serverUrl} (2/4)");

        // Check which authentication modes are available
        var authenticationMode = await Authentication.GetAuthenticationMode(serverUrl);

        Console.WriteLine();
        ConsoleUI.WriteInfo("Creating event connection (3/4)");

        // Try to connect to the /events endpoint
        if (!TryCreateConnection(serverUrl, authenticationMode, out var connection))
            return;

        // Subscribe to the events you want to be informed about
        SubscribeEvents(connection);

        if (!await TryConnect(connection))
            return;

        Console.WriteLine();
        ConsoleUI.WriteInfo("Listening for events... (READY)");

        // Now simply wait for any incoming events
        await Task.Delay(Timeout.Infinite);
    }

    private static bool TryGetServerUrl([NotNullWhen(true)] out Uri? serverUrl)
    {
        var serverUrlString = ConsoleUI.InputText("Server URL with port");

        try
        {
            serverUrl = new Uri(serverUrlString);
            return true;
        }
        catch (Exception exception)
        {
            ConsoleUI.WriteError(exception.ToString());
            serverUrl = null;
            return false;
        }
    }

    private static bool TryCreateConnection(Uri serverUrl, Authentication.AuthenticationMode authenticationMode,
        [NotNullWhen(true)] out HubConnection? connection)
    {
        try
        {
            // Use the SignalR client to connect while specifying the chosen authentication mode
            connection = new HubConnectionBuilder()
                .WithUrl(new Uri(serverUrl, "/events"), options => Authentication.SetConnectionOptions(authenticationMode, options))
                .Build();
            return true;
        }
        catch (Exception exception)
        {
            ConsoleUI.WriteError(exception.ToString());
            connection = null;
            return false;
        }
    }

    private static void SubscribeEvents(HubConnection connection)
    {
        // Example: subscribe to the PartCreatedEvent which is sent after a part was created.
        // On receiving such an event, print a simple message to the console, using information from the event.
        connection.On<PartCreatedEvent>("PartCreated", e => Console.WriteLine($"PartCreated: {e.PartUuid} as child of part {e.ParentPartUuid}"));
        connection.On<PartDeletedEvent>("PartDeleted", e => Console.WriteLine($"PartDeleted: {e.PartUuid} as child of part {e.ParentPartUuid}"));
        connection.On<PartModifiedEvent>("PartModified", e => Console.WriteLine($"PartModified: {e.PartUuid}"));
        connection.On<PartMovedEvent>("PartMoved", e => Console.WriteLine($"PartMoved: {e.PartUuid} from {e.FromPath} to {e.ToPath}"));

        connection.On<CharacteristicCreatedEvent>("CharacteristicCreated", e => Console.WriteLine($"CharacteristicCreated: {e.CharacteristicUuid} as child of part or characteristic {e.ParentUuid}"));
        connection.On<CharacteristicDeletedEvent>("CharacteristicDeleted", e => Console.WriteLine($"CharacteristicDeleted: {e.CharacteristicUuid} as child of part {e.ParentPartUuid}"));
        connection.On<CharacteristicModifiedEvent>("CharacteristicModified", e => Console.WriteLine($"CharacteristicModified: {e.CharacteristicUuid} as child of part {e.ParentPartUuid}"));
        connection.On<CharacteristicMovedEvent>("CharacteristicMoved", e => Console.WriteLine($"CharacteristicMoved: {e.CharacteristicUuid} from {e.FromPath} to {e.ToPath}"));

        connection.On<MeasurementCreatedEvent>("MeasurementCreated", e => Console.WriteLine($"MeasurementCreated: {e.MeasurementUuid} for part {e.PartUuid}"));
        connection.On<MeasurementDeletedEvent>("MeasurementDeleted", e => Console.WriteLine($"MeasurementDeleted: {e.MeasurementUuid} for part {e.PartUuid}"));
        connection.On<MeasurementModifiedEvent>("MeasurementModified", e => Console.WriteLine($"MeasurementModified: {e.MeasurementUuid} for part {e.PartUuid}"));

        connection.On<RawDataCreatedEvent>("RawDataCreated", e => Console.WriteLine($"RawDataCreated: {e.Filename} with key {e.Key} for {e.RawDataEntity} {e.TargetUuid}"));
        connection.On<RawDataDeletedEvent>("RawDataDeleted", e => Console.WriteLine($"RawDataDeleted: {e.Filename} with key {e.Key} for {e.RawDataEntity} {e.TargetUuid}"));
        connection.On<RawDataModifiedEvent>("RawDataModified", e => Console.WriteLine($"RawDataModified: from {e.OldFilename} to {e.NewFilename} with key {e.Key} for {e.RawDataEntity} {e.TargetUuid}"));
    }

    private static async Task<bool> TryConnect(HubConnection connection)
    {
        try
        {
            await connection.StartAsync();
            ConsoleUI.WriteSuccess("Connected");
            return true;
        }
        catch (Exception exception)
        {
            ConsoleUI.WriteError(exception.ToString());
            return false;
        }
    }
}