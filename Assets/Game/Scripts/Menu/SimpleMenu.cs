using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // <--- ВАЖНО: для EventSystem
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
    public GameObject inventoryUI; // Перетащи сюда корень инвентаря

    private bool isPaused = false;

    void Start()
    {
        // 1. ПРОВЕРКА EVENTSYSTEM (Критично для новой сцены!)
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("EventSystem не найден! Создаю автоматически...");
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            // Для старых версий Unity:
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

        // Скрываем меню при старте
        if (menuCanvasObject != null)
            menuCanvasObject.SetActive(false);

        // Подписываем кнопки
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
        
        // ВАЖНО: Отключаем инвентарь, чтобы он не блокировал клики!
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            Debug.Log("Инвентарь отключен");
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
        
        // Инвентарь можно включить обратно, если нужно
        // if (inventoryUI != null) inventoryUI.SetActive(true);
        
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