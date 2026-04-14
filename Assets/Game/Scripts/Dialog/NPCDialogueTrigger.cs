using System.IO;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string fileName = "npc_greeter.json";

    private string[] loadedLines;
    private bool playerInRange = false;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            DialogueData data = JsonUtility.FromJson<DialogueData>(json);
            loadedLines = data.lines;
        }
        else
        {
            Debug.LogError("Файл не найден: " + path);
            loadedLines = new string[] { "Ошибка загрузки диалога." };
        }
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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            if (!DialogueManager.Instance.IsOpen)
                dialogueManager.OpenDialogue(loadedLines);
    }
}