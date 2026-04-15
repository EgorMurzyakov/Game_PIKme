using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class SimpleMenu : MonoBehaviour
{
    [Header("Ссылки")]
    public GameObject menuCanvasObject;
    public Button btnContinue;
    public Button btnSave;
    public Button btnSettings;
    public Button btnQuit;
    
    [Header("Инвентарь (для блокировки)")]
    public GameObject inventoryUI;

    private bool isPaused = false;
    private bool wasInventoryActive = false; // Запоминаем состояние инвентаря

    void Start()
    {
        // 1. ПРОВЕРКА EVENTSYSTEM
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("EventSystem не найден! Создаю автоматически...");
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // 2. ПРОВЕРКА GRAPHIC RAYCASTER
        if (menuCanvasObject != null)
        {
            if (menuCanvasObject.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogWarning("GraphicRaycaster не найден на Canvas! Добавляю...");
                menuCanvasObject.AddComponent<GraphicRaycaster>();
            }
        }

        if (menuCanvasObject != null)
            menuCanvasObject.SetActive(false);

        if (btnContinue) btnContinue.onClick.AddListener(Resume);
        if (btnSave) btnSave.onClick.AddListener(SaveDummy);
        if (btnSettings) btnSettings.onClick.AddListener(SettingsDummy);
        if (btnQuit) btnQuit.onClick.AddListener(QuitGame);
        
        Debug.Log("Меню инициализировано");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // ЗАПОМИНАЕМ состояние инвентаря ПЕРЕД отключением
        if (inventoryUI != null)
        {
            wasInventoryActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(false);
            Debug.Log($"Инвентарь отключен (был активен: {wasInventoryActive})");
        }
        
        if (menuCanvasObject != null) 
        {
            menuCanvasObject.SetActive(true);
            Debug.Log("Меню включено");
        }
        
        Debug.Log("ПАУЗА");
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (menuCanvasObject != null) 
        {
            menuCanvasObject.SetActive(false);
        }
        
        // ВОССТАНАВЛИВАЕМ состояние инвентаря
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(wasInventoryActive);
            Debug.Log($"Инвентарь восстановлен: {wasInventoryActive}");
        }
        
        Debug.Log("ПРОДОЛЖИТЬ");
    }

    public void SaveDummy() { Debug.Log("Сохранено (пустышка)"); }
    public void SettingsDummy() { Debug.Log("Настройки (пустышка)"); }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        
        try {
            var procs = Process.GetProcessesByName("spell_recognizer");
            foreach (var p in procs) p.Kill();
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