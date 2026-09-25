using Microsoft.Data.Sqlite;

string connectionString = "Data Source=rule_of_lords.db;Foreign Keys=True";
using SqliteConnection connection = new(connectionString);
connection.Open();

Console.WriteLine("Rule of Lords Persistence Prototype");

while (true)
{
    Console.WriteLine("\n1 - Show players");
    Console.WriteLine("2 - Add player");
    Console.WriteLine("3 - Update player");
    Console.WriteLine("4 - Delete player");
    Console.WriteLine("0 - Exit");
    Console.Write("Choose an option: ");

    switch (Console.ReadLine())
    {
        case "1": ShowPlayers(connection); break;
        case "2": AddPlayer(connection); break;
        case "3": UpdatePlayer(connection); break;
        case "4": DeletePlayer(connection); break;
        case "0": return;
        default: Console.WriteLine("Invalid option."); break;
    }
}

static void ShowPlayers(SqliteConnection connection)
{
    // Display player data with the related game and race names.
    // The JOIN replaces stored IDs with readable values.
    string sql = """
        SELECT players.player_id, players.name, games.name,
               races.name, players.gold, players.troops
        FROM players
        JOIN games ON players.game_id = games.game_id
        JOIN races ON players.race_id = races.race_id
        ORDER BY players.player_id;
        """;

    using SqliteCommand command = new(sql, connection);
    using SqliteDataReader reader = command.ExecuteReader();

    Console.WriteLine("\nPlayers:");
    while (reader.Read())
    {
        Console.WriteLine(
            $"{reader.GetInt32(0)} | {reader.GetString(1)} | " +
            $"{reader.GetString(2)} | {reader.GetString(3)} | " +
            $"Gold: {reader.GetInt32(4)} | Troops: {reader.GetInt32(5)}");
    }
}

static void AddPlayer(SqliteConnection connection)
{
    // Add a new player to an existing game.
    Console.Write("Player name: ");
    string? name = Console.ReadLine();
    Console.Write("Game ID: ");
    string? gameInput = Console.ReadLine();

    // Showing the race list makes the database IDs easier to use.
    using (SqliteCommand raceCommand = new("SELECT race_id, name FROM races ORDER BY race_id;", connection))
    using (SqliteDataReader reader = raceCommand.ExecuteReader())
    {
        Console.WriteLine("Available race IDs:");
        while (reader.Read())
            Console.WriteLine($"{reader.GetInt32(0)} - {reader.GetString(1)}");
    }

    Console.Write("Race ID: ");
    string? raceInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(name) ||
        !int.TryParse(gameInput, out int gameId) ||
        !int.TryParse(raceInput, out int raceId))
    {
        Console.WriteLine("Invalid player information.");
        return;
    }

    string sql = """
        INSERT INTO players (game_id, race_id, name)
        VALUES (@gameId, @raceId, @name);
        """;

    using SqliteCommand command = new(sql, connection);
    command.Parameters.AddWithValue("@gameId", gameId);
    command.Parameters.AddWithValue("@raceId", raceId);
    command.Parameters.AddWithValue("@name", name);

    try
    {
        command.ExecuteNonQuery();
        Console.WriteLine("Player added.");
    }
    catch (SqliteException ex)
    {
        Console.WriteLine($"Could not add player: {ex.Message}");
    }
}

static void UpdatePlayer(SqliteConnection connection)
{
    // Load the current values first so blank input can keep them unchanged.
    Console.Write("Player ID: ");
    if (!int.TryParse(Console.ReadLine(), out int playerId))
    {
        Console.WriteLine("Invalid player ID.");
        return;
    }

    string selectSql = "SELECT name, game_id, race_id, gold, troops FROM players WHERE player_id = @id;";
    using SqliteCommand selectCommand = new(selectSql, connection);
    selectCommand.Parameters.AddWithValue("@id", playerId);

    string name;
    int gameId, raceId, gold, troops;
    using (SqliteDataReader reader = selectCommand.ExecuteReader())
    {
        if (!reader.Read())
        {
            Console.WriteLine("Player not found.");
            return;
        }

        name = reader.GetString(0);
        gameId = reader.GetInt32(1);
        raceId = reader.GetInt32(2);
        gold = reader.GetInt32(3);
        troops = reader.GetInt32(4);
    }

    Console.WriteLine("Press Enter to keep the current value.");
    Console.Write($"Name [{name}]: ");
    string? nameInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nameInput)) name = nameInput;

    gameId = ReadIntOrKeep("Game ID", gameId);
    raceId = ReadIntOrKeep("Race ID", raceId);
    gold = ReadIntOrKeep("Gold", gold);
    troops = ReadIntOrKeep("Troops", troops);

    string updateSql = """
        UPDATE players
        SET name = @name, game_id = @gameId, race_id = @raceId,
            gold = @gold, troops = @troops
        WHERE player_id = @id;
        """;

    using SqliteCommand updateCommand = new(updateSql, connection);
    updateCommand.Parameters.AddWithValue("@name", name);
    updateCommand.Parameters.AddWithValue("@gameId", gameId);
    updateCommand.Parameters.AddWithValue("@raceId", raceId);
    updateCommand.Parameters.AddWithValue("@gold", gold);
    updateCommand.Parameters.AddWithValue("@troops", troops);
    updateCommand.Parameters.AddWithValue("@id", playerId);

    try
    {
        updateCommand.ExecuteNonQuery();
        Console.WriteLine("Player updated.");
    }
    catch (SqliteException ex)
    {
        Console.WriteLine($"Could not update player: {ex.Message}");
    }
}

static void DeletePlayer(SqliteConnection connection)
{
    // Delete one player by the primary key.
    Console.Write("Player ID: ");
    if (!int.TryParse(Console.ReadLine(), out int playerId))
    {
        Console.WriteLine("Invalid player ID.");
        return;
    }

    using SqliteCommand command = new(
        "DELETE FROM players WHERE player_id = @id;", connection);
    command.Parameters.AddWithValue("@id", playerId);

    Console.WriteLine(command.ExecuteNonQuery() == 1
        ? "Player deleted."
        : "Player not found.");
}


static int ReadIntOrKeep(string label, int currentValue)
{
    // Return the new number, or keep the old value when Enter is pressed.
    Console.Write($"{label} [{currentValue}]: ");
    string? input = Console.ReadLine();
    return int.TryParse(input, out int value) ? value : currentValue;
}
