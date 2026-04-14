using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton; // перетащи кнопку в Inspector

    public bool IsOpen { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
        // Привязываем кнопку здесь — надёжнее чем через Inspector
        acceptButton.onClick.AddListener(CloseDialogue);
    }

    public void OpenDialogue(string text)
    {
        dialogueText.text = text;
        dialoguePanel.SetActive(true);
        IsOpen = true;

        // Замораживаем игру (см. пункт 3)
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        IsOpen = false;

        // Размораживаем игру
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}