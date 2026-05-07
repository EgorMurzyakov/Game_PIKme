using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Music Tracks")]
    [SerializeField] private AudioClip normalMusic;  // Обычная музыка
    [SerializeField] private AudioClip battleMusic;  // Боевая музыка

    [Header("Settings")]
    [SerializeField] private float fadeTime = 1f;    // Время плавного переключения

    private AudioSource audioSource;
    private AudioClip currentNormalMusic; // Для хранения обычного трека

    void Start()
    {
        // Получаем или добавляем AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Настройка
        audioSource.loop = true;
        audioSource.clip = normalMusic;
        audioSource.Play();
    }

    // Публичный метод для переключения музыки
    public void SwitchToBattleMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToNewTrack(battleMusic));
    }

    public void SwitchToNormalMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToNewTrack(normalMusic));
    }

    private IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        // Плавно убавляем громкость
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        // Меняем трек
        audioSource.clip = newClip;
        audioSource.Play();

        // Плавно прибавляем громкость
        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = startVolume;
    }
}