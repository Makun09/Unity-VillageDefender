<?php
// Importe fake_scores.csv dans la base SQLite locale.
// Usage : php import_csv.php

require __DIR__ . '/api/db.php';

$csvPath = __DIR__ . '/fake_scores.csv';
if (!file_exists($csvPath)) {
    die("fake_scores.csv introuvable dans " . __DIR__ . "\n");
}

$handle = fopen($csvPath, 'r');
fgetcsv($handle);

$stmt = $pdo->prepare(
    'INSERT INTO scores (player_name, time_seconds, created_at) VALUES (:name, :time, :created_at)'
);

$pdo->beginTransaction();
$count = 0;
while (($row = fgetcsv($handle)) !== false) {
    [$playerName, $timeSeconds, $createdAt] = $row;
    $stmt->execute([
        'name' => $playerName,
        'time' => (float)$timeSeconds,
        'created_at' => $createdAt,
    ]);
    $count++;
}
$pdo->commit();
fclose($handle);

echo "$count scores importés dans data/leaderboard.db\n";