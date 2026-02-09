using UnityEngine;
using System.Diagnostics;
using UnityDebug = UnityEngine.Debug;

public class VoiceMagic : MonoBehaviour
{
    private Process process;

    // Сюда кладём заклинание, которое пришло из Python (stdout).
    // ВАЖНО: обрабатываем его только в Update() (в главном потоке Unity).
    private string pendingSpell = null;

    [Header("Префабы")]
    public GameObject fireballPrefab;

    [Header("Фильтры срабатывания голоса")]
    [Tooltip("Минимальная пауза (в секундах) между кастами.")]
    public float spellCooldownSeconds = 1.0f;

    [Tooltip("Если включено, одинаковое заклинание не будет срабатывать снова, пока не пройдёт кулдаун.")]
    public bool blockSameSpellDuringCooldown = true;

    private float lastCastTime = -999f;
    private string lastSpell = null;

    void Start()
    {
        // Путь к Python. Если Unity не видит команду "python" — укажи полный путь к python.exe
        string pythonPath = "python";

        // Путь к скрипту относительно Assets
        string scriptPath = Application.dataPath + "/Game/Scripts/Voice/spell_recognizer.py";

        process = new Process();
        process.StartInfo.FileName = pythonPath;
        process.StartInfo.Arguments = $"\"{scriptPath}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.OutputDataReceived += OnOutput;
        process.ErrorDataReceived += OnError;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        UnityEngine.Debug.Log("VoiceMagic: Python запущен, ждём заклинания...");
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data)) return;
        pendingSpell = e.Data.Trim();
    }

    void Update()
    {
        if (string.IsNullOrEmpty(pendingSpell)) return;

        // Копируем и сразу очищаем, чтобы не обработать одно и то же несколько раз.
        string spell = pendingSpell;
        pendingSpell = null;

        // Кулдаун
        float now = Time.time;
        bool cooldownReady = (now - lastCastTime) >= spellCooldownSeconds;

        if (!cooldownReady)
        {
            // Дополнительно блокируем повтор того же спелла во время кулдауна
            if (blockSameSpellDuringCooldown && spell == lastSpell)
                return;

            // Сейчас у нас общий кулдаун на всё — поэтому выходим.
            // Если захочешь разрешить разные спеллы без общего кулдауна — скажи, переделаем.
            return;
        }

        // Прошли фильтры — запоминаем время/последний спелл
        lastCastTime = now;
        lastSpell = spell;

        CastSpell(spell);
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
            UnityEngine.Debug.LogError("PYTHON ERROR: " + e.Data);
    }

    private void CastSpell(string spell)
    {
        UnityEngine.Debug.Log("CastSpell called: " + spell);

        switch (spell)
        {
            case "FIREBALL":
                UnityDebug.Log("Fireball cast");
                FireballTest();
                break;

            case "TORNADO":
                UnityDebug.Log("Tornado cast");
                break;

            case "ICE_ARROW":
                UnityDebug.Log("Ice arrow cast");
                break;
        }
    }

    private void FireballTest()
    {
        // Самая частая причина, почему "не спавнится" — префаб не назначен в Inspector.
        if (fireballPrefab == null)
        {
            UnityEngine.Debug.LogError("fireballPrefab НЕ назначен в Inspector! Перетащи префаб шара в поле Fireball Prefab.");
            return;
        }

        GameObject fireball = Instantiate(
        fireballPrefab,
        transform.position + transform.forward * 2f,
        Quaternion.identity
        );


        Destroy(fireball, 5f);

        UnityEngine.Debug.Log("Fireball: заспавнен объект (проверь Scene view, позицию 0,0,0)");
    }

    void OnApplicationQuit()
    {
        try
        {
            if (process != null && !process.HasExited)
                process.Kill();
        }
        catch { }
    }
}
