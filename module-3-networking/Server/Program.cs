using System.Net;
using System.Net.Sockets;
using System.Text.Json;

// Keep the lobby state in memory while the server is running.
List<PlayerState> players = new();

TcpListener listener = new(IPAddress.Loopback, 5000);
listener.Start();

Console.WriteLine("Server is listening on port 5000...");

while (true)
{
    // Wait for a client to connect.
    using TcpClient client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Client connected.");

    using NetworkStream stream = client.GetStream();
    using StreamReader reader = new(stream);
    using StreamWriter writer = new(stream) { AutoFlush = true };

    // Handle requests until this client disconnects.
    while (true)
    {
        string? jsonRequest = await reader.ReadLineAsync();

        if (jsonRequest == null)
        {
            Console.WriteLine("Client disconnected.");
            break;
        }

        RequestMessage? request;

        try
        {
            request = JsonSerializer.Deserialize<RequestMessage>(jsonRequest);
        }
        catch (JsonException)
        {
            await SendResponse(
                writer,
                new ResponseMessage(false, "Invalid JSON.")
            );

            continue;
        }

        if (request == null)
        {
            await SendResponse(
                writer,
                new ResponseMessage(false, "Invalid request.")
            );

            continue;
        }

        if (request.Type == "JOIN")
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                await SendResponse(
                    writer,
                    new ResponseMessage(false, "Player name is required.")
                );

                continue;
            }

            bool alreadyJoined = players.Any(
                p => string.Equals(
                    p.Name,
                    request.Name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (alreadyJoined)
            {
                await SendResponse(
                    writer,
                    new ResponseMessage(false, "Player already joined.")
                );

                continue;
            }

            players.Add(new PlayerState(request.Name, false));

            await SendResponse(
                writer,
                new ResponseMessage(
                    true,
                    $"{request.Name} joined the lobby."
                )
            );

            Console.WriteLine($"{request.Name} joined.");
        }
        else if (request.Type == "GET_STATE")
        {
            await SendResponse(
                writer,
                new ResponseMessage(
                    true,
                    "Current lobby state.",
                    players
                )
            );

            Console.WriteLine("Lobby state requested.");
        }
        else if (request.Type == "SET_READY")
        {
            PlayerState? player = players.FirstOrDefault(
                p => string.Equals(
                    p.Name,
                    request.Name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (player == null || !request.Ready.HasValue)
            {
                await SendResponse(
                    writer,
                    new ResponseMessage(false, "Player not found.")
                );

                continue;
            }

            int index = players.IndexOf(player);

            players[index] = new PlayerState(
                player.Name,
                request.Ready.Value
            );

            await SendResponse(
                writer,
                new ResponseMessage(
                    true,
                    $"{player.Name} ready status changed."
                )
            );

            Console.WriteLine(
                $"{player.Name} ready: {request.Ready.Value}"
            );
        }
        else
        {
            await SendResponse(
                writer,
                new ResponseMessage(false, "Unknown request type.")
            );
        }
    }
}

// Convert a response to JSON and send it to the client.
static async Task SendResponse(
    StreamWriter writer,
    ResponseMessage response)
{
    string json = JsonSerializer.Serialize(response);
    await writer.WriteLineAsync(json);
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