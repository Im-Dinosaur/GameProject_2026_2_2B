using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Lesson10PlayVerification
{
    private const string ActiveKey = "Lesson10PlayVerification.Active";
    private const string DeadlineKey = "Lesson10PlayVerification.Deadline";

    static Lesson10PlayVerification()
    {
        EditorApplication.update += Check;
    }

    public static void Run()
    {
        Lesson10SceneSetup.OpenScene();
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetString(DeadlineKey, DateTime.UtcNow.AddSeconds(90).ToString("o"));
        EditorApplication.isPlaying = true;
    }

    private static void Check()
    {
        if (!SessionState.GetBool(ActiveKey, false)) return;
        if (DateTime.UtcNow > DateTime.Parse(SessionState.GetString(DeadlineKey, "")))
        {
            SessionState.SetBool(ActiveKey, false);
            Debug.LogError("LESSON10_PLAY_VERIFY_TIMEOUT");
            EditorApplication.Exit(1);
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        var manager = UnityEngine.Object.FindFirstObjectByType<GameDataManager>();
        if (manager == null || manager.playerQuests.Count == 0) return;
        var quest = manager.GetQuest(1);
        if (quest == null || quest.status != "시작" || quest.reward_exp != 100) return;
        SessionState.SetBool(ActiveKey, false);
        Debug.Log("LESSON10_PLAY_VERIFY_PASS: actual Unity coroutine loaded quest 1 from Node/MySQL; status=시작; exp=100");
        EditorApplication.Exit(0);
    }
}
