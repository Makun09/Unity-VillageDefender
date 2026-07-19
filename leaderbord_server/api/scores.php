<?php
header('Content-Type: application/json; charset=utf-8');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type');

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(204);
    exit;
}

require __DIR__ . '/db.php';

function getMondayOfCurrentWeekISO(): string
{
    $now = new DateTime();
    $dayOfWeek = (int)$now->format('N');
    $monday = (clone $now)->modify('-' . ($dayOfWeek - 1) . ' days')->setTime(0, 0, 0);
    return $monday->format('Y-m-d\TH:i:s.000\Z');
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $body = json_decode(file_get_contents('php://input'), true);

    $timeSeconds = $body['time_seconds'] ?? null;
    $playerName = $body['player_name'] ?? 'Joueur';

    if (!is_numeric($timeSeconds) || $timeSeconds < 0) {
        http_response_code(400);
        echo json_encode(['error' => 'time_seconds invalide']);
        exit;
    }

    $safeName = trim((string)$playerName);
    if ($safeName === '') $safeName = 'Joueur';
    if (mb_strlen($safeName) > 20) $safeName = mb_substr($safeName, 0, 20);

    $stmt = $pdo->prepare('INSERT INTO scores (player_name, time_seconds) VALUES (:name, :time)');
    $stmt->execute(['name' => $safeName, 'time' => (float)$timeSeconds]);

    http_response_code(201);
    echo json_encode([
        'id' => $pdo->lastInsertId(),
        'player_name' => $safeName,
        'time_seconds' => (float)$timeSeconds,
    ]);
    exit;
}

if ($_SERVER['REQUEST_METHOD'] === 'GET') {
    $limit = isset($_GET['limit']) ? min((int)$_GET['limit'], 500) : 500;
    $monday = getMondayOfCurrentWeekISO();

    $stmt = $pdo->prepare('
        SELECT player_name, time_seconds, created_at
        FROM scores
        WHERE created_at >= :monday
        ORDER BY time_seconds DESC
        LIMIT :limit
    ');
    $stmt->bindValue(':monday', $monday, PDO::PARAM_STR);
    $stmt->bindValue(':limit', $limit, PDO::PARAM_INT);
    $stmt->execute();

    echo json_encode($stmt->fetchAll(PDO::FETCH_ASSOC));
    exit;
}

http_response_code(405);
echo json_encode(['error' => 'Méthode non supportée']);