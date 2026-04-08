using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics; // Нужно для управления процессами
using Debug = UnityEngine.Debug; // Чтобы не путать с системным Debug

public class MainMenuController : MonoBehaviour
{
    [Header("Настройки сцены")]
    [SerializeField] private string gameSceneName = "GameLevel1"; // Имя сцены игры

    [Header("Настройки процесса голосового ассистента")]
    [SerializeField] private string processName = "spell_recognizer"; // Имя процесса БЕЗ .exe

    // 1. Кнопка "Начать игру"
    public void StartGame()
    {

        SceneManager.LoadScene(gameSceneName);

    }

    // 2. Кнопка "Выход"
    public void QuitGame()
    {
        KillVoiceProcess();

#if UNITY_EDITOR
        // В редакторе Unity нельзя сделать Application.Quit(), поэтому просто останавливаем плеймод
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // В билде закрываем приложение
        Application.Quit();
#endif
    }

    // Метод для принудительного завершения процесса
    private void KillVoiceProcess()
    {
        try
        {
            // Ищем все процессы с таким именем
            var processes = Process.GetProcessesByName(processName);
            
            if (processes.Length > 0)
            {
                foreach (var proc in processes)
                {
                    // Проверяем путь, чтобы случайно не убить чужой процесс с таким же именем
                    // Это опционально, но рекомендуется для безопасности
                    if (!string.IsNullOrEmpty(proc.MainModule?.FileName))
                    {
                         // Можно добавить проверку пути, если нужно:
                         // if (proc.MainModule.FileName.Contains("StreamingAssets")) 
                         
                        proc.Kill();
                        proc.WaitForExit(); // Ждем завершения
                        Debug.Log($"Процесс {processName} успешно завершен.");
                    }
                }
            }
            else
            {
                Debug.Log($"Процесс {processName} не запущен или уже закрыт.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при завершении процесса {processName}: {e.Message}");
        }
    }
}