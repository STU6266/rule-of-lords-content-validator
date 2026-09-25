PRAGMA foreign_keys = ON;

-- Stores basic information about each saved game.
CREATE TABLE IF NOT EXISTS games (
    game_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Stores the available Rule of Lords races.
CREATE TABLE IF NOT EXISTS races (
    race_id INTEGER PRIMARY KEY,
    name TEXT NOT NULL UNIQUE
);

-- Stores players and their current basic game state.
CREATE TABLE IF NOT EXISTS players (
    player_id INTEGER PRIMARY KEY,
    game_id INTEGER NOT NULL,
    race_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    gold INTEGER NOT NULL DEFAULT 0 CHECK (gold >= 0),
    troops INTEGER NOT NULL DEFAULT 3 CHECK (troops >= 0),


    FOREIGN KEY (game_id) REFERENCES games(game_id),
    FOREIGN KEY (race_id) REFERENCES races(race_id)
);

-- Ensures that each race can only be used once per game.
CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_race_per_game
ON players(game_id, race_id);
