using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityDebug = UnityEngine.Debug;

public class VoiceMagic : MonoBehaviour
{
    private Process process;

    private string pendingSpell = null;

    [Header("Фильтры срабатывания голоса")]
    [Tooltip("Минимальная пауза (в секундах) между кастами.")]
    public float spellCooldownSeconds = 1.0f;

    [Tooltip("Если включено, одинаковое заклинание не будет срабатывать снова, пока не пройдёт кулдаун.")]
    public bool blockSameSpellDuringCooldown = true;

    private float lastCastTime = -999f;
    private string lastSpell = null;

    [Header("Проверка процесса")]
    [Tooltip("Интервал проверки запущен ли процесс (в секундах).")]
    public float processCheckIntervalSeconds = 5f;
    private float lastProcessCheckTime = 0f;

    public Transform cameraTransform;
    public Transform playerTransform;

    [Header("Спеллы (компоненты)")]
    public FireballSpell fireballSpell;
    public TornadoSpell tornadoSpell;

    [Header("Управление с клавиатуры")]
    [Tooltip("Включить управление заклинаниями с клавиатуры")]
    public bool enableKeyboardSpells = true;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        StartVoiceProcess();
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data)) return;
        Interlocked.Exchange(ref pendingSpell, e.Data.Trim());
    }

    void Update()
    {
        // ===== УПРАВЛЕНИЕ С КЛАВИАТУРЫ =====
        if (enableKeyboardSpells)
        {
            // Пробел для торнадо
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnityDebug.Log("Клавиатура: Пробел нажат - кастуем торнадо!");
                HandleSpellCast("TORNADO");
            }

            // Опционально: Огненный шар на F
            if (Input.GetKeyDown(KeyCode.F))
            {
                UnityDebug.Log("Клавиатура: F нажат - кастуем огненный шар!");
                HandleSpellCast("FIREBALL");
            }

            // Опционально: Ледяная стрела на R
            if (Input.GetKeyDown(KeyCode.R))
            {
                UnityDebug.Log("Клавиатура: R нажат - кастуем ледяную стрелу!");
                HandleSpellCast("ICE_ARROW");
            }
        }
        // ===== КОНЕЦ УПРАВЛЕНИЯ С КЛАВИАТУРЫ =====

        // Периодичная проверка процесса (для голоса)
        float now = Time.time;
        if (now - lastProcessCheckTime >= processCheckIntervalSeconds)
        {
            lastProcessCheckTime = now;
            CheckAndRestartProcess();
        }

        // Обработка голосовых команд
        string spell = Interlocked.Exchange(ref pendingSpell, null);
        if (!string.IsNullOrEmpty(spell))
        {
            HandleSpellCast(spell);
        }
    }

    // Общий метод для обработки заклинаний (и с голоса, и с клавиатуры)
    private void HandleSpellCast(string spell)
    {
        float now = Time.time;
        bool cooldownReady = (now - lastCastTime) >= spellCooldownSeconds;

        if (!cooldownReady)
        {
            if (blockSameSpellDuringCooldown && spell == lastSpell)
                return;
            return;
        }

        lastCastTime = now;
        lastSpell = spell;

        CastSpell(spell);
    }

    private void CheckAndRestartProcess()
    {
        if (process == null || process.HasExited)
        {
            UnityEngine.Debug.LogWarning("VoiceMagic: процесс завершился, перезапускаю...");
            StartVoiceProcess();
        }
    }

    private void StartVoiceProcess()
    {
        try
        {
            if (process != null && !process.HasExited)
                return; // Процесс уже запущен

            string exePath = Path.Combine(
                Application.streamingAssetsPath,
                "Voice/spell_recognizer/spell_recognizer.exe"
            );
            UnityEngine.Debug.Log("VoiceMagic: запускаю exe по пути: " + exePath);

            if (!File.Exists(exePath))
            {
                UnityEngine.Debug.LogError("VoiceMagic: НЕ найден exe по пути: " + exePath);
                return;
            }

            process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);

            process.OutputDataReceived += OnOutput;
            process.ErrorDataReceived += OnError;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UnityEngine.Debug.Log("VoiceMagic: exe запущен, ждём заклинания...");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("VoiceMagic: ошибка при запуске процесса: " + e.Message);
        }
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
            UnityEngine.Debug.LogError("PYTHON ERROR: " + e.Data);
    }

    private void CastSpell(string spell)
    {
        UnityEngine.Debug.Log("CastSpell called: " + spell);

        if (cameraTransform == null)
        {
            UnityEngine.Debug.LogError("cameraTransform не задан и Camera.main не найден.");
            return;
        }

        switch (spell)
        {
            case "FIREBALL":
                UnityDebug.Log("Fireball cast");
                if (fireballSpell == null)
                {
                    UnityEngine.Debug.LogError("fireballSpell не назначен в Inspector.");
                    return;
                }
                fireballSpell.Cast(transform, cameraTransform);
                break;

            case "TORNADO":
                UnityDebug.Log("Tornado cast");
                if (tornadoSpell == null)
                {
                    UnityEngine.Debug.LogError("tornadoSpell не назначен в Inspector.");
                    return;
                }
                tornadoSpell.Cast(transform, playerTransform);
                break;

            case "ICE_ARROW":
                UnityDebug.Log("Ice arrow cast");
                // Добавьте логику для ледяной стрелы, если есть
                break;
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            if (process != null)
            {
                if (!process.HasExited)
                    process.Kill();
                process.Dispose();
                process = null;
            }
        }
        catch { }
    }
}