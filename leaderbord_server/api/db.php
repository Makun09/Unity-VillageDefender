<?php
// Connexion à la base SQLite (créée automatiquement au premier lancement)

$dbPath = __DIR__ . '/../data/leaderboard.db';

try {
    $pdo = new PDO('sqlite:' . $dbPath);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    http_response_code(500);
    die(json_encode(['error' => 'Connexion à la base de données impossible']));
}

$pdo->exec("
    CREATE TABLE IF NOT EXISTS scores (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        player_name TEXT NOT NULL,
        time_seconds REAL NOT NULL,
        created_at TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))
    )
");
$pdo->exec("CREATE INDEX IF NOT EXISTS idx_scores_time ON scores (time_seconds DESC)");
$pdo->exec("CREATE INDEX IF NOT EXISTS idx_scores_created_at ON scores (created_at)");