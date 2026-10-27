-- 9강 추가 실습: 새 아이템, 인벤토리 추가, 최고 가치 조회, 퀘스트 테이블
USE gametest;
START TRANSACTION;
INSERT INTO items (`name`, DESCRIPTION, VALUE)
SELECT '용검', '용 기본 무기', 20
WHERE NOT EXISTS (SELECT 1 FROM items WHERE `name` = '용검' AND DESCRIPTION = '용 기본 무기' AND VALUE = 20);
SET @lesson9_dragon_sword = (
    SELECT item_id FROM items WHERE `name` = '용검' AND DESCRIPTION = '용 기본 무기' AND VALUE = 20 ORDER BY item_id LIMIT 1
);
-- 강의의 (5, 4, 2). 자동 증가 ID를 조회해 재실행 시에도 같은 아이템을 참조합니다.
INSERT INTO inventories(player_id, item_id, quantity)
SELECT 5, @lesson9_dragon_sword, 2
WHERE NOT EXISTS (
    SELECT 1 FROM inventories WHERE player_id = 5 AND item_id = @lesson9_dragon_sword AND quantity = 2
);
COMMIT;

SELECT `name`, VALUE FROM items ORDER BY VALUE DESC, item_id ASC LIMIT 1;

CREATE TABLE IF NOT EXISTS quests (
    quest_id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    DESCRIPTION TEXT,
    reward_exp INT DEFAULT 0,
    reward_item_id INT,
    FOREIGN KEY (reward_item_id) REFERENCES items(item_id)
);

SELECT p.username, i.name, inv.quantity
FROM inventories inv
JOIN players p ON p.player_id = inv.player_id
JOIN items i ON i.item_id = inv.item_id
ORDER BY inv.inventory_id;
