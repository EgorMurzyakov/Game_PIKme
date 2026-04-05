using UnityEngine;
using UnityEditor;

public class FixAllMaterialsUnity6 : MonoBehaviour
{
    [MenuItem("Tools/Fix ALL FBX Materials (Unity 6)")]
    static void FixAll()
    {
        // Ищем ВСЕ модели во всём проекте
        string[] fbxFiles = AssetDatabase.FindAssets("t:model");

        int count = 0;
        foreach (string guid in fbxFiles)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Пропускаем пакеты Unity
            if (path.StartsWith("Packages/")) continue;

            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                importer.materialSearch = ModelImporterMaterialSearch.Everywhere;

                AssetDatabase.WriteImportSettingsIfDirty(path);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ ГОТОВО! Исправлено {count} FBX файлов во всём проекте.");
    }
}