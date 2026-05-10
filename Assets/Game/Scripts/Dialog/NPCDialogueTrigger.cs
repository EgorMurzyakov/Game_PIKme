using System.IO;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string introFile  = "npc_fabian_intro.json";
    public string rewardFile = "npc_fabian_reward.json";
    public string doneFile   = "npc_fabian_done.json";

    public GameObject rewardSpawnPrefab;
    public Vector3 rewardSpawnOffset = new Vector3(0f, 0.5f, 1f);

    [Header("Подсказка")]
    public InteractionHint interactionHint;

    private string[] introLines;
    private string[] rewardLines;
    private string[] doneLines;

    private InventoryManager inventoryManager;
    private bool playerInRange = false;

    void Start()
    {
        introLines  = LoadLines(introFile);
        rewardLines = LoadLines(rewardFile);
        doneLines   = LoadLines(doneFile);
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    string[] LoadLines(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError("Файл не найден: " + path);
            return new string[] { "..." };
        }
        string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
        return JsonUtility.FromJson<DialogueDataMulti>(json).lines;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        // Показываем только если диалог не открыт
        if (!DialogueManager.Instance.IsOpen)
            interactionHint?.Show();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        interactionHint?.Hide();
    }

    void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;
        if (DialogueManager.Instance.IsOpen) return;

        // Прячем подсказку — диалог вот-вот откроется
        interactionHint?.Hide();

        string status = QuestManager.Instance.Data.questStatus;
        bool hasBlackRose = inventoryManager != null && inventoryManager.HasBlackRose();

        if (status != "completed" && hasBlackRose)
        {
            QuestManager.Instance.CompleteQuestWithBlackRose();
            status = QuestManager.Instance.Data.questStatus;
            inventoryManager?.RemoveItemByID("Rose");
            Debug.Log("Квест завершён с чёрной розой — роза удалена из инвентаря");
        }

        if (status == "completed" && !QuestManager.Instance.Data.rewardGiven)
        {
            bool rewardSuccess = false;

            if (inventoryManager != null)
                rewardSuccess = inventoryManager.GiveItemByID("СЕРЕБРЯННЫЙ МЕЧ", 1);

            if (!rewardSuccess && rewardSpawnPrefab != null)
            {
                Instantiate(rewardSpawnPrefab, transform.position + rewardSpawnOffset, Quaternion.identity);
                rewardSuccess = true;
                Debug.Log("СЕРЕБРЯННЫЙ МЕЧ не в инвентаре — заспавнен префаб.");
            }

            QuestManager.Instance.Data.rewardGiven = true;
            QuestManager.Instance.SaveQuestState();

            if (!rewardSuccess)
                Debug.LogWarning("Не удалось выдать СЕРЕБРЯННЫЙ МЕЧ и нет префаба.");
            else
                Debug.Log("СЕРЕБРЯННЫЙ МЕЧ выдана.");

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

[System.Serializable]
public class DialogueDataMulti
{
    public string[] lines;
}