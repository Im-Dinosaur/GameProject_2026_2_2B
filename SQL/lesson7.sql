-- 7강: MySQL 설치 및 쿼리. 수업용 가상 데이터입니다.
CREATE DATABASE IF NOT EXISTS gametest CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE gametest;
CREATE TABLE IF NOT EXISTS players (
    player_id INT NOT NULL AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL DEFAULT '0',
    email VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP NULL DEFAULT NULL,
    PRIMARY KEY (player_id),
    UNIQUE INDEX username (username),
    UNIQUE INDEX email (email)
) COLLATE=utf8mb4_0900_ai_ci;

-- 단일 플레이어 삽입 (해시는 수업용 자리표시자)
INSERT IGNORE INTO players(username, email, password_hash)
VALUES ('hero123', 'hero123@example.com', 'hashed_password');
SELECT * FROM players LIMIT 1000;

-- 여러 플레이어 삽입
INSERT IGNORE INTO players(username, email, password_hash) VALUES
('hero423', 'hero423@example.com', 'hashed_password3'),
('hero523', 'hero523@example.com', 'hashed_password4'),
('hero623', 'hero623@example.com', 'hashed_password5'),
('hero723', 'hero723@example.com', 'hashed_password6'),
('hero823', 'hero823@example.com', 'hashed_password7');

-- 로그인 시간 조회 및 갱신
SELECT username, last_login FROM players;
UPDATE players SET last_login = CURRENT_TIMESTAMP WHERE username = 'hero123';
SELECT username, last_login FROM players WHERE username = 'hero123';

-- 조건 검색
SELECT username, email FROM players WHERE username LIKE '%hero%';

-- 앞에서 생성한 실습 레코드 삭제
DELETE FROM players WHERE username = 'hero123';
SELECT COUNT(*) AS remaining_practice_players FROM players;
