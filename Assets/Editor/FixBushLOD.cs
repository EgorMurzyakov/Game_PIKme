#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FixBushLODPrefabs : EditorWindow
{
    [MenuItem("Tools/Fix Bush Prefabs LOD")]
    static void FixLOD()
    {
        // Ищем все префабы с "bush" в названии
        string[] guids = AssetDatabase.FindAssets("bush t:Prefab");
        
        Debug.Log($"Found {guids.Length} bush prefabs");
        
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            LODGroup[] lodGroups = prefab.GetComponentsInChildren<LODGroup>(true);

            foreach (LODGroup group in lodGroups)
            {
                Debug.Log($"Fixing: {path}");

                LOD[] lods = group.GetLODs();

                if (lods.Length >= 3)
                {
                    lods[0].screenRelativeTransitionHeight = 0.16f;
                    lods[1].screenRelativeTransitionHeight = 0.08f;
                    lods[2].screenRelativeTransitionHeight = 0.04f;
                }
                else if (lods.Length == 2)
                {
                    lods[0].screenRelativeTransitionHeight = 0.16f;
                    lods[1].screenRelativeTransitionHeight = 0.06f;
                }
                else if (lods.Length == 1)
                {
                    lods[0].screenRelativeTransitionHeight = 0.16f;
                }

                group.SetLODs(lods);
                group.RecalculateBounds();
                count++;
            }

            // Сохраняем префаб
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Done! Fixed {count} LODGroups in prefabs");
    }
}
#endif