# 10강 진행 기록

원문: https://pineapple-cupcake-2ad.notion.site/10-DB-NODEJS-UNITY-40d8daa8a4ba80b5abf3e7e1f00da933

## 2026-11-03 공개 내용 적용

- Node 폴더에서 mysql2 설치 (3.24.6). 기존 package.json/package-lock.json은 Lesson7Tools/lesson10-backup에 백업했습니다.
- Node/GameServer.js 생성: Express, JSON 미들웨어, mysql2/promise 연결 풀, gametest DB, 포트 3000 시작 코드를 적용했습니다.
- 강의 이미지의 하드코딩 비밀번호 대신 Git에서 제외된 Lesson7Tools/mysql-client.ini를 재사용합니다. DB_HOST, DB_PORT, DB_USER, DB_PASSWORD, DB_NAME 환경변수로 덮어쓸 수 있습니다.
- 실제 MySQL 연결 및 SELECT 1, 기존 5개 테이블 읽기, 임시 포트의 Express 시작을 검증했습니다.
- 기존 데이터: players 5행, items 4행, inventories 4행, quests 2행, player_quests 2행. 검증에서는 쓰기 쿼리를 실행하지 않았습니다.

## 현재 범위

현재 공개된 예제는 연결 풀과 서버 시작 코드까지입니다. 아직 DB 조회 API나 Unity 클라이언트 수정은 게시되지 않아 추가하지 않았습니다. GameServer.js의 루트 경로는 등록된 API가 없으므로 404 응답이 정상입니다.

Node 폴더에서 `node GameServer.js`로 시작합니다. 기존 DungeonServer.js와 기본 포트가 같으므로 동시에 시작하지 않습니다. 연결 풀은 처음 조회할 때 실제 연결을 만듭니다. 이번 검증에서는 읽기 쿼리로 실제 연결까지 확인했습니다. 기존 던전 파일 및 실행 스크립트는 그대로 보존했습니다. 자동 커밋·푸시는 하지 않았습니다.


## 추가 확인: 플레이어별 인벤토리 조회

SQL/lesson10-inventory-query.sql에 원문 SELECT inv.item_id, inv.quantity와 player_id=6 조건을 저장했습니다. 읽기 전용 실행 결과 item_id=1, quantity=1을 확인했습니다. 로그인 API 이미지도 추가됐으나 성공 분기에 HTTP 응답 코드가 없어 요청이 끝나지 않는 작성 중 상태입니다. 성공 응답 형식은 추측해 추가하지 않고 후속 공개 내용을 기다립니다. 기존 GameServer.js와 DB 데이터는 변경하지 않았습니다.


## 추가 확인: 퀘스트 조회 SQL 및 API 작성 중 상태

SQL/lesson10-quest-query.sql에 플레이어 4번의 q.*와 pq.STATUS 조회를 저장했습니다. 읽기 전용 실행으로 초보자 퀘스트·보상 경험치 100·보상 아이템 3·시작 상태를 확인했습니다. /inventory/:playerId 및 /quests/:playerId 예제도 공개됐으나 둘 다 성공 응답이 없는 작성 중 코드입니다. 로그인 API도 동일 상태이므로 서버에는 아직 등록하지 않았습니다. 후속 응답 형식을 기다립니다.


## Unity Newtonsoft.Json 준비

10강에 com.unity.nuget.newtonsoft-json 설치 안내가 추가됐습니다. 기존 잠금 파일과 PackageCache에서 이미 3.2.2가 간접 의존성으로 설치되어 있음을 확인하고, 동일 버전을 manifest의 직접 의존성으로 등록했습니다. manifest와 lock 원본은 Lesson7Tools/lesson10-backup에 보관했습니다. GameDataManager는 현재 제목만 있고 코드가 없어 생성하지 않았습니다. 설치 완료 이미지는 Notion 429로 읽히지 않아 버전을 추측하지 않고 기존 설치 버전을 유지했습니다.


