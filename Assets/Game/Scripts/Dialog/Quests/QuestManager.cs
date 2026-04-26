using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Настройки")]
    public int enemiesToKill = 5;

    [Header("UI")]
    public TextMeshProUGUI questText;

    public QuestData Data { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Data = new QuestData
        {
            questStatus   = "inactive",
            questCompleted = false,
            killCount      = 0,
            totalEnemies   = enemiesToKill
        };
    }

    void Start()
    {
        if (questText != null)
            questText.gameObject.SetActive(false);
    }

    // Вызывается DialogueManager когда игрок дочитал вводный диалог
    public void StartKillPhase()
    {
        if (Data.questStatus != "inactive") return;

        Data.questStatus  = "kill_phase";
        Data.killCount    = 0;
        Data.totalEnemies = enemiesToKill;

        if (questText != null)
        {
            questText.gameObject.SetActive(true);
            UpdateUI();
        }

        Debug.Log("Квест начат. Убей врагов: " + enemiesToKill);
    }

    // Вызывается EnemyCounter когда все враги мертвы
    public void OnAllEnemiesKilled()
    {
        if (Data.questStatus != "kill_phase") return;

        Data.questStatus   = "completed";
        Data.questCompleted = true;
        Data.killCount      = Data.totalEnemies;

        if (questText != null)
            questText.text = "Вернись к Фабиану";

        Debug.Log("Все враги убиты. Возвращайся к NPC.");
    }

    // Вызывается EnemyHealth при каждом убийстве (для счётчика UI)
    public void OnEnemyKilled()
    {
        if (Data.questStatus != "kill_phase") return;

        Data.killCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (questText != null)
            questText.text = $"Убито: {Data.killCount} / {Data.totalEnemies}";
    }
}