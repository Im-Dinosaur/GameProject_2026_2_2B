-- 10강 공개 예제: 특정 플레이어가 보유한 아이템 ID와 수량 조회
USE gametest;
SELECT inv.item_id, inv.quantity
FROM inventories inv
JOIN items i ON inv.item_id = i.item_id
WHERE inv.player_id = 6;
