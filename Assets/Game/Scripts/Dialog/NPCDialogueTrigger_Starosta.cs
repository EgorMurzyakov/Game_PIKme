using System.IO;
using UnityEngine;

public class NPCDialogueTrigger_Starosta : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string introFile    = "npc_starosta_intro.json";
    public string rewardFile   = "npc_starosta_reward.json";
    public string doneFile     = "npc_starosta_done.json";
    public string waitFile     = "npc_starosta_wait_fabian.json"; // диалог если Fabian не завершён

    [Header("Квест Фабиана")]
    public string fabianQuestFile = "quest_fabian.json"; // путь в StreamingAssets

    [Header("Подсказка")]
    public InteractionHint interactionHint;

    private string[] introLines;
    private string[] rewardLines;
    private string[] doneLines;
    private string[] waitLines;

    private bool playerInRange = false;

    // ── Данные quest_fabian.json ──────────────────────────────────────────────
    [System.Serializable]
    private class FabianQuestData
    {
        public string questStatus   = "";
        public bool   questCompleted = false;
        public bool   rewardGiven   = false;
        public int    killCount     = 0;
        public int    totalEnemies  = 1;
    }

    void Start()
    {
        introLines  = LoadLines(introFile);
        rewardLines = LoadLines(rewardFile);
        doneLines   = LoadLines(doneFile);
        waitLines   = LoadLines(waitFile);
    }

    // ── Проверка quest_fabian ─────────────────────────────────────────────────
    private bool IsFabianQuestCompleted()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fabianQuestFile);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[Starosta] Файл квеста не найден: {path}");
            return false;
        }

        try
        {
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            FabianQuestData data = JsonUtility.FromJson<FabianQuestData>(json);

            bool completed = data != null
                             && data.questStatus   == "completed"
                             && data.questCompleted == true;

            Debug.Log($"[Starosta] quest_fabian → status={data?.questStatus}, completed={data?.questCompleted} | результат: {completed}");
            return completed;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Starosta] Ошибка чтения quest_fabian.json: {ex.Message}");
            return false;
        }
    }

    // ── Загрузка диалоговых линий ─────────────────────────────────────────────
    string[] LoadLines(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Файл не найден: " + path);
            return new string[] { "Ты ещё не готов, малец" };
        }
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return JsonUtility.FromJson<DialogueDataMulti>(json).lines;
    }

    // ── Триггеры ──────────────────────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        interactionHint?.Show();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        interactionHint?.Hide();
    }

    // ── Основная логика ───────────────────────────────────────────────────────
    void Update()
    {
        if (playerInRange && interactionHint != null)
        {
            if (DialogueManager.Instance.IsOpen) interactionHint.Hide();
            else                                 interactionHint.Show();
        }

        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;
        if (DialogueManager.Instance.IsOpen) return;

        interactionHint?.Hide();

        // ── Проверка пререквизита ─────────────────────────────────────────────
        if (!IsFabianQuestCompleted())
        {
            Debug.Log("[Starosta] Квест Фабиана не завершён — показываю wait-диалог.");
            dialogueManager.OpenDialogue(waitLines);
            return;
        }

        // ── Нормальная логика старосты ────────────────────────────────────────
        string status = QuestManager_Starosta.Instance.Data.questStatus;

        if (status == "completed" && !QuestManager_Starosta.Instance.Data.rewardGiven)
        {
            QuestManager_Starosta.Instance.Data.rewardGiven = true;
            dialogueManager.OpenDialogue(rewardLines, isReward: true);
        }
        else if (status == "inactive")
        {
            dialogueManager.OpenDialogue(introLines);
        }
        else if (status == "completed")
        {
            dialogueManager.OpenDialogue(doneLines);
        }
        else
        {
            dialogueManager.OpenDialogue(introLines);
        }
    }
}