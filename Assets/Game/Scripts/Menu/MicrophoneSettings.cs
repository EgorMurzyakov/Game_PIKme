using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MicrophoneSettings : MonoBehaviour
{
    [Header("UI элементы")]
    public TMP_Dropdown microphoneDropdown;
    public Image volumeMeterFill;        // Image с типом Filled (горизонтальный)
    public Image volumeMeterBackground;  // Фоновая полоска (опционально)
    public Button applyButton;
    public Button closeButton;
    public TMP_Text statusText;

    [Header("Настройки захвата")]
    [Tooltip("Частота дискретизации для предпросмотра громкости")]
    public int sampleRate = 16000;

    [Tooltip("Сглаживание шкалы громкости (0=резко, 0.9=плавно)")]
    [Range(0f, 0.99f)]
    public float smoothing = 0.85f;

    // --- Приватные ---
    private AudioClip previewClip;
    private string selectedMicDevice;
    private int selectedDeviceIndex = 0;
    private float currentVolume = 0f;
    private bool isPreviewing = false;

    private const string PREF_MIC_INDEX = "SelectedMicIndex";
    private const string PREF_MIC_NAME  = "SelectedMicName";

    // -------------------------------------------------------
    void OnEnable()
    {
        if (applyButton)  applyButton.onClick.AddListener(OnApply);
        if (closeButton)  closeButton.onClick.AddListener(OnClose);

        // Подписываемся ПОСЛЕ того как заполним дропдаун
        PopulateDropdown();
        
        if (microphoneDropdown)
            microphoneDropdown.onValueChanged.AddListener(OnDropdownChanged);

        StartMicPreview();
    }

    void OnDisable()
    {
        StopMicPreview();

        if (applyButton)  applyButton.onClick.RemoveListener(OnApply);
        if (closeButton)  closeButton.onClick.RemoveListener(OnClose);
        if (microphoneDropdown) microphoneDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    // -------------------------------------------------------
    void Update()
    {
        if (isPreviewing && volumeMeterFill != null)
            UpdateVolumeMeter();
    }

    // -------------------------------------------------------
    // Заполнить дропдаун именами микрофонов
    // -------------------------------------------------------
    private void PopulateDropdown()
    {
        if (microphoneDropdown == null) return;

        // Снять listener перед изменением value — иначе OnDropdownChanged
        // сработает раньше времени и запустит StartMicPreview дважды
        microphoneDropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        microphoneDropdown.ClearOptions();
        string[] devices = Microphone.devices;

        if (devices.Length == 0)
        {
            microphoneDropdown.AddOptions(
                new System.Collections.Generic.List<string> { "Нет микрофонов" });
            if (statusText) statusText.text = "Микрофоны не найдены!";
            return;
        }

        microphoneDropdown.AddOptions(
            new System.Collections.Generic.List<string>(devices));

        int saved = PlayerPrefs.GetInt(PREF_MIC_INDEX, 0);
        saved = Mathf.Clamp(saved, 0, devices.Length - 1);
        microphoneDropdown.value = saved;          // теперь НЕ стреляет событие
        microphoneDropdown.RefreshShownValue();

        selectedDeviceIndex = saved;
        selectedMicDevice   = devices[saved];

        if (statusText)
            statusText.text = $"Активен: {selectedMicDevice}";
        // listener вернётся в OnEnable после этого метода
    }

    // -------------------------------------------------------
    // Пользователь сменил микрофон в дропдауне
    // -------------------------------------------------------
    private void OnDropdownChanged(int index)
    {
        string[] devices = Microphone.devices;
        if (index < 0 || index >= devices.Length) return;

        selectedDeviceIndex = index;
        selectedMicDevice   = devices[index];

        // Перезапустить предпросмотр с новым устройством
        StopMicPreview();
        StartMicPreview();

        if (statusText)
            statusText.text = $"Предпросмотр: {selectedMicDevice}";
    }

    // -------------------------------------------------------
    // Применить и перезапустить процесс
    // -------------------------------------------------------
    private void OnApply()
    {
        PlayerPrefs.SetInt(PREF_MIC_INDEX, selectedDeviceIndex);
        PlayerPrefs.SetString(PREF_MIC_NAME, selectedMicDevice);
        PlayerPrefs.Save();

        if (statusText)
            statusText.text = "Сохранено! Перезапуск...";

        // Найти индекс среди ВСЕХ sounddevice-устройств для Python-скрипта
        int pythonDeviceId = GetPythonDeviceId(selectedMicDevice);

        if (VoiceProcessManager.Instance != null)
            VoiceProcessManager.Instance.RestartWithDevice(pythonDeviceId);

        StartCoroutine(ShowSavedFeedback());
    }

    private void OnClose()
    {
        gameObject.SetActive(false);

        // Возобновить паузу (настройки открыты поверх меню паузы)
        if (SimpleMenu.Instance != null)
            SimpleMenu.Instance.ShowPauseMenu();
    }

    // -------------------------------------------------------
    // Захват микрофона для предпросмотра громкости
    // -------------------------------------------------------
    private void StartMicPreview()
    {
        if (string.IsNullOrEmpty(selectedMicDevice) && Microphone.devices.Length == 0) return;

        string device = Microphone.devices.Length > 0 ? selectedMicDevice : null;
        previewClip = Microphone.Start(device, true, 1, sampleRate);
        isPreviewing = true;
    }

    private void StopMicPreview()
    {
        isPreviewing = false;
        if (previewClip != null)
        {
            // Проверяем что устройство реально пишет прежде чем его останавливать
            if (!string.IsNullOrEmpty(selectedMicDevice) &&
                Microphone.IsRecording(selectedMicDevice))
            {
                Microphone.End(selectedMicDevice);
            }
            Destroy(previewClip);
            previewClip = null;
        }
    }

    // -------------------------------------------------------
    // Считать RMS громкость и обновить шкалу
    // -------------------------------------------------------
    private void UpdateVolumeMeter()
    {
        if (previewClip == null) return;

        int sampleWindow = 1024;
        float[] samples = new float[sampleWindow];

        int pos = Microphone.GetPosition(selectedMicDevice) - sampleWindow;
        if (pos < 0) return;

        previewClip.GetData(samples, pos);

        // RMS
        float sum = 0f;
        foreach (float s in samples)
            sum += s * s;

        float rms = Mathf.Sqrt(sum / sampleWindow);
        float db  = 20f * Mathf.Log10(rms + 1e-6f); // в децибелах

        // Нормализовать: -60 dB → 0, 0 dB → 1
        float normalized = Mathf.InverseLerp(-60f, 0f, db);

        // Сглаживание
        currentVolume = Mathf.Lerp(currentVolume, normalized, 1f - smoothing);

        volumeMeterFill.fillAmount = currentVolume;

        // Цвет: зелёный → жёлтый → красный
        volumeMeterFill.color = currentVolume < 0.6f
            ? Color.Lerp(new Color(0.2f, 0.8f, 0.3f), new Color(1f, 0.85f, 0f), currentVolume / 0.6f)
            : Color.Lerp(new Color(1f, 0.85f, 0f), new Color(0.9f, 0.2f, 0.2f), (currentVolume - 0.6f) / 0.4f);
    }

    // -------------------------------------------------------
    // Получить ID устройства для Python (sounddevice нумерует
    // все устройства системы, а не только микрофоны)
    // -------------------------------------------------------
    private int GetPythonDeviceId(string unityMicName)
    {
        // Unity и sounddevice используют похожие имена устройств.
        // Сохраняем индекс Unity и передаём его Python напрямую.
        // Если имена не совпадают — запускаем --pick в консоли.
        return selectedDeviceIndex;
    }

    private IEnumerator ShowSavedFeedback()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        if (statusText)
            statusText.text = $"Активен: {selectedMicDevice}";
    }
}