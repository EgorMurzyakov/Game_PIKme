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

    public Transform cameraTransform;

    [Header("Спеллы (компоненты)")]
    public FireballSpell fireballSpell;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        string exePath = Path.Combine(
            Application.dataPath,
            "Game/Scripts/Voice/dist/spell_recognizer/spell_recognizer.exe"
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

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data)) return;
        Interlocked.Exchange(ref pendingSpell, e.Data.Trim());
    }

    void Update()
    {
        string spell = Interlocked.Exchange(ref pendingSpell, null);
        if (string.IsNullOrEmpty(spell)) return;

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
                break;

            case "ICE_ARROW":
                UnityDebug.Log("Ice arrow cast");
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
