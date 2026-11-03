-- 10강 공개 예제: 플레이어 4번에게 할당된 퀘스트와 상태 조회
USE gametest;
SELECT q.*, pq.STATUS
FROM player_quests pq
JOIN quests q ON pq.quest_id = q.quest_id
WHERE pq.player_id = 4;
