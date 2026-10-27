-- 9강 새로 추가된 인벤토리 실습
USE gametest;
CREATE TABLE IF NOT EXISTS inventories (
    inventory_id INT AUTO_INCREMENT PRIMARY KEY,
    player_id INT,
    item_id INT,
    quantity INT DEFAULT 1,
    FOREIGN KEY (player_id) REFERENCES players(player_id),
    FOREIGN KEY (item_id) REFERENCES items(item_id)
);

-- 강의 스크린샷의 데이터. 재실행해도 동일한 행은 중복되지 않습니다.
INSERT INTO inventories(player_id, item_id, quantity)
SELECT 6, 1, 1 WHERE NOT EXISTS (
    SELECT 1 FROM inventories WHERE player_id = 6 AND item_id = 1 AND quantity = 1
);
INSERT INTO inventories(player_id, item_id, quantity)
SELECT 4, 2, 5 WHERE NOT EXISTS (
    SELECT 1 FROM inventories WHERE player_id = 4 AND item_id = 2 AND quantity = 5
);
INSERT INTO inventories(player_id, item_id, quantity)
SELECT 5, 3, 2 WHERE NOT EXISTS (
    SELECT 1 FROM inventories WHERE player_id = 5 AND item_id = 3 AND quantity = 2
);

SELECT inv.inventory_id, p.player_id, p.username, i.item_id, i.name, inv.quantity
FROM inventories inv
JOIN players p ON p.player_id = inv.player_id
JOIN items i ON i.item_id = inv.item_id
ORDER BY inv.inventory_id;
