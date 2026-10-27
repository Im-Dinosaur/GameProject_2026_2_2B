# 9강 실습 결과

원문: https://pineapple-cupcake-2ad.notion.site/9-DB-4068daa8a4ba80edb8bad967c6057a72

교수님이 작성 중인 페이지를 새로고침하여 아래 SQL 실습까지 적용했습니다.

- `gametest.players`에 `level INT DEFAULT 1` 추가
- 모든 기존 플레이어 레벨을 1 증가: 기존 5명 모두 Lv.2
- `ORDER BY level DESC LIMIT 1`로 최고 레벨 플레이어 조회
- `items` 테이블 생성: `item_id`, `name`, `DESCRIPTION`, `VALUE`
- 검 / 기본 무기 / 10, 방패 / 기본 방어구 / 15, 물약 / 체력을 회복 / 5 삽입
- `inventories` 테이블 생성: `inventory_id`, `player_id`, `item_id`, `quantity`
- `players`와 `items`를 참조하는 외래키 2개 생성
- 플레이어 6번: 검 1개, 4번: 방패 5개, 5번: 물약 2개 삽입
- 플레이어·아이템 JOIN 조회로 3행 및 총 수량 8개 검증

레벨·기본 아이템 실습 SQL은 `SQL/lesson9.sql`에 있으며 이후 실습은 아래 개별 SQL 파일에 있습니다. 재실행해도 레벨 증가와 아이템 삽입이 중복되지 않도록 구성했습니다. 변경 전 DB 백업은 Git에서 제외된 `Lesson7Tools/before-lesson9.sql`입니다.

추가된 인벤토리 SQL은 `SQL/lesson9-inventory.sql`에 있습니다. 변경 전 추가 백업은 `Lesson7Tools/before-lesson9-inventory.sql`입니다. 인벤토리 SQL도 재실행 시 동일 실습 행이 중복되지 않습니다.

페이지는 작성 중이며 새로고침 후 하단 이미지가 정상적으로 열렸습니다. 추가 이미지는 `username`, `name`, `quantity`를 표시한 JOIN 조회 결과입니다. 같은 3개 컬럼의 조회를 `SQL/lesson9-inventory-query.sql`에 저장하고 실행했습니다. 로컬의 기존 플레이어 이름과 교수님 스크린샷의 이름이 달라 이름 자체는 다르지만, 강의에서 지정한 플레이어 ID와 아이템·수량 관계는 유지했습니다.

자동 확인에서 추가된 실습을 적용했습니다: 용검(용 기본 무기, VALUE 20) 생성, 플레이어 5번의 용검 2개 추가, 최고 가치 아이템 조회, quests 테이블 및 reward_item_id 외래키 생성. SQL은 SQL/lesson9-quests.sql, 변경 전 백업은 Lesson7Tools/before-lesson9-quests.sql입니다. 재실행 후에도 아이템 4종, 인벤토리 4행·총 수량 10개를 유지함을 검증했습니다. 추가된 퀘스트 데이터도 적용했습니다: 초보자 퀘스트(경험치 100, 물약 보상)와 용사의 검(경험치 500, 검 보상). SQL/lesson9-quest-data.sql을 반복 실행해도 2행과 총 경험치 600이 유지되며 보상 아이템 외래키 JOIN이 정상임을 검증했습니다. 변경 전 백업은 Lesson7Tools/before-lesson9-quest-data.sql입니다. 현재 Node API 및 Unity 가방 UI 연동 코드는 아직 확인되지 않았습니다. 던전의 메모리 상태와 이 SQL 실습 DB는 강의대로 별개입니다.

## Unity 7강 실제 Play 검증

입장, 슬라임 처치, 상자 열기, 휴식, 고블린 처치, 출구 이동, 귀환 버튼을 직접 클릭했습니다. 최종 HP 84/100, Gold 55, 탐험 골드 0을 확인했습니다. UI 텍스트 잘림과 탐험 전 빈 상태 표시를 수정했습니다. Console의 게임 오류/경고 카운터는 0이며 Unity AI 구독 관련 정보 로그는 게임 코드와 별개입니다.


## 추가 실습: 플레이어 퀘스트 진행 상황

player_quests 테이블(플레이어·퀘스트 복합 기본키, 외래키 2개, 시작/진행중/완료 ENUM, 시작·완료 시각)을 생성했습니다. 플레이어 4번에게 퀘스트 1번, 5번에게 2번을 시작 상태로 할당했습니다. 완료가 아닌 퀘스트의 플레이어 이름·제목·상태 JOIN 조회를 검증했습니다. SQL/lesson9-player-quests.sql을 반복 실행해도 기존 상태와 시각을 보존하고 2행을 유지합니다. 변경 전 백업: Lesson7Tools/before-lesson9-player-quests.sql.
