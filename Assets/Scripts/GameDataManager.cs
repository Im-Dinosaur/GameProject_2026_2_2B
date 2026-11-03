using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GameDataManager : MonoBehaviour
{
    private string serverUrl = "http://localhost:3000";
    // The lesson's Start sets player_id before login; initialize to avoid a null reference.
    private Player currentPlayer = new Player();

    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public List<Quest> playerQuests = new List<Quest>();

    public delegate void OnQuestUpdateHandler(List<Quest> quests);
    public event OnQuestUpdateHandler OnQuestUpdate;

    private IEnumerator GetQuest()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverUrl}/quests/{currentPlayer.player_id}"))
        {
            www.timeout = 10;
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    playerQuests = JsonConvert.DeserializeObject<List<Quest>>(www.downloadHandler.text)
                        ?? new List<Quest>();
                }
                catch (JsonException)
                {
                    Debug.LogError("퀘스트 응답 형식 오류");
                    yield break;
                }
                foreach (var quest in playerQuests)
                    Debug.Log($"{quest.title} {quest.status}");
                OnQuestUpdate?.Invoke(playerQuests);
            }
            else
                Debug.LogError("퀘스트 조회 실패 : " + www.error);
        }
    }

    public Quest GetQuest(int questId)
    {
        return playerQuests.Find(quest => quest.quest_id == questId);
    }

    private void Start()
    {
        currentPlayer.player_id = 4;
        StartCoroutine(GetQuest());
    }
}
