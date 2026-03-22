using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FenceBrushData))]
public class FenceBrushEditor : Editor
{
    FenceBrushData brush;
    private bool isDrawing = false;
    
    private Vector3 previewPos;
    private Quaternion previewRot;
    private bool hasPreview;

    void OnEnable()
    {
        brush = (FenceBrushData)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        if (!IsValid()) return;

        Vector3 cursorPos = GetCursorSurfacePosition();
        
        if (brush.showPreview && cursorPos != Vector3.zero)
        {
            UpdatePreview(cursorPos);
            DrawPreview();
        }

        HandleInput(cursorPos);
    }

    private bool IsValid()
    {
        if (brush == null) return false;
        if (brush.fencePrefabs == null || brush.fencePrefabs.Length == 0)
        {
            EditorGUILayout.HelpBox("⚠️ Назначь Fence Prefabs!", MessageType.Warning);
            return false;
        }
        return true;
    }

    private Vector3 GetCursorSurfacePosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 10000f, brush.surfaceLayers))
        {
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

    // === ИСПРАВЛЕННАЯ ФУНКЦИЯ: Теперь точно работает для Mesh ===
    private Vector3 GetSurfacePoint(Vector3 worldPos, out Vector3 normal)
    {
        normal = Vector3.up;
        
        // === РЕЖИМ TERRAIN ===
        if (brush.heightMode == FenceBrushData.HeightMode.Terrain || 
            (brush.heightMode == FenceBrushData.HeightMode.Auto && Terrain.activeTerrain != null))
        {
            Terrain terrain = brush.targetTerrain != null ? brush.targetTerrain : Terrain.activeTerrain;
            
            if (terrain != null && terrain.terrainData != null)
            {
                Vector3 terrainPos = worldPos - terrain.transform.position;
                TerrainData data = terrain.terrainData;
                
                if (terrainPos.x >= 0 && terrainPos.x <= data.size.x &&
                    terrainPos.z >= 0 && terrainPos.z <= data.size.z)
                {
                    float height = terrain.SampleHeight(worldPos);
                    normal = data.GetInterpolatedNormal(terrainPos.x / data.size.x, terrainPos.z / data.size.z);
                    return new Vector3(worldPos.x, height, worldPos.z);
                }
            }
        }
        
        // === РЕЖИМ MESH (Raycast) ===
        Ray ray = new Ray(worldPos + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, brush.surfaceLayers))
        {
            normal = hit.normal;
            return hit.point;
        }
        
        // Если ничего не нашли — возвращаем исходную позицию
        return worldPos;
    }

    private void UpdatePreview(Vector3 cursorPos)
    {
        if (!isDrawing || brush.lastPlacePos == Vector3.zero)
        {
            Vector3 normal;
            previewPos = GetSurfacePoint(cursorPos, out normal);
            previewPos += Vector3.up * brush.heightOffset;
            
            Vector3 camForward = SceneView.lastActiveSceneView.camera.transform.forward;
            camForward.y = 0;
            if (camForward.magnitude > 0.1f)
            {
                previewRot = Quaternion.LookRotation(camForward, Vector3.up) * Quaternion.Euler(0, brush.rotationOffset, 0);
            }
            else
            {
                previewRot = Quaternion.LookRotation(Vector3.forward, Vector3.up) * Quaternion.Euler(0, brush.rotationOffset, 0);
            }
            
            hasPreview = true;
            return;
        }

        Vector3 direction = cursorPos - brush.lastPlacePos;
        direction.y = 0;
        
        if (direction.magnitude >= brush.spacing)
        {
            Vector3 nextPos = brush.lastPlacePos + direction.normalized * brush.spacing;
            
            Vector3 normal;
            previewPos = GetSurfacePoint(nextPos, out normal);
            previewPos += Vector3.up * brush.heightOffset;
            
            previewRot = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, brush.rotationOffset, 0);
            
            if (brush.alignToSurface)
            {
                previewRot = Quaternion.FromToRotation(Vector3.up, normal) * previewRot;
            }
            
            hasPreview = true;
        }
        else
        {
            hasPreview = false;
        }
    }

    private void DrawPreview()
    {
        if (!hasPreview) return;

        Handles.color = brush.previewColor;
        
        float halfWidth = 0.5f;
        Vector3 right = previewRot * Vector3.right * halfWidth;
        Vector3 forward = previewRot * Vector3.forward * brush.spacing * 0.9f;
        
        Vector3[] corners = new Vector3[4];
        corners[0] = previewPos - right - forward;
        corners[1] = previewPos + right - forward;
        corners[2] = previewPos + right + forward;
        corners[3] = previewPos - right + forward;
        
        Handles.DrawLine(corners[0], corners[1]);
        Handles.DrawLine(corners[1], corners[2]);
        Handles.DrawLine(corners[2], corners[3]);
        Handles.DrawLine(corners[3], corners[0]);
        
        Handles.ArrowHandleCap(0, previewPos, previewRot, brush.spacing * 0.5f, EventType.Repaint);
        Handles.DrawSolidDisc(previewPos, Vector3.up, 0.1f);
    }

    private void HandleInput(Vector3 cursorPos)
    {
        Event e = Event.current;

        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                if (cursorPos != Vector3.zero && IsValidPosition(cursorPos))
                {
                    isDrawing = true;
                    brush.lastPlacePos = cursorPos;
                    brush.canPlace = true;
                    
                    if (brush.clearOnNewStroke)
                    {
                        brush.ClearAllFences();
                    }
                    
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseUp)
            {
                if (isDrawing)
                {
                    isDrawing = false;
                    brush.lastPlacePos = Vector3.zero;
                    e.Use();
                }
            }
            else if (e.type == EventType.MouseDrag && isDrawing)
            {
                if (cursorPos != Vector3.zero)
                {
                    TryPlaceFence(cursorPos);
                    e.Use();
                }
            }
        }
        
        if (e.button == 1 && e.type == EventType.MouseDown)
        {
            isDrawing = false;
            brush.lastPlacePos = Vector3.zero;
            e.Use();
        }
    }

    private bool IsValidPosition(Vector3 pos)
    {
        Vector3 normal;
        GetSurfacePoint(pos, out normal);
        
        float angle = Vector3.Angle(Vector3.up, normal);
        if (angle > brush.maxSlopeAngle)
            return false;
        
        return true;
    }

    private void TryPlaceFence(Vector3 cursorPos)
    {
        if (!brush.canPlace) return;
        
        float distance = Vector3.Distance(
            new Vector3(cursorPos.x, 0, cursorPos.z),
            new Vector3(brush.lastPlacePos.x, 0, brush.lastPlacePos.z)
        );
        
        if (distance < brush.spacing)
            return;
        
        Vector3 direction = cursorPos - brush.lastPlacePos;
        direction.y = 0;
        if (direction.magnitude < 0.1f) return;
        
        Vector3 nextPos = brush.lastPlacePos + direction.normalized * brush.spacing;
        
        // === КЛЮЧЕВОЙ МОМЕНТ: Получаем высоту для каждой секции ===
        Vector3 normal;
        Vector3 surfacePos = GetSurfacePoint(nextPos, out normal);
        
        float angle = Vector3.Angle(Vector3.up, normal);
        if (angle > brush.maxSlopeAngle)
            return;
        
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, brush.rotationOffset, 0);
        
        if (brush.alignToSurface)
        {
            rotation = Quaternion.FromToRotation(Vector3.up, normal) * rotation;
        }
        
        GameObject prefab = brush.fencePrefabs[Random.Range(0, brush.fencePrefabs.Length)];
        GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(prefab, brush.transform);
        if (fence == null) fence = Instantiate(prefab, brush.transform);
        
        fence.transform.position = surfacePos + Vector3.up * brush.heightOffset;
        fence.transform.rotation = rotation;
        
        if (brush.randomRotationVariance > 0)
        {
            float randomY = Random.Range(-brush.randomRotationVariance, brush.randomRotationVariance);
            fence.transform.rotation *= Quaternion.Euler(0, randomY, 0);
        }
        
        if (brush.randomScale.x != brush.randomScale.y)
        {
            float scale = Random.Range(brush.randomScale.x, brush.randomScale.y);
            fence.transform.localScale = Vector3.one * scale;
        }
        
        // === ВАЖНО: Сохраняем позицию поверхности (не с heightOffset) ===
        brush.lastPlacePos = surfacePos;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("🛠 Инструменты", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🗑️ Clear All Fences", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Подтверждение", 
                "Удалить все заборы?", "Да", "Отмена"))
            {
                brush.ClearAllFences();
            }
        }
        EditorGUILayout.EndVertical();

        // Подсказка по режиму
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("InfoBox");
        
        if (brush.heightMode == FenceBrushData.HeightMode.Mesh)
        {
            EditorGUILayout.LabelField("🔧 Режим: MESH", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Убедись, что на земле есть Mesh Collider!", EditorStyles.miniLabel);
        }
        else if (brush.heightMode == FenceBrushData.HeightMode.Terrain)
        {
            EditorGUILayout.LabelField("🏔️ Режим: TERRAIN", EditorStyles.boldLabel);
        }
        else
        {
            EditorGUILayout.LabelField("🔍 Режим: AUTO (определяет сам)", EditorStyles.boldLabel);
        }
        
        EditorGUILayout.LabelField("• ЛКМ + драг: рисовать забор", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
    }
}