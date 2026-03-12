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
        if (brush == null || brush.targetMesh == null)
            return;

        // Рисуем круг кисти
        Vector3 brushPos = GetBrushPosition();
        if (brushPos != Vector3.zero)
        {
            Handles.color = brush.brushColor;
            Handles.DrawSolidDisc(brushPos, Vector3.up, brush.brushSize);
            
            // Рисуем границу круга
            Handles.color = Color.green;
            Handles.DrawWireDisc(brushPos, Vector3.up, brush.brushSize);
        }

        // Получаем событие
        Event e = Event.current;

        // Проверяем, нажата ли левая кнопка мыши
        if (e.button == 0)
        {
            if (e.type == EventType.MouseDown)
            {
                isPainting = true;
                e.Use(); // ⚠️ ВАЖНО: Забираем событие у Unity, чтобы не сбрасывалось выделение
            }
            else if (e.type == EventType.MouseUp)
            {
                isPainting = false;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && isPainting)
            {
                PaintTrees(brushPos);
                e.Use(); // ⚠️ ВАЖНО: Чтобы Unity не двигал камеру и не снимал выделение
            }
        }
    }

    private Vector3 GetBrushPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.transform.GetComponent<MeshFilter>() == brush.targetMesh)
            {
                return hit.point;
            }
        }
        return Vector3.zero;
    }

    private void PaintTrees(Vector3 centerPos)
    {
        if (centerPos == Vector3.zero) return;
        if (brush.treePrefabs == null || brush.treePrefabs.Length == 0) return;

        for (int i = 0; i < brush.density; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * brush.brushSize;
            Vector3 testPos = centerPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            Ray ray = new Ray(testPos + Vector3.up * 5f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f))
            {
                if (hit.transform.GetComponent<MeshFilter>() != brush.targetMesh)
                    continue;

                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle > brush.maxSlopeAngle)
                    continue;

                GameObject tree = Instantiate(
                    brush.treePrefabs[Random.Range(0, brush.treePrefabs.Length)],
                    hit.point,
                    Quaternion.identity,
                    brush.transform
                );

                float scale = Random.Range(brush.minScale, brush.maxScale);
                tree.transform.localScale = Vector3.one * scale;

                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                if (brush.randomRotationY)
                {
                    rot *= Quaternion.Euler(0, Random.Range(0f, brush.rotationYVariance), 0);
                }
                tree.transform.rotation = rot;
            }
        }
    }
}