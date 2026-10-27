-- 9강 추가 이미지: username / name / quantity 3개 컬럼 조회
USE gametest;
SELECT p.username, i.name, inv.quantity
FROM inventories AS inv
JOIN players AS p ON p.player_id = inv.player_id
JOIN items AS i ON i.item_id = inv.item_id
ORDER BY inv.inventory_id;
