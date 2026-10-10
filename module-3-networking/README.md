# Overview

I created a small Rule of Lords LAN Coordinator Prototype in C#. The program uses a client and server that communicate over a TCP connection. The client can join a lobby, request the current lobby state, and change a player's ready status.

I wanted to build something small that could also be useful for my Rule of Lords project later. My main goal was to understand how client-server communication works and how two programs can send requests and responses over a network.

The client and server use JSON messages to send information. The server keeps the current lobby and player ready status in memory while it is running.

[Software Demo Video] https://youtu.be/F2O67Zz0hIo 

# Network Communication

The project uses a client-server model with TCP.

The server listens on port 5000 and waits for a client to connect. The client connects to the server and sends requests as JSON messages. The server reads the request, performs the requested action, and sends a response back to the client.

The program supports three request types:

- `JOIN` adds a player to the lobby.
- `GET_STATE` returns the current players and their ready status.
- `SET_READY` changes the ready status of a player.

The client can send several requests during the same connection. The lobby data is only stored in memory and is cleared when the server stops.

# Development Environment

I used Visual Studio Code on Ubuntu to write and test the program. I also used the terminal, Git, and GitHub to manage and publish the project.

The client and server are written in C# and run with .NET 10. I used the built-in .NET networking libraries for TCP communication and `System.Text.Json` for JSON messages.

# Useful Websites

- [TcpListener Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener)

- [System.Text.Json Documentation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)

- [C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)

# Future Work

- Allow multiple clients to stay connected at the same time.

- Test the client and server on two different computers on the same local network.

- Add more lobby information if the prototype is expanded later.