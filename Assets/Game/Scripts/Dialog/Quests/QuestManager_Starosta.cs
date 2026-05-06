using UnityEngine;

public class QuestManager_Starosta : MonoBehaviour
{
    public static QuestManager_Starosta Instance;

    public QuestData Data { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Data = new QuestData
        {
            questStatus    = "inactive",
            questCompleted = false,
            rewardGiven    = false
        };
    }

    // Вызывается DialogueManager когда игрок дочитал вводный диалог
    public void StartQuest()
    {
        if (Data.questStatus != "inactive") return;

        Data.questStatus = "active";
        Debug.Log("Квест Старосты начат.");
    }

    // Вызывается триггером на другой сцене
    public void CompleteQuest()
    {
        if (Data.questStatus != "active") return;

        Data.questStatus    = "completed";
        Data.questCompleted = true;
        Debug.Log("Квест Старосты выполнен.");
    }
}