const express = require('express');
const mysql = require('mysql2/promise');
const fs = require('node:fs');
const path = require('node:path');

const app = express();
app.use(express.json());

// Reuse the ignored local MySQL configuration; never put a password in source.
const configPath = process.env.MYSQL_CLIENT_CONFIG ||
    path.join(__dirname, '..', 'Lesson7Tools', 'mysql-client.ini');
const localConfig = {};
if (fs.existsSync(configPath)) {
    let inClient = false;
    for (const line of fs.readFileSync(configPath, 'utf8').split(/\r?\n/)) {
        const trimmed = line.trim();
        if (trimmed.startsWith('[')) { inClient = trimmed === '[client]'; continue; }
        if (!inClient || !trimmed || trimmed.startsWith('#') || trimmed.startsWith(';')) continue;
        const separator = trimmed.indexOf('=');
        if (separator > 0) localConfig[trimmed.slice(0, separator).trim()] = trimmed.slice(separator + 1).trim();
    }
}

const pool = mysql.createPool({
    host: process.env.DB_HOST || localConfig.host || '127.0.0.1',
    port: Number(process.env.DB_PORT || localConfig.port || 3306),
    user: process.env.DB_USER || localConfig.user || 'root',
    password: process.env.DB_PASSWORD ?? localConfig.password,
    database: process.env.DB_NAME || 'gametest'
});

// Unity's published GetQuest coroutine deserializes a raw List<Quest>.
app.get('/quests/:playerId', async (req, res) => {
    if (!/^[1-9]\d*$/.test(req.params.playerId)) {
        return res.status(400).json({ success: false, message: '잘못된 플레이어 ID' });
    }
    try {
        const [quests] = await pool.query(
            'SELECT q.*, pq.STATUS FROM player_quests pq JOIN quests q ON pq.quest_id = q.quest_id WHERE pq.player_id = ?',
            [req.params.playerId]
        );
        return res.json(quests);
    } catch {
        return res.status(500).json({ success: false, message: '퀘스트 조회 실패' });
    }
});

const PORT = Number(process.env.PORT || 3000);
if (require.main === module) {
    app.listen(PORT, '127.0.0.1', () => console.log(`서버 실행 중 : ${PORT}`));
}
module.exports = { app, pool };
