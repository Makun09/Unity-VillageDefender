<?php

$dbHost = getenv('LEADERBOARD_DB_HOST') ?: '127.0.0.1';
$dbPort = getenv('LEADERBOARD_DB_PORT') ?: '3306';
$dbName = getenv('LEADERBOARD_DB_NAME') ?: 'leaderboard_db';
$dbUser = getenv('LEADERBOARD_DB_USER') ?: 'root';
$dbPass = getenv('LEADERBOARD_DB_PASS') ?: '';

try {
    $serverPdo = new PDO(
        "mysql:host={$dbHost};port={$dbPort};charset=utf8mb4",
        $dbUser,
        $dbPass,
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
    );
    $serverPdo->exec("CREATE DATABASE IF NOT EXISTS `{$dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci");

    $pdo = new PDO(
        "mysql:host={$dbHost};port={$dbPort};dbname={$dbName};charset=utf8mb4",
        $dbUser,
        $dbPass,
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
    );
} catch (PDOException $e) {
    http_response_code(500);
    die(json_encode([
        'error' => 'Connexion a la base de donnees impossible',
        'details' => $e->getMessage(),
    ]));
}

$pdo->exec("
    CREATE TABLE IF NOT EXISTS scores (
        id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
        player_name VARCHAR(20) NOT NULL,
        time_seconds FLOAT NOT NULL,
        created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
        INDEX idx_scores_time (time_seconds DESC),
        INDEX idx_scores_created_at (created_at)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
");