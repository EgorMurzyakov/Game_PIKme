using System.IO;
using UnityEngine;

public class NPCDialogueTrigger_Starosta : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string introFile  = "npc_starosta_intro.json";
    public string rewardFile = "npc_starosta_reward.json";
    public string doneFile   = "npc_starosta_done.json";

    [Header("Подсказка")]
    public InteractionHint interactionHint; // перетащи объект с InteractionHint сюда

    private string[] introLines;
    private string[] rewardLines;
    private string[] doneLines;

    private bool playerInRange = false;

    void Start()
    {
        introLines  = LoadLines(introFile);
        rewardLines = LoadLines(rewardFile);
        doneLines   = LoadLines(doneFile);
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
        // Прячем подсказку, пока диалог открыт
        if (playerInRange && interactionHint != null)
        {
            if (DialogueManager.Instance.IsOpen) interactionHint.Hide();
            else                                  interactionHint.Show();
        }

        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;
        if (DialogueManager.Instance.IsOpen) return;

        interactionHint?.Hide();

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
        else
        {
            dialogueManager.OpenDialogue(doneLines);
        }
    }
}