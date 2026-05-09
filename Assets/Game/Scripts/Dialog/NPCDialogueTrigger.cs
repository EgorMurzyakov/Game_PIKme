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
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;
        if (DialogueManager.Instance.IsOpen) return;

        string status = QuestManager.Instance.Data.questStatus;
        bool hasBlackRose = inventoryManager != null && inventoryManager.HasBlackRose();

        // Если есть роза и квест ещё не выполнен — завершаем
        if (status != "completed" && hasBlackRose)
        {
            QuestManager.Instance.CompleteQuestWithBlackRose();
            status = QuestManager.Instance.Data.questStatus;
            Debug.Log("Квест завершён с чёрной розой");
        }

        // Выдаём награду
        if (status == "completed" && !QuestManager.Instance.Data.rewardGiven)
        {
            bool rewardSuccess = false;

            if (inventoryManager != null)
                rewardSuccess = inventoryManager.GiveItemByID("TornadoBook", 1);

            if (!rewardSuccess && rewardSpawnPrefab != null)
            {
                Instantiate(rewardSpawnPrefab, transform.position + rewardSpawnOffset, Quaternion.identity);
                rewardSuccess = true;
                Debug.Log("TornadoBook не в инвентаре — заспавнен префаб.");
            }

            QuestManager.Instance.Data.rewardGiven = true;
            QuestManager.Instance.SaveQuestState();

            if (!rewardSuccess)
                Debug.LogWarning("Не удалось выдать TornadoBook и нет префаба.");
            else
                Debug.Log("TornadoBook выдана.");

            dialogueManager.OpenDialogue(rewardLines, isReward: true);
        }
        else if (status == "inactive")
        {
            dialogueManager.OpenDialogue(introLines);
        }
        else
        {
            dialogueManager.OpenDialogue(doneLines);
        }
    }
}

[System.Serializable]
public class DialogueDataMulti
{
    public string[] lines;
}