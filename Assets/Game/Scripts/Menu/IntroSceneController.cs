using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string nextSceneName = "village1 valera"; // сцена после заставки
    [SerializeField] private VideoClip introClip;               // перетащи видео сюда

    private VideoPlayer videoPlayer;
    private RawImage screen;

    void Start()
    {
        // Создаём VideoPlayer на камере
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.clip = introClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;

        // Создаём RenderTexture под размер видео
        RenderTexture rt = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = rt;

        // Подключаем к RawImage на Canvas
        screen = FindObjectOfType<RawImage>();
        if (screen != null)
            screen.texture = rt;

        // Подписываемся на событие окончания видео
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}