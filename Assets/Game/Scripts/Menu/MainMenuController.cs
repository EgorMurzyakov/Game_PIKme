using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug; // Чтобы не путать с системным Debug

public class MainMenuController : MonoBehaviour
{
    [Header("Настройки сцены")]
    [SerializeField] private string gameSceneName = "GameLevel1"; // Имя сцены игры

    // 1. Кнопка "Начать игру"
    public void StartGame()
    {
        // Создаём менеджер голоса — он начнёт грузиться пока играет заставка
        if (VoiceProcessManager.Instance == null)
        {
            GameObject go = new GameObject("VoiceProcessManager");
            go.AddComponent<VoiceProcessManager>();
        }

        SceneManager.LoadScene("IntroScene");
    }

    //2. Продолжить игру
    public void ContinueGame()
    {
        if (VoiceProcessManager.Instance == null)
        {
            GameObject go = new GameObject("VoiceProcessManager");
            go.AddComponent<VoiceProcessManager>();
        }

        PlayerPrefs.SetString("TargetScene", "village1 valera");
        SceneManager.LoadScene("LoadingScreen");
    }

    // 3. Кнопка "Выход"
    public void QuitGame()
    {
#if UNITY_EDITOR
        // В редакторе Unity нельзя сделать Application.Quit(), поэтому просто останавливаем плеймод
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // В билде закрываем приложение
        Application.Quit();
#endif
    }
}
