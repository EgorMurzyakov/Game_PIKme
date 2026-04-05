using UnityEngine;
using UnityEditor;
using System.Linq;

public class SmartShadowOptimizer : MonoBehaviour
{
    // 🎯 НАСТРОЙКИ (меняйте под свою сцену)
    [System.Serializable]
    public class ShadowSettings
    {
        public float keepSoftShadowsIfRangeAbove = 2f;   // Мягкие тени только у крупных прожекторов
        public float keepHardShadowsIfRangeAbove = 0.8f; // Жёсткие у средних, остальное → без теней
        public string[] keepShadowIfNameContains = { "Main", "Key", "Player", "Sun" }; // Имена ламп, у которых ВСЕГДА оставлять тени
    }

    public static ShadowSettings settings = new ShadowSettings();

    [MenuItem("Tools/Optimize: Smart Shadow Cleanup (RECOMMENDED)")]
    static void OptimizeShadowsSmart()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        int softKept = 0, hardKept = 0, disabled = 0, skipped = 0;

        foreach (Light light in allLights)
        {
            // Пропускаем главный направленный свет (солнце)
            if (light.type == LightType.Directional)
            {
                skipped++;
                continue;
            }

            // Проверка по имени (если лампа важная — оставляем тени)
            bool isImportant = settings.keepShadowIfNameContains
                .Any(keyword => light.name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0);

            if (isImportant)
            {
                light.shadows = LightShadows.Soft;
                softKept++;
                continue;
            }

            // Проверка по радиусу
            if (light.range >= settings.keepSoftShadowsIfRangeAbove)
            {
                light.shadows = LightShadows.Soft;
                softKept++;
            }
            else if (light.range >= settings.keepHardShadowsIfRangeAbove)
            {
                light.shadows = LightShadows.Hard;
                hardKept++;
            }
            else
            {
                light.shadows = LightShadows.None;
                disabled++;
            }
        }

        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Smart Shadow Cleanup завершён!");
        Debug.Log($"   🟢 Мягкие тени оставлены: {softKept} (большие/важные лампы)");
        Debug.Log($"   🟡 Жёсткие тени оставлены: {hardKept} (средние лампы)");
        Debug.Log($"   🔴 Тени отключены: {disabled} (мелкий декор)");
        Debug.Log($"   ⚪ Пропущено (Directional): {skipped}");
        Debug.Log($"   💡 Итого активных теней: {softKept + hardKept + skipped} (было {allLights.Length})");
    }

    // 🔧 Быстрая настройка: отключить ВСЕ тени кроме направленного света
    [MenuItem("Tools/Optimize: Disable ALL Point/Spot Shadows (MAX PERFORMANCE)")]
    static void DisableAllNonDirectionalShadows()
    {
        Light[] allLights = FindObjectsOfType<Light>();
        int count = 0;

        foreach (Light light in allLights)
        {
            if (light.type != LightType.Directional)
            {
                light.shadows = LightShadows.None;
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Все тени у точечных/прожекторных ламп отключены ({count} ламп). Остался только Directional Light.");
    }
}