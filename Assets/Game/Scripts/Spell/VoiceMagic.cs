using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityDebug = UnityEngine.Debug;

public class VoiceMagic : MonoBehaviour
{
    [Header("Фильтры срабатывания голоса")]
    public float spellCooldownSeconds = 1.0f;
    public bool blockSameSpellDuringCooldown = true;

    private float lastCastTime = -999f;
    private string lastSpell = null;
    private Process process;
    private string pendingSpell;

    public Transform cameraTransform;
    public Transform playerTransform;

    [Header("Спеллы (компоненты)")]
    public FireballSpell fireballSpell;
    public TornadoSpell tornadoSpell;

    [Header("Связи")]
    public InventoryManager inventoryManager;

    [Header("Управление с клавиатуры")]
    public bool enableKeyboardSpells = true;

    // Звуки
    private AudioSource audioSource;
    private AudioClip fireballClip;
    private AudioClip tornadoClip;

    public string PendingSpell => Interlocked.Exchange(ref pendingSpell, null);

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Загружаем звуки из StreamingAssets
        StartCoroutine(LoadClip("sounds/fireball.wav", clip => fireballClip = clip));
        StartCoroutine(LoadClip("sounds/tornado.wav", clip => tornadoClip = clip));

        if (VoiceProcessManager.Instance == null)
        {
            UnityDebug.LogWarning("VoiceMagic: VoiceProcessManager не найден!");
            StartVoiceProcess();
            UnityDebug.LogWarning("VoiceMagic: VoiceProcessManager запущен вручную!");
        }
    }

    private IEnumerator LoadClip(string relativePath, System.Action<AudioClip> onLoaded)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string url = "file://" + fullPath;

        using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
                onLoaded?.Invoke(clip);
                UnityDebug.Log("VoiceMagic: звук загружен — " + relativePath);
            }
            else
            {
                UnityDebug.LogError("VoiceMagic: не удалось загрузить звук: " + relativePath + " | " + req.error);
            }
        }
    }

    void Update()
    {
        if (enableKeyboardSpells)
        {
            if (Input.GetKeyDown(KeyCode.Space)) HandleSpellCast("TORNADO");
            if (Input.GetKeyDown(KeyCode.F))     HandleSpellCast("FIREBALL");
            if (Input.GetKeyDown(KeyCode.R))     HandleSpellCast("ICE_ARROW");
        }

        if (VoiceProcessManager.Instance != null)
        {
            string spell = VoiceProcessManager.Instance.PendingSpell;
            if (!string.IsNullOrEmpty(spell))
                HandleSpellCast(spell);
        }
        else
        {
            string spell = PendingSpell;
            if (!string.IsNullOrEmpty(spell))
                HandleSpellCast(spell);
        }
    }

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

    private void CastSpell(string spell)
    {
        UnityDebug.Log("CastSpell called: " + spell);

        if (cameraTransform == null)
        {
            UnityDebug.LogError("cameraTransform не задан и Camera.main не найден.");
            return;
        }

        switch (spell)
        {
            case "FIREBALL":
                UnityDebug.Log("Fireball cast");
                if (fireballSpell == null)
                {
                    UnityDebug.LogError("fireballSpell не назначен в Inspector.");
                    return;
                }
                audioSource.PlayOneShot(fireballClip);
                fireballSpell.Cast(transform, cameraTransform);
                break;

            case "TORNADO":
                UnityDebug.Log("Tornado cast");
                if (!CanUseTornado())
                {
                    UnityDebug.LogWarning("VoiceMagic: книга Торнадо не найдена в инвентаре.");
                    return;
                }
                if (tornadoSpell == null)
                {
                    UnityDebug.LogError("tornadoSpell не назначен в Inspector.");
                    return;
                }
                audioSource.PlayOneShot(tornadoClip);
                tornadoSpell.Cast(transform, playerTransform);
                break;
        }
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
                UnityDebug.LogError("VoiceMagic: файл не найден: " + exePath);
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

            UnityDebug.Log("VoiceMagic: процесс запущен.");
        }
        catch (System.Exception e)
        {
            UnityDebug.LogError("VoiceMagic: ошибка запуска: " + e.Message);
        }
    }

    private bool CanUseTornado()
    {
        if (inventoryManager == null)
            return false;
        return inventoryManager.HasTornadoBook();
    }
}