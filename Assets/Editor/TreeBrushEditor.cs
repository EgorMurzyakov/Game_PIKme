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

        // Рисуем круг кисти
        Vector3 brushPos = GetBrushPosition();
        if (brushPos != Vector3.zero)
        {
            Handles.color = brush.brushColor;
            Handles.DrawSolidDisc(brushPos, Vector3.up, brush.brushSize);
            
            Handles.color = Color.green;
            Handles.DrawWireDisc(brushPos, Vector3.up, brush.brushSize);
            
            // Подпись с размером кисти
            Handles.Label(brushPos + Vector3.up * 0.5f, 
                $"Radius: {brush.brushSize:F1}m", 
                new GUIStyle(EditorStyles.label) { fontSize = 12 });
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
        if (brush.treePrefabs == null || brush.treePrefabs.Length == 0) return false;
        
        if (brush.useTerrain)
        {
            return brush.targetTerrain != null && brush.targetTerrain.terrainData != null;
        }
        else
        {
            return brush.targetMesh != null && brush.targetMesh.sharedMesh != null;
        }
    }

    private Vector3 GetBrushPosition()
{
    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    
    // Используем Physics.Raycast для обоих режимов
    if (Physics.Raycast(ray, out RaycastHit hit, 10000f))
    {
        if (brush.useTerrain)
        {
            // Для Terrain проверяем компонент Terrain на объекте
            Terrain hitTerrain = hit.transform.GetComponent<Terrain>();
            if (hitTerrain != null && hitTerrain == brush.targetTerrain)
            {
                return hit.point;
            }
        }
        else
        {
            // Для Mesh проверяем, что попали в нужный MeshFilter
            MeshFilter hitMeshFilter = hit.transform.GetComponent<MeshFilter>();
            if (hitMeshFilter != null && hitMeshFilter == brush.targetMesh)
            {
                return hit.point;
            }
        }
    }
    return Vector3.zero;
}

    private Vector3 GetSurfaceNormal(Vector3 worldPos)
    {
        if (brush.useTerrain && brush.targetTerrain != null)
        {
            TerrainData terrainData = brush.targetTerrain.terrainData;
            Vector3 terrainPos = worldPos - brush.targetTerrain.transform.position;
            
            // Нормализуем координаты для TerrainData (0..1)
            float normalizedX = terrainPos.x / terrainData.size.x;
            float normalizedZ = terrainPos.z / terrainData.size.z;
            
            return terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
        }
        else if (brush.targetMesh != null)
        {
            // Raycast для получения нормали меши
            Ray ray = new Ray(worldPos + Vector3.up * 10f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f))
            {
                return hit.normal;
            }
            return Vector3.up;
        }
        return Vector3.up;
    }

        private void PaintTrees(Vector3 centerPos)
    {
        if (centerPos == Vector3.zero || !IsValid()) return;

        for (int i = 0; i < brush.density; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * brush.brushSize;
            Vector3 testPos = centerPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            Vector3 spawnPos;
            Vector3 surfaceNormal;

            // === ЛОГИКА ПОЗИЦИОНИРОВАНИЯ ===
            if (brush.useTerrain && brush.targetTerrain != null)
            {
                float height = brush.targetTerrain.SampleHeight(testPos);
                spawnPos = new Vector3(testPos.x, height, testPos.z);
                surfaceNormal = GetSurfaceNormal(spawnPos);
            }
            else if (brush.targetMesh != null)
            {
                Ray ray = new Ray(testPos + Vector3.up * 50f, Vector3.down);
                if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
                    continue;
                    
                MeshFilter hitMeshFilter = hit.transform.GetComponent<MeshFilter>();
                if (hitMeshFilter == null || hitMeshFilter != brush.targetMesh)
                    continue;
                    
                spawnPos = hit.point;
                surfaceNormal = hit.normal;
            }
            else
            {
                continue;
            }

            // Проверка угла наклона
            float angle = Vector3.Angle(Vector3.up, surfaceNormal);
            if (angle > brush.maxSlopeAngle)
                continue;

            // === СОЗДАНИЕ ПРЕФАБА ===
            GameObject prefab = brush.treePrefabs[Random.Range(0, brush.treePrefabs.Length)];
            
            // Используем PrefabUtility для корректного создания в редакторе
            GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(prefab, brush.transform);
            
            if (tree == null) 
            {
                // Если PrefabUtility не сработал (редко), пробуем обычный Instantiate
                tree = Instantiate(prefab, brush.transform);
            }

            tree.transform.position = spawnPos;

            // Масштаб
            float scale = Random.Range(brush.minScale, brush.maxScale);
            tree.transform.localScale = Vector3.one * scale;

            // Поворот по нормали поверхности
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            if (brush.randomRotationY)
            {
                rot *= Quaternion.Euler(0, Random.Range(0f, brush.rotationYVariance), 0);
            }
            tree.transform.rotation = rot;
        }
    }

    // Добавляем кнопку Clear в инспектор
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Инструменты", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🗑️ Clear All Trees", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Подтверждение", 
                "Удалить все деревья, созданные этой кистью?", 
                "Да", "Отмена"))
            {
                brush.ClearAllTrees();
            }
        }
        EditorGUILayout.EndVertical();
    }
}