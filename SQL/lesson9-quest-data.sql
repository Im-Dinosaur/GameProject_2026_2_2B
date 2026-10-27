-- 9강 추가 실습: 퀘스트 데이터 생성. 반복 실행 시 중복 삽입 방지.
USE gametest;
START TRANSACTION;
INSERT INTO quests(title, DESCRIPTION, reward_exp, reward_item_id)
SELECT '초보자 퀘스트', '첫번째 퀘스트를 완료 하세요', 100, 3
WHERE NOT EXISTS (
    SELECT 1 FROM quests WHERE title = '초보자 퀘스트'
    AND DESCRIPTION = '첫번째 퀘스트를 완료 하세요' AND reward_exp = 100 AND reward_item_id = 3
);
INSERT INTO quests(title, DESCRIPTION, reward_exp, reward_item_id)
SELECT '용사의 검', '전설의 검을 찾아보세요', 500, 1
WHERE NOT EXISTS (
    SELECT 1 FROM quests WHERE title = '용사의 검'
    AND DESCRIPTION = '전설의 검을 찾아보세요' AND reward_exp = 500 AND reward_item_id = 1
);
COMMIT;
SELECT q.quest_id, q.title, q.DESCRIPTION, q.reward_exp, q.reward_item_id, i.name AS reward_item
FROM quests q JOIN items i ON i.item_id = q.reward_item_id ORDER BY q.quest_id;
