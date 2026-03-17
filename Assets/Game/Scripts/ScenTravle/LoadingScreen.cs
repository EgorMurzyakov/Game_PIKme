// LoadingScreen.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private float minDisplayTime = 2f; // минимум сколько показывать картинку

    private void Start()
    {
        string targetScene = PlayerPrefs.GetString("TargetScene");
        StartCoroutine(LoadScene(targetScene));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        // Начинаем грузить сцену в фоне
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // не переключаем сразу

        float elapsed = 0f;

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;

            // Ждём пока И сцена загрузилась И прошло минимальное время
            if (op.progress >= 0.9f && elapsed >= minDisplayTime)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}