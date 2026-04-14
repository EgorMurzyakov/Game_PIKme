using System.IO;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public string fileName = "npc_greeter.json"; // пишешь в Inspector

    private string loadedText;
    private bool playerInRange = false;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            DialogueData data = JsonUtility.FromJson<DialogueData>(json);
            loadedText = data.text;
        }
        else
        {
            Debug.LogError("Файл не найден: " + path);
            loadedText = "Ошибка: файл не найден.";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogueManager.Instance.IsOpen)
                dialogueManager.OpenDialogue(loadedText);
        }
    }
}