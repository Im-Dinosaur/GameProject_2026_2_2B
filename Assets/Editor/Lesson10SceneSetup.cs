using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Lesson10SceneSetup
{
    [MenuItem("Tools/Lesson 10/Open DB Connection Scene %#F10")]
    public static void OpenScene()
    {
        const string scenePath = "Assets/Scenes/DBConnectionScene.unity";
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (System.IO.File.Exists(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
            return;
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var manager = new GameObject("GameDataManager");
        manager.AddComponent<GameDataManager>();
        EditorSceneManager.SaveScene(scene, scenePath);
        Selection.activeGameObject = manager;
    }
}
