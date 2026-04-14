using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityDebug = UnityEngine.Debug;

public class VoiceProcessManager : MonoBehaviour
{
    public static VoiceProcessManager Instance { get; private set; }

    private Process process;
    private string pendingSpell = null;

    public string PendingSpell => Interlocked.Exchange(ref pendingSpell, null);

    void Awake()
    {
        // Singleton + DontDestroyOnLoad
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartVoiceProcess();
    }

    private void StartVoiceProcess()
    {
        try
        {
            string exePath = Path.Combine(
                Application.streamingAssetsPath,
                "Voice/spell_recognizer/spell_recognizer.exe"
            );

            if (!File.Exists(exePath))
            {
                UnityDebug.LogError("VoiceProcessManager: файл не найден: " + exePath);
                return;
            }

            process = new Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Interlocked.Exchange(ref pendingSpell, e.Data.Trim());
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityDebug.LogError("SPELL_RECOGNIZER ERR: " + e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UnityDebug.Log("VoiceProcessManager: процесс запущен.");
        }
        catch (System.Exception e)
        {
            UnityDebug.LogError("VoiceProcessManager: ошибка запуска: " + e.Message);
        }
    }

    public bool IsProcessAlive =>
        process != null && !process.HasExited;

    void OnApplicationQuit()
    {
        try
        {
            if (process != null && !process.HasExited)
                process.Kill();
            process?.Dispose();
        }
        catch { }
    }
}