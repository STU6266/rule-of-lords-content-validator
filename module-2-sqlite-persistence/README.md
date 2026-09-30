# Overview

I created a small Rule of Lords SQLite Persistence Prototype in C#. The program uses a SQLite database to store games, races, and players. It can show players, add new players, update player information, and delete players.

I wanted to build something small that could also be useful for my Rule of Lords project later. My main goal was to get more comfortable with relational databases and learn how to use SQL from a C# program. I also wanted to practice working with tables, primary keys, foreign keys, constraints, and joins.

The program uses a simple console menu so I can work with the database without entering SQL commands directly.

[Software Demo Video]:https://youtu.be/iZ6McI3mdGI

# Relational Database

The database uses three related tables:

games stores basic information about each game.
races stores the available Rule of Lords races.
players stores player information and connects each player to a game and a race.

The players table uses foreign keys to connect to the games and races tables. The database also uses constraints to help keep the data valid. For example, gold and troop values cannot be negative, and the same race cannot be assigned to more than one player in the same game.

The program demonstrates several SQL operations:

INSERT to add a player.
SELECT to retrieve player information.
UPDATE to change player information.
DELETE to remove a player.
JOIN to combine information from the players, games, and races tables.

# Development Environment


I used Visual Studio Code on Ubuntu to write and test the program. I also used the terminal, Git, and GitHub to manage and publish the project.

The program is written in C# and runs with .NET 10. I used SQLite for the relational database and the Microsoft.Data.Sqlite library to connect the C# program to the database.

# Useful Websites

- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Microsoft.Data.Sqlite Documentation](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)

# Future Work

- Add more game state information if the prototype is expanded later.
- Improve input validation for incorrect user entries.
- Add a transaction for operations that change more than one table.
