using System.IO;
using UnityEngine;
using TMPro;

public class QuestManager_Starosta : MonoBehaviour
{
    public static QuestManager_Starosta Instance;

    [Header("Настройки")]

    [Header("UI")]
    public TextMeshProUGUI questText;

    public QuestData Data { get; private set; }
    private string questSavePath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        questSavePath = Path.Combine(Application.streamingAssetsPath, "quest_Starosta.json");

        Data = new QuestData
        {
            questStatus   = "inactive",
            questCompleted = false,
            rewardGiven    = false,
            killCount      = 0
        };

        LoadQuestState();
    }

    void Start()
    {
        if (questText != null)
            questText.gameObject.SetActive(false);
    }

    private void LoadQuestState()
    {
        if (!File.Exists(questSavePath))
            return;

        try
        {
            string json = File.ReadAllText(questSavePath);
            QuestData loaded = JsonUtility.FromJson<QuestData>(json);
            if (loaded != null)
            {
                Data = loaded;
                if (Data.questStatus == "active")
                    Data.questStatus = "inactive";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("QuestManager_Starosta: не удалось загрузить состояние квеста: " + e.Message);
        }
    }

    public void SaveQuestState()
    {
        try
        {
            File.WriteAllText(questSavePath, JsonUtility.ToJson(Data, true));
        }
        catch (System.Exception e)
        {
            Debug.LogError("QuestManager_Starosta: не удалось сохранить состояние квеста: " + e.Message);
        }
    }

    public void CompleteQuestWithBlackRose()
    {
        if (Data.questCompleted)
            return;

        Data.questStatus = "completed";
        Data.questCompleted = true;

        if (questText != null)
        {
            questText.gameObject.SetActive(true);
            questText.text = "Квест выполнен";
        }

        SaveQuestState();
    }

    // Вызывается DialogueManager когда игрок дочитал вводный диалог
    public void StartKillPhase()
    {
        if (Data.questStatus != "inactive") return;

        Data.questStatus  = "kill_phase";
        Data.killCount    = 0;

        SaveQuestState();

        if (questText != null)
        {
            questText.gameObject.SetActive(true);
        }

        Debug.Log("Квест начат. Убей врагов: ");
    }

    // Вызывается EnemyCounter когда все враги мертвы
    public void OnAllEnemiesKilled()
    {
        if (Data.questStatus != "kill_phase") return;

        Data.questStatus   = "completed";
        Data.questCompleted = true;
        Data.killCount      = Data.totalEnemies;
        SaveQuestState();

        if (questText != null)
            questText.text = "Вернись к Фабиану";

        Debug.Log("Все враги убиты. Возвращайся к NPC.");
    }




}