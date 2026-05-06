using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityDebug = UnityEngine.Debug;

public class VoiceMagic : MonoBehaviour
{
    [Header("Фильтры срабатывания голоса")]
    [Tooltip("Минимальная пауза (в секундах) между кастами.")]
    public float spellCooldownSeconds = 1.0f;

    [Tooltip("Если включено, одинаковое заклинание не будет срабатывать снова, пока не пройдёт кулдаун.")]
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
    [Tooltip("Включить управление заклинаниями с клавиатуры")]
    public bool enableKeyboardSpells = true;

    public string PendingSpell => Interlocked.Exchange(ref pendingSpell, null);

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        // Процесс запускается в VoiceProcessManager, здесь ничего не делаем
        if (VoiceProcessManager.Instance == null)
        {
            UnityDebug.LogWarning("VoiceMagic: VoiceProcessManager не найден!");

            UnityDebug.LogWarning("VoiceMagic: VoiceProcessManager запущен вручную!");
        }
    }

    void Update()
    {
        // Клавиатура — без изменений
        if (enableKeyboardSpells)
        {
            if (Input.GetKeyDown(KeyCode.Space)) HandleSpellCast("TORNADO");
            if (Input.GetKeyDown(KeyCode.F))     HandleSpellCast("FIREBALL");
            if (Input.GetKeyDown(KeyCode.R))     HandleSpellCast("ICE_ARROW");
        }

        // Голосовые команды — берём из менеджера
        if (VoiceProcessManager.Instance != null)
        {
            string spell = VoiceProcessManager.Instance.PendingSpell;
            if (!string.IsNullOrEmpty(spell))
                HandleSpellCast(spell);
        }
        else
        {
            // Если VoiceProcessManager не найден, используем свой PendingSpell
            string spell = PendingSpell;
            if (!string.IsNullOrEmpty(spell))
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
                if (!CanUseTornado())
                {
                    UnityDebug.LogWarning("VoiceMagic: книга Торнадо не найдена в инвентаре.");
                    return;
                }
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


    private bool CanUseTornado()
    {
        if (inventoryManager == null)
            return false;

        return inventoryManager.HasTornadoBook();
    }

}
