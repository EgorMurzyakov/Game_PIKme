using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance;

    [Header("Перетащи сюда всех врагов руками")]
    public EnemyHP[] enemies;

    [Header("UI (необязательно)")]
    public TextMeshProUGUI questText;

    private int killsRequired;
    private int killsCurrent;
    private bool active = false;

    void Awake() { Instance = this; }

    public void StartCounting(int required)
    {
        killsRequired = required;
        killsCurrent  = 0;
        active        = true;

        if (questText != null)
        {
            questText.gameObject.SetActive(true);
            UpdateUI();
        }

        Debug.Log("KillCounter запущен, цель: " + required);
    }

    public void ReportKill()
    {
        if (!active) return;

        killsCurrent++;
        Debug.Log($"Убито: {killsCurrent}/{killsRequired}");
        UpdateUI();

        if (killsCurrent >= killsRequired)
        {
            active = false;
            QuestManager.Instance.OnAllEnemiesKilled();
        }
    }

    void UpdateUI()
    {
        if (questText != null)
            questText.text = $"Убито: {killsCurrent} / {killsRequired}";
    }
}