using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeBrushData))]
public class TreeBrushEditor : Editor
{
    TreeBrushData brush;
    private bool isPainting = false;

    void OnEnable()
    {
        brush = (TreeBrushData)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        if (!IsValid()) return;

        Vector3 brushPos = GetBrushPosition();
        if (brushPos != Vector3.zero)
        {
            Handles.color = brush.brushColor;
            Handles.DrawSolidDisc(brushPos, Vector3.up, brush.brushSize);
            
            Handles.color = Color.green;
            Handles.DrawWireDisc(brushPos, Vector3.up, brush.brushSize);
        }

        Event e = Event.current;

        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                isPainting = true;
                e.Use();
            }
            else if (e.type == EventType.MouseUp)
            {
                isPainting = false;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && isPainting)
            {
                PaintTrees(brushPos);
                e.Use();
            }
        }
    }

    private bool IsValid()
    {
        if (brush == null) return false;
        if (brush.treePrefabs == null || brush.treePrefabs.Length == 0)
        {
            EditorGUILayout.HelpBox("⚠️ Назначь Tree Prefabs!", MessageType.Warning);
            return false;
        }
        return true;
    }

    private Vector3 GetBrushPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, brush.surfaceLayers))
        {
            // Игнорируем указанные слои
            string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
            foreach (string ignoreLayer in brush.ignoreLayerNames)
            {
                if (layerName == ignoreLayer)
                    return Vector3.zero;
            }
            
            return hit.point;
        }
        return Vector3.zero;
    }

    private void PaintTrees(Vector3 centerPos)
    {
        if (centerPos == Vector3.zero || !IsValid()) return;

        for (int i = 0; i < brush.density; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * brush.brushSize;
            Vector3 testPos = centerPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            Ray ray = new Ray(testPos + Vector3.up * 50f, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, brush.surfaceLayers))
                continue;
            
            // Игнорируем указанные слои
            string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
            bool shouldIgnore = false;
            foreach (string ignoreLayer in brush.ignoreLayerNames)
            {
                if (layerName == ignoreLayer)
                {
                    shouldIgnore = true;
                    break;
                }
            }
            if (shouldIgnore)
                continue;
            
            // Проверка угла наклона
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > brush.maxSlopeAngle)
                continue;

            // Создание префаба
            GameObject prefab = brush.treePrefabs[Random.Range(0, brush.treePrefabs.Length)];
            GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(prefab, brush.transform);
            
            if (tree == null) 
            {
                tree = Instantiate(prefab, brush.transform);
            }

            tree.transform.position = hit.point;

            // Масштаб
            float scale = Random.Range(brush.minScale, brush.maxScale);
            tree.transform.localScale = Vector3.one * scale;

            // Поворот
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (brush.randomRotationY)
            {
                rot *= Quaternion.Euler(0, Random.Range(0f, brush.rotationYVariance), 0);
            }
            tree.transform.rotation = rot;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("🛠 Инструменты", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🗑️ Clear All Trees", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Подтверждение", 
                "Удалить все деревья?", 
                "Да", "Отмена"))
            {
                brush.ClearAllTrees();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("InfoBox");
        EditorGUILayout.LabelField("💡 Кисть работает на ЛЮБОЙ поверхности!", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Просто выбери объект TreeBrush и рисуй в Scene", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
    }
}