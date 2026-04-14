using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;     
    public TextMeshProUGUI buttonLabel; 

    private string[] currentLines;
    private int currentIndex = 0;
    public bool IsOpen { get; private set; }

    void Awake() { Instance = this; }

    void Start()
    {
        dialoguePanel.SetActive(false);
        continueButton.onClick.AddListener(OnContinuePressed);
    }

    public void OpenDialogue(string[] lines)
    {
        currentLines = lines;
        currentIndex = 0;
        IsOpen = true;

        dialoguePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        dialogueText.text = currentLines[currentIndex];

        // Меняем текст кнопки на последнем блоке
        if (currentIndex == currentLines.Length - 1)
            buttonLabel.text = "Принять";
        else
            buttonLabel.text = "Продолжить";
    }

    void OnContinuePressed()
    {
        // Если есть следующий блок — показываем его
        if (currentIndex < currentLines.Length - 1)
        {
            currentIndex++;
            ShowCurrentLine();
        }
        else
        {
            // Последний блок — закрываем диалог
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        IsOpen = false;
        currentIndex = 0;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}