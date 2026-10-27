-- 9강: 플레이어 퀘스트 진행 상황 및 할당, 진행 중 퀘스트 조회
USE gametest;
CREATE TABLE IF NOT EXISTS player_quests (
    player_id INT,
    quest_id INT,
    STATUS ENUM('시작', '진행중', '완료') DEFAULT '시작',
    start_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP(),
    completed_at TIMESTAMP NULL,
    PRIMARY KEY (player_id, quest_id),
    FOREIGN KEY (player_id) REFERENCES players(player_id),
    FOREIGN KEY (quest_id) REFERENCES quests(quest_id)
);
-- 재실행 시 기존 진행 상태와 시작 시각을 보존합니다.
INSERT INTO player_quests(player_id, quest_id)
SELECT 4, 1 WHERE NOT EXISTS (SELECT 1 FROM player_quests WHERE player_id = 4 AND quest_id = 1);
INSERT INTO player_quests(player_id, quest_id)
SELECT 5, 2 WHERE NOT EXISTS (SELECT 1 FROM player_quests WHERE player_id = 5 AND quest_id = 2);

SELECT p.username, q.title, pq.STATUS
FROM players p
JOIN player_quests pq ON p.player_id = pq.player_id
JOIN quests q ON pq.quest_id = q.quest_id
WHERE pq.STATUS != '완료'
ORDER BY p.player_id, q.quest_id;
