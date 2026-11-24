# 13강 포톤 퓨전 3강

원문: https://pineapple-cupcake-2ad.notion.site/13-3-4228daa8a4ba80148bb4d70d56ed11bc

2026-11-24 작성 중인 원문을 확인해 공개된 실습만 적용했다. 마지막 확인 범위는 MatchState, Bootstrap, HUD 및 '프리팹 만들기 전까지' 커밋 예시 이미지다. Projectile/MatchState 프리팹 제작은 제목만 있으며 상세 설정은 아직 없다. 교수님의 13강 완료 명시는 아직 없다.

## 적용 완료 범위

- Lesson02Button Fire/Reload, 오른쪽 마우스/R 키 네트워크 입력 및 메서드 연결.
- Lesson02Projectile: 속도 13, Networked Owner/Damage/Life, 3초 수명, 이동/수명 종료 Despawn, 발사자 본인 충돌 제외, 피해 및 공격자 전달 후 Despawn.
- 피해 요청/RPC의 PlayerRef attacker와 근접 공격 호출 수정.
- 탄창 크기 6 및 Ammo 초기화, FireCoolDown/ReloadTimer, TryFire의 0.25초 쿨다운·피해 20·탄약 소비·빈 탄창 자동 재장전 시작. 재장전은 2초 후 탄약 6으로 복구.
- Networked kills/IsDead/IsInvincible/RespawnTimer/InvincibleTimer. 사망·무적 중 추가 피해 차단, HP 0에서 Die 호출, 킬 RPC 전달. 사망 중 중복 피해로 킬이 다시 증가하지 않는다.
- 사망 3초 후 HP 100 및 지정 위치로 리스폰, 2초 무적 후 해제. 리스폰 위치 x는 (InputAuthority.RawEncoded % 5 - 2) * 2, y=1, z=0이다.
- Lesson02Player.PlayerDisplayName은 11강 TodoPlayer.PlayerName을 읽고 컴포넌트가 없으면 Player를 반환한다. 11강 소스는 수정하지 않았다.
- Lesson02MatchState: 독립된 NetworkObject용 NetworkBehaviour, Instance, 90초 MatchTimer, IsFinished, winnerName, RemainingSeconds, CanPlay. State Authority가 타이머 종료 시 최고 kills의 단독 승자 이름 또는 DRAW를 기록한다. 파괴 시 Instance를 비운다.
- Lesson02Bootstrap에 matchPrefab 필드와 코어 스포너 생성 직후 공용 경기 객체를 생성하는 경로 추가. 상세 프리팹 연결 안내 전이므로 null이면 경기 객체 생성만 건너뛴다.
- Lesson02HUD에 AMMO, KILLS, 남은 시간(올림), 경기 종료 시 WINNER 표시 경로 추가. 새 행을 담도록 배경 높이를 늘렸다.

## 원문에 대한 보완

투사체 충돌 이미지의 HasInputAuthority 대신 이동/Despawn과 동일한 HasStateAuthority를 검사하며, 유효한 Spawn 상태만 처리한다. 투사체 프리팹이 null이면 탄약 소비/예외 없이 발사를 무시한다.

원문은 ApplyMovement 이후에 사망 분기를 검사하고, 입력 수신 실패 시 타이머 갱신 전에 반환한다. 사망 중 이동을 막고 입력이 없어도 리스폰/재장전이 끝나도록 State Authority 검사 → UpdateTimers → 입력 수신 → 사망 분기 → 이동/공격 순서로 보완했다.

MatchState의 원문 이미지에는 winnerName.Length == 1이 쓰여 있다. 승자 수를 판정해야 하므로 winners.Length == 1로 수정했다. 이 보완이 없으면 초기 빈 winnerName 때문에 단독 승자도 DRAW가 된다. 승자 0명 또는 최고 킬 동률이면 DRAW다.

## 아직 공개되지 않은 연결과 검증 한계

Projectile/MatchState 프리팹 제작 제목 아래에는 구체적인 제작·참조 설정 이미지가 없다. 마지막 커밋 예시도 '프리팹 만들기 전까지'라고 명시한다. 따라서 프리팹을 추측해 생성하거나 씬/플레이어에 연결하지 않았다. projectilePrefab 및 matchPrefab은 미할당이다.

