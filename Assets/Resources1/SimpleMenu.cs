using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class SimpleMenu : MonoBehaviour
{
    public static SimpleMenu Instance { get; private set; }

    [Header("Ссылки")]
    public GameObject menuCanvasObject;  // Canvas — НЕ трогаем
    public GameObject menuPanel;         // ← добавь это поле (MenuPanel)
    public GameObject settingsPanel;
    public Button btnContinue;
    public Button btnSave;
    public Button btnSettings;
    public Button btnQuit;

    [Header("Инвентарь (для блокировки)")]
    public GameObject inventoryUI;

    private bool isPaused = false;
    private bool wasInventoryActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        if (menuCanvasObject != null && menuCanvasObject.GetComponent<GraphicRaycaster>() == null)
            menuCanvasObject.AddComponent<GraphicRaycaster>();

        if (menuCanvasObject != null) menuCanvasObject.SetActive(false);
        if (settingsPanel != null)    settingsPanel.SetActive(false);   // скрыть настройки
        // menuPanel не трогаем, он внутри Canvas

        // Сначала очищаем все listeners чтобы не накапливались
        if (btnContinue) btnContinue.onClick.RemoveAllListeners();
        if (btnSave)     btnSave.onClick.RemoveAllListeners();
        if (btnSettings) btnSettings.onClick.RemoveAllListeners();
        if (btnQuit)     btnQuit.onClick.RemoveAllListeners();

        if (btnContinue) btnContinue.onClick.AddListener(Resume);
        if (btnSave)     btnSave.onClick.AddListener(SaveDummy);
        if (btnSettings) btnSettings.onClick.AddListener(OpenSettings);
        if (btnQuit)     btnQuit.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
                ShowPauseMenu();            // Esc из настроек → обратно в меню паузы
            else if (isPaused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (inventoryUI != null)
        {
            wasInventoryActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(false);
        }

        if (menuCanvasObject != null) menuCanvasObject.SetActive(true); // Canvas всегда включён
        if (menuPanel != null) menuPanel.SetActive(true);               // показываем меню
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (menuCanvasObject != null) menuCanvasObject.SetActive(false);
        if (menuPanel != null)        menuPanel.SetActive(false);
        if (settingsPanel != null)    settingsPanel.SetActive(false);

        if (inventoryUI != null) inventoryUI.SetActive(wasInventoryActive);
        Cursor.lockState = CursorLockMode.Locked; // Фиксируем курсор в центре экрана
        Cursor.visible = false; // Делаем курсор невидимым
    }

    // Открыть панель настроек (скрыть меню паузы)
    public void OpenSettings()
    {
        if (menuPanel != null)     menuPanel.SetActive(false);      // скрыть меню
        if (settingsPanel != null) settingsPanel.SetActive(false);  // сброс
        if (settingsPanel != null) settingsPanel.SetActive(true);   // OnEnable сработает
    }

    // Вернуться из настроек в меню паузы
    public void ShowPauseMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (menuPanel != null)     menuPanel.SetActive(true);
    }

    public void SaveDummy() { Debug.Log("Сохранено (пустышка)"); }

    public void QuitGame()
    {
        try {
            foreach (var p in Process.GetProcessesByName("spell_recognizer")) p.Kill();
        } catch (System.Exception e) {
            Debug.LogError($"Ошибка при закрытии процесса: {e.Message}");
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}