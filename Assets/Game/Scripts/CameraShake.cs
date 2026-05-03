using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Настройки дрожания")]
    [SerializeField] private float delay = 0f;        // Через сколько секунд начать
    [SerializeField] private float duration = 1f;     // Продолжительность дрожания
    [SerializeField] private float strength = 0.5f;   // Сила дрожания

    [Header("Плавность")]
    [SerializeField] private float fadeInTime = 0.2f;  // Время нарастания (сек)
    [SerializeField] private float fadeOutTime = 0.4f; // Время затухания (сек)

    [Header("Запуск")]
    [SerializeField] private bool playOnStart = false; // Запустить при старте сцены

    private Vector3 originalLocalPos;
    private bool isShaking = false;

    private void Start()
    {
        originalLocalPos = transform.localPosition;

        if (playOnStart)
            Shake();
    }

    public void Shake()
    {
        if (isShaking)
        {
            StopAllCoroutines();
            transform.localPosition = originalLocalPos;
            isShaking = false;
        }

        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        // Задержка перед началом
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Вычисляем множитель с плавным нарастанием и затуханием
            float fadeInFactor = (fadeInTime > 0f) ? Mathf.Clamp01(elapsed / fadeInTime) : 1f;
            float fadeOutFactor = (fadeOutTime > 0f) ? Mathf.Clamp01((duration - elapsed) / fadeOutTime) : 1f;
            float currentStrength = strength * fadeInFactor * fadeOutFactor;

            // Случайное смещение
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * currentStrength;

            transform.localPosition = originalLocalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPos;
        isShaking = false;
    }

    // Вызов из другого скрипта с кастомными параметрами
    public void Shake(float _duration, float _strength, float _fadeIn, float _fadeOut, float _delay = 0f)
    {
        duration = _duration;
        strength = _strength;
        fadeInTime = _fadeIn;
        fadeOutTime = _fadeOut;
        delay = _delay;
        Shake();
    }
}