일반 씬의 실제 투사체 발사와 경기 객체 생성은 아직 비활성이다. TIME/WINNER HUD 경로는 경기 객체가 연결된 뒤 표시된다. MatchState의 90초 종료 및 승자·무승부 네트워크 통합 검증은 이 연결 이후 수행해야 한다. CanPlay 속성은 원문대로 제공하지만 경기 종료 후 입력 차단 연동은 아직 원문에 없다. 현재 결과를 실제 발사나 경기 종료 통합 검증 완료로 해석하면 안 된다.

## 검증

Unity 6000.3.23f1의 C# 컴파일, Fusion IL weaving, Windows 개발 빌드를 통과했다. 이전 단계에서 두 Photon Shared 클라이언트의 코어 수집/리스폰/근접 공격/이펙트/넉백 회귀 검사도 양쪽 PASS였다.

이번 빌드의 실제 두 클라이언트에서 다음 항목을 양쪽 PASS로 확인했다.

- 새 PlayerDisplayName이 자신의 접속 닉네임과 일치하고, 두 클라이언트의 서로 다른 닉네임을 양쪽에서 읽는다.
- 피해 RPC로 HP 0 및 IsDead 전환, 킬 1 증가. 사망 중 추가 피해에도 킬은 1 유지.
- 사망 중 이동 입력 차단, 3초 후 HP 100·지정 위치·무적 상태 동기화.
- 무적 중 피해 차단, 2초 후 무적 해제, 이후 피해 20 적용(HP 80).
- Reload 입력 이후 즉시 채워지지 않고 2초 뒤 탄약 6 동기화.

재장전 테스트에서는 투사체 프리팹이 아직 없으므로 소유자의 Ammo를 0으로 지정하는 테스트 픽스처만 사용했다. 이후 입력/타이머/네트워크 복제는 실제 게임 경로로 검증했다. 원격 Transform은 상태 값보다 보간 표시가 늦을 수 있어 검사 시 1초 한도 내에서 위치 반영을 기다린다. 배치 모드 검사이므로 HUD 화면 육안 검사는 별도로 하지 않았다.

테스트 진입: Lesson02SmokeTest의 -lesson13LifecycleSmoke 인자 → Lesson13LifecycleVerification.Run. 일반 실행에서는 테스트가 작동하지 않으며 UNITY_EDITOR 또는 DEVELOPMENT_BUILD에서만 컴파일된다. 기존 -lesson02Smoke 검사는 유지했다. 공유 테스트 방은 실행마다 고유 이름을 쓴다.

최신 로그: Lesson7Tools/lesson13-match-build.log, lesson13-match-client1.log, lesson13-match-client2.log. 결과: names/death/singleKill/deadStopped/respawn/invincible/expired/vulnerable/reloadWaited/reloaded 모두 True, kills=1, hp=80, ammo=6. 이전 검증 로그는 lesson13-lifecycle-build.log 및 lesson13-lifecycle-client1.log, lesson13-lifecycle-client2.log다.

## 보존 및 진행 상태

12강은 e68e49c '[Feat] 캐릭터 및 공격, 수집, 점수 동기화'로 origin/main에 푸시했다. 13강은 사용자의 요청으로 원문 사진의 메시지 '[Feat] 네트워크 공용 코드 & 방 상태 공유 프리팹 만들기 전까지'로 로컬 커밋했다. 원격 푸시는 하지 않았다. 이후 추가 실습은 별도 요청 없이 자동 커밋/푸시하지 않는다. 11강 소스와 사용자의 ProjectSettings.asset 변경은 보존했다. Unity 빌드가 자동 추가한 preload 항목만 적용 전 설정과 대조해 복원했다. 비밀/키는 기록하거나 커밋하지 않았다.

백업: Lesson7Tools/lesson13-backup-20261124-105508, lesson13-fire-backup-20261124-112015, lesson13-lifecycle-backup-20261124-112659, lesson13-match-backup-20261124-031426 및 탄약 준비 단계 백업. 최신 백업의 이름은 UTC 시각이다. 5분 간격 자동 확인을 계속하며 새 공개 원문만 적용한다. 교수님이 13강 완료를 명시하고 공개 작업을 검증하면 자동 확인을 중지한다.
