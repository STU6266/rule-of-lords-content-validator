using System.Net.Sockets;
using System.Text.Json;

using TcpClient client = new();

try
{
    await client.ConnectAsync("127.0.0.1", 5000);
}
catch (SocketException)
{
    Console.WriteLine("Could not connect to the server.");
    return;
}

Console.WriteLine("Connected to server.");

using NetworkStream stream = client.GetStream();
using StreamReader reader = new(stream);
using StreamWriter writer = new(stream) { AutoFlush = true };

// Keep the client running until Exit is selected.
while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Join lobby");
    Console.WriteLine("2 - Get lobby state");
    Console.WriteLine("3 - Set ready status");
    Console.WriteLine("4 - Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    if (choice == "4")
    {
        Console.WriteLine("Disconnected.");
        break;
    }

    RequestMessage request;

    if (choice == "1")
    {
        Console.Write("Enter your player name: ");
        string? playerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Console.WriteLine("Player name is required.");
            continue;
        }

        request = new RequestMessage("JOIN", playerName);
    }
    else if (choice == "2")
    {
        request = new RequestMessage("GET_STATE", null);
    }
    else if (choice == "3")
    {
        Console.Write("Enter player name: ");
        string? playerName = Console.ReadLine();

        Console.Write("Ready? (true/false): ");
        string? readyInput = Console.ReadLine();

        if (!bool.TryParse(readyInput, out bool ready))
        {
            Console.WriteLine("Invalid ready value.");
            continue;
        }

        request = new RequestMessage(
            "SET_READY",
            playerName,
            ready
        );
    }
    else
    {
        Console.WriteLine("Invalid option.");
        continue;
    }

    // Send the request as one line of JSON.
    string jsonRequest = JsonSerializer.Serialize(request);
    await writer.WriteLineAsync(jsonRequest);

    Console.WriteLine($"Sent: {jsonRequest}");

    string? jsonResponse = await reader.ReadLineAsync();

    if (jsonResponse == null)
    {
        Console.WriteLine("Server disconnected.");
        break;
    }

    ResponseMessage? response =
        JsonSerializer.Deserialize<ResponseMessage>(jsonResponse);

    Console.WriteLine($"Success: {response?.Success}");
    Console.WriteLine($"Message: {response?.Message}");

    if (response?.Players != null)
    {
        Console.WriteLine("Players:");

        if (response.Players.Count == 0)
        {
            Console.WriteLine("- Lobby is empty.");
        }

        foreach (PlayerState player in response.Players)
        {
            Console.WriteLine(
                $"- {player.Name} | Ready: {player.Ready}"
            );
        }
    }
}

record RequestMessage(
    string Type,
    string? Name,
    bool? Ready = null
);

record ResponseMessage(
    bool Success,
    string Message,
    List<PlayerState>? Players = null
);

record PlayerState(string Name, bool Ready);