## 추가 확인: Unity 데이터 타입

Player(player_id, username, level), InventoryItem(item_id, name, description, value, quantity), Quest(quest_id, title, description, reward_exp, reward_item_id, status) 타입 원문이 공개되어 Assets/Scripts/GameDataModels.cs로 분리 적용했습니다. 기존 클래스와 이름 충돌이 없음을 확인했습니다. GameDataManager의 실제 통신·필드 코드는 아직 공개되지 않아 생성하지 않았습니다.


## 추가 확인: GameDataManager 클래스 선언

원문에 추가된 GameDataManager MonoBehaviour의 serverUrl/currentPlayer, inventoryItems/playerQuests 리스트, OnQuestUpdateHandler 델리게이트와 OnQuestUpdate 이벤트를 Assets/Scripts/GameDataManager.cs에 적용했습니다. 통신 메서드·이벤트 호출·Scene 연결은 아직 공개되지 않은 상태라 추가하지 않았습니다. 선언만 있는 단계이므로 미사용 멤버 경고는 예상되는 상태입니다.


## 추가 적용: 퀘스트 조회 코루틴

GameDataManager의 GetQuest() 코루틴, GetQuest(int) 검색, Start에서 플레이어 4번 지정 후 조회하는 원문을 적용했습니다. currentPlayer가 null인 상태로 ID를 지정하는 원문 오류는 new Player() 초기화로 보완했습니다. 오류 응답·잘못된 JSON 처리와 10초 제한을 추가했습니다.

서버 이미지에는 아직 성공 응답이 없으나 새 Unity 코드가 List<Quest>를 직접 역직렬화하므로, 이 소비 코드로 확인된 배열 계약에 맞춰 GET /quests/:playerId에 res.json(quests)를 보완했습니다. 로그인·인벤토리 API는 성공 응답 계약이 확인되지 않아 아직 추가하지 않았습니다.

검증: C# 컴파일 경고 0·오류 0, 실제 DB 기반 API에서 플레이어 4번의 퀘스트 1개·시작 상태·경험치 100, 미할당 ID의 빈 배열, 잘못된 ID의 400 응답 통과. 데이터 행 수는 그대로입니다. 아직 Scene에 GameDataManager를 붙이는 원문 안내가 없어 Scene 연결 및 Unity Play 통합 검증은 수행하지 않았습니다. 기존 두 소스는 Lesson7Tools/lesson10-backup/before-quest-coroutine에 백업했습니다.


## 원문 서버 오류 수정 및 실행 확인

교수님이 quests 변수명과 res.json(quests)를 추가한 오류 수정 이미지를 공개했습니다. 이미 적용한 배열 응답과 일치함을 확인했습니다. 새 Node.js 서버 실행 안내에 따라 GameServer.js를 기본 3000 포트에서 실행하고 /quests/4가 퀘스트 1번을 반환함을 확인했습니다. 서버 로그는 Git에서 제외된 Lesson7Tools/game-server.*.log입니다. 로그인 및 인벤토리 성공 응답은 여전히 공개되지 않았습니다.


## Unity 실행 예시 확인 및 검증 준비

원문에 GameDataManager를 GameObject에 부착하고 Inspector의 Player Quests 및 Console에서 조회 결과를 확인한 화면과 Unity→Node.js→MySQL 흐름도가 추가됐습니다. 기존 DungeonScene을 변경하지 않기 위해 별도 DBConnectionScene을 만드는 Tools/Lesson 10/Open DB Connection Scene 메뉴를 Assets/Editor/Lesson10SceneSetup.cs에 추가했습니다. 프로젝트를 열고 스크립트 import까지 확인했으며, 사용자가 데스크톱에서 작업 중인 상태여서 Scene 생성·Play 검증은 아직 완료하지 않았습니다. 이 메뉴를 통해 기존 Scene 수정 여부를 확인한 뒤 별도 Scene에서 테스트할 예정입니다.
