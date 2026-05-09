using UnityEngine;
using TMPro;

/// <summary>
/// Прикрепи этот скрипт к Canvas-объекту с подсказкой.
/// Ссылку на него передай в каждый NPCDialogueTrigger.
/// </summary>
public class InteractionHint : MonoBehaviour
{
    [Header("UI")]
    public GameObject hintPanel;          // панель с фоном (Image)
    public TextMeshProUGUI hintText;      // текст внутри панели

    [Header("Настройки")]
    public string message = "Нажмите [E] для разговора";

    void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (hintPanel != null) hintPanel.SetActive(true);
        if (hintText  != null) hintText.text = message;
    }

    public void Hide()
    {
        if (hintPanel != null) hintPanel.SetActive(false);
    }
}
