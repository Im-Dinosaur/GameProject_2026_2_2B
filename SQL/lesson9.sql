-- 9강: 현재 공개된 페이지의 SQL 실습 전체.
-- 같은 파일을 다시 실행해도 전체 플레이어 레벨이 재차 증가하지 않도록 처리합니다.
USE gametest;
SET @lesson9_new_level = (
    SELECT COUNT(*) = 0 FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = 'gametest' AND TABLE_NAME = 'players' AND COLUMN_NAME = 'level'
);
SET @lesson9_ddl = IF(@lesson9_new_level,
    'ALTER TABLE players ADD COLUMN `level` INT DEFAULT 1', 'SELECT ''level column already exists'' AS status');
PREPARE lesson9_stmt FROM @lesson9_ddl;
EXECUTE lesson9_stmt;
DEALLOCATE PREPARE lesson9_stmt;

-- 최초 적용 시 모든 플레이어 레벨 1 증가.
UPDATE players SET `level` = `level` + 1 WHERE @lesson9_new_level = 1;
SELECT player_id, username, `level` FROM players;

-- 가장 레벨이 높은 플레이어. 동률일 때 player_id 순으로 결정.
SELECT username, `level` FROM players ORDER BY `level` DESC, player_id ASC LIMIT 1;

CREATE TABLE IF NOT EXISTS items (
    item_id INT AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(100) NOT NULL,
    DESCRIPTION TEXT,
    VALUE INT DEFAULT 0
);

-- 강의의 아이템 3종. 재실행 시 같은 아이템을 중복 삽입하지 않습니다.
INSERT INTO items (item_id, `name`, DESCRIPTION, VALUE) VALUES
    (1, '검', '기본 무기', 10),
    (2, '방패', '기본 방어구', 15),
    (3, '물약', '체력을 회복', 5)
ON DUPLICATE KEY UPDATE item_id = items.item_id;
SELECT item_id, `name`, DESCRIPTION, VALUE FROM items ORDER BY item_id;
