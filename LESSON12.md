# 12강 포톤 퓨전 2강

원문: https://pineapple-cupcake-2ad.notion.site/12-2-41b8daa8a4ba806bb8adf3acf8634e00

2026-11-24 원문 코드 이미지부터 마지막 커밋 예시까지 확인하고 Desktop/Fusion_02.unitypackage를 적용했다.

## 적용 범위

- Assets/CoreClash/Lesson02 패키지 에셋과 원래 GUID를 유지했다. 11강 소스는 변경하지 않았다.
- Lesson02InputData: Attack 버튼, Move와 NetworkButtons.
- Lesson02NetworkManager: Shared 로비, 공개 방 목록, 8인 Shared 방, 로컬 플레이어 생성/닉네임, WASD/왼쪽 마우스, Runner 생성/종료.
- Lesson02Bootstrap/CoreSpawner: Shared Master Client가 한 번 생성하며 코어 5개 배치.
- Lesson02CorePickup: Networked 상태/타이머, 로컬 충돌 수집 RPC, 중복 수집 방지, 5초 리스폰, 표시/Collider 동기화.
- Lesson02Player: 체력 100, 점수/쿨다운/이전 버튼 동기화, 이동/회전, 점수/피해 RPC, 넉백, 0.6초 공격 쿨다운, OverlapSphere, 이펙트 RPC.

## 실행

Unity에서 Assets/CoreClash/Lesson02/Scenes/Lesson02_CoreAndMelee.unity를 연다. 같은 방 이름과 서로 다른 닉네임으로 두 클라이언트를 접속한다. WASD 이동, 왼쪽 마우스 근접 공격. 청록색 코어를 획득하면 CORE가 증가하고 5초 뒤 다시 나타난다. 공격 피해는 25, 초기 HP는 100이다. 기존 11강 Photon 설정을 사용한다.

## 패키지 정리와 백업

교수님 PC의 F:/CodexOutPut 경로에 자동 빌드/재수출하던 InitializeOnLoad 코드를 제거했다. 명시적 빌드 경로는 Builds/Lesson12로 변경했다. 기존 에셋을 재생성하는 CreateContent는 자동 실행하지 않는다.

백업: Lesson7Tools/lesson12-backup-20261124-102406. 원래 unitypackage는 수정하지 않았다. Builds와 Lesson7Tools는 Git 제외 상태다. 자동 커밋/푸시는 하지 않았다.

## 검증

Unity 6000.3.23f1 배치 import, C# 컴파일 및 Fusion IL weaving 통과. 실제 두 클라이언트 빌드 검증 결과는 아래에 기록한다.

Lesson02SmokeTest는 -lesson02Smoke 인자가 있을 때만 실행한다. 원본 테스트에 있던 피해 직접 지급 fallback을 제거하고 실제 충돌 수집, 리스폰, 공격 overlap/RPC, 상대 체력/넉백, 두 클라이언트 이펙트를 검사한다.

### 최종 결과

실제 Windows 개발 빌드 두 개를 Photon Shared 테스트 방에 접속시켜 양쪽 모두 PASS를 확인했다. 플레이어 2명, 코어 정확히 5개, 실제 네트워크 입력으로 이동 후 코어 수집 점수 1, 중복 점수 없이 리스폰, 공격 버튼 입력 후 상대 HP 75, 양쪽 이펙트와 0.45 넉백을 검증했다. 직접 Transform을 옮기는 초기 테스트는 Fusion 틱에서 복원되어 실패했으며, 최종 테스트는 OnInput → FixedUpdateNetwork → 실제 충돌/공격 RPC 경로를 사용한다. 테스트 입력 지원은 UNITY_EDITOR 또는 DEVELOPMENT_BUILD에서만 포함되고 일반 플레이에서는 비활성이다.

로그: Lesson7Tools/lesson12-build.log, lesson12-client1.log, lesson12-client2.log. 배치 모드 검증이므로 화면 렌더링/마우스 조작의 육안 검증은 별도로 수행하지 않았다. 원래 사용자가 변경한 ProjectSettings.asset은 백업과 내용 일치를 확인해 보존했다.
