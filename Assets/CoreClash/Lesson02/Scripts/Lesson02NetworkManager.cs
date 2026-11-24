using System.Collections.Generic;
using CoreClash.Lesson01.Course;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreClash.Lesson02
{
    public class Lesson02NetworkManager : TodoRunnerCallbacks
    {
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private Lesson02RoomUI roomUI;
        private NetworkRunner runner;
        private string nickname;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal Lesson02InputData? VerificationInput;
#endif

        public async void JoinLobby()
        {
            if (runner != null) await runner.Shutdown();
            runner = CreateRunner();
            StartGameResult result = await runner.JoinSessionLobby(SessionLobby.Shared);
            roomUI.SetStatus(result.Ok ? "로비에 연결되었습니다." : $"로비 실패: {result.ShutdownReason}");
        }

        public override void OnSessionListUpdated(NetworkRunner r, List<SessionInfo> sessions)
        {
            List<string> rooms = new();
            foreach (SessionInfo session in sessions)
                if (session.IsVisible && session.IsOpen)
                    rooms.Add($"{session.Name} ({session.PlayerCount}/{session.MaxPlayers})");
            roomUI.ShowRooms(rooms);
        }

        public async void StartRoom(string roomName, string playerName)
        {
            if (string.IsNullOrWhiteSpace(roomName) || string.IsNullOrWhiteSpace(playerName))
            {
                roomUI.SetStatus("닉네임과 방 이름을 입력하세요");
                return;
            }
            nickname = playerName.Trim();
            roomUI.SetConnecting(true);
            if (runner != null) await runner.Shutdown();
            runner = CreateRunner();
            NetworkSceneManagerDefault sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            StartGameResult result = await runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName.Trim(),
                SceneManager = sceneManager,
                IsOpen = true, IsVisible = true, PlayerCount = 8
            });
            roomUI.SetConnected(result.Ok);
            roomUI.SetStatus(result.Ok ? $"{roomName} 방에 연결되었습니다." : $"연결 실패: {result.ShutdownReason}");
        }

        public override void OnPlayerJoined(NetworkRunner r, PlayerRef player)
        {
            if (player != r.LocalPlayer) return;
            float x = (player.RawEncoded % 5 - 2) * 2f;
            r.Spawn(playerPrefab, new Vector3(x, 1f, 0f), Quaternion.identity, player,
                (_, obj) => obj.GetComponent<TodoPlayer>().SetInitialName(nickname));
        }

        public override void OnInput(NetworkRunner r, NetworkInput input)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (VerificationInput.HasValue) { input.Set(VerificationInput.Value); return; }
#endif
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Vector2 move = Vector2.zero;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed) move.y++;
                if (keyboard.sKey.isPressed) move.y--;
                if (keyboard.aKey.isPressed) move.x--;
                if (keyboard.dKey.isPressed) move.x++;
            }
            NetworkButtons buttons = default;
            buttons.Set((int)Lesson02Button.Attack, mouse != null && mouse.leftButton.isPressed);
            buttons.Set((int)Lesson02Button.Fire, mouse != null && mouse.rightButton.isPressed);
            buttons.Set((int)Lesson02Button.Reload, keyboard != null && keyboard.rKey.isPressed);
            input.Set(new Lesson02InputData { Move = Vector2.ClampMagnitude(move, 1f), Buttons = buttons });

        }

        public override void OnShutdown(NetworkRunner r, ShutdownReason reason)
        {
            if (runner == r) runner = null;
            roomUI.SetConnected(false);
            roomUI.SetStatus($"연결 종료: {reason}");
        }

        private NetworkRunner CreateRunner()
        {
            NetworkRunner created = new GameObject("Lesson02Runner").AddComponent<NetworkRunner>();
            created.ProvideInput = true;
            created.AddCallbacks(this);
            return created;

        }
    }
}

