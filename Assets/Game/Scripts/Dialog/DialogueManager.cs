using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;
    public Button acceptButton;

    private string[] lines;
    private int currentIndex;
    private bool isRewardDialogue = false;

    public bool IsOpen { get; private set; }
    public bool isStarostaDialogue = false;

    void Awake() { Instance = this; }

    void Start()
    {
        dialoguePanel.SetActive(false);
        continueButton.onClick.AddListener(NextLine);
        acceptButton.onClick.AddListener(CloseDialogue);
    }

    public void OpenDialogue(string[] dialogueLines, bool isReward = false)
    {
        lines             = dialogueLines;
        currentIndex      = 0;
        isRewardDialogue  = isReward;

        dialoguePanel.SetActive(true);
        IsOpen = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        ShowLine();
    }

    void NextLine()
    {
        currentIndex++;
        ShowLine();
    }

    void ShowLine()
    {
        dialogueText.text = lines[currentIndex];
        bool isLast = currentIndex >= lines.Length - 1;

        Debug.Log($"Страница {currentIndex}, последняя: {isLast}, статус квеста: {QuestManager.Instance.Data.questStatus}");

        continueButton.gameObject.SetActive(!isLast);
        acceptButton.gameObject.SetActive(isLast);
        
        if (isLast && !isRewardDialogue)
        {
            if (isStarostaDialogue)
            {
                if (QuestManager_Starosta.Instance.Data.questStatus == "inactive")
                    QuestManager_Starosta.Instance.StartQuest();
            }
            else
            {
                if (QuestManager.Instance.Data.questStatus == "inactive")
                    QuestManager.Instance.StartKillPhase();
            }
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        IsOpen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
}