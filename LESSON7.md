# 7강 실습 완료

## 실행

프로젝트 폴더에서 PowerShell로 실행:

```powershell
.\Start-Lesson7.ps1
```

Unity에서 `Assets/Scenes/DungeonScene.unity`를 열고 Play를 누릅니다.
입장 → 공격 2회 → 다음 방 → 상자 열기 → 다음 방 → 휴식 → 다음 방 → 공격 3회 → 다음 방 → 귀환 순서로 확인할 수 있습니다. 첫 탐험의 귀환 보상은 55 Gold입니다.

SQL 콘솔까지 열려면:

```powershell
.\Start-Lesson7.ps1 -SqlConsole
```

## 구성

- Node.js: `Node/DungeonServer.js`, `http://127.0.0.1:3000`
- MySQL 8.4.13: `Lesson7Tools/mysql-8.4.13-winx64`, `127.0.0.1:3306`
- HeidiSQL: `Lesson7Tools/HeidiSQL/heidisql.exe`
- 데이터베이스: `gametest`, 테이블: `players`
- 실습 SQL: `SQL/lesson7.sql`
- 원본 코드 백업: `Lesson7Backup-20261027`

MySQL은 이 프로젝트 전용 포터블 구성입니다. Windows 서비스나 자동 시작으로 등록하지 않았으므로 재부팅 후 위 실행 스크립트를 사용합니다. 서버들은 로컬 주소에만 바인딩됩니다.

HeidiSQL에서 새 세션을 만들 때 호스트 `127.0.0.1`, 포트 `3306`, 사용자 `root`, 데이터베이스 `gametest`를 사용합니다. 임의로 생성한 로컬 DB 비밀번호는 `Lesson7Tools/mysql-client.ini`의 password 항목에 있습니다. 이 파일을 공유하지 마세요. 도구·DB 데이터·비밀번호·백업 폴더는 로컬 Git 제외 목록에 등록했습니다.

## 검증 결과

- Unity C# 컴파일: 경고 0, 오류 0
- API: 초기 상태, 잘못된 행동 거절, 중복 입장 거절, 전투, 보물상자, 회복, 방 이동, 귀환, 중복 보상 방지, 재입장 통과
- MySQL: 테이블 생성, 단일/다중 삽입, 조회, 로그인 시간 갱신, LIKE 검색, 실습 레코드 삭제 통과 (최종 5행)
- Unity DungeonScene에서 실제 버튼 클릭으로 전투·회복·상자·귀환을 검증했습니다. 최종 HP 84/100, Gold 55입니다. 텍스트 영역 잘림과 JSON null 초기 상태 표시도 수정했습니다.

7강 범위대로 던전 상태는 Node 메모리에 있고, SQL 실습은 별도입니다. 서버를 재시작하면 던전 진행과 골드는 초기화됩니다. `password_hash` 샘플 값은 수업용 자리표시자입니다.
