using UnityEngine;

public class MeshTreeScatterer : MonoBehaviour
{
    [Header("Настройки Меши")]
    public MeshFilter targetMesh;
    public bool alignToNormal = true;

    [Header("Настройки Деревьев")]
    public GameObject[] treePrefabs;
    public int count = 100;

    [Header("Случайный масштаб")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Случайный поворот")]
    public bool randomRotationY = true;
    public float rotationYVariance = 360f;

    [Header("Ограничения")]
    [Range(0, 90)]
    public float maxSlopeAngle = 30f; // Максимальный угол склона (горы будут игнорироваться)

    [ContextMenu("Generate Trees")]
    public void GenerateTrees()
    {
        if (targetMesh == null)
        {
            Debug.LogError("Не назначен targetMesh!");
            return;
        }

        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogError("Не назначены префабы деревьев!");
            return;
        }

        Mesh mesh = targetMesh.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;

        if (normals == null || normals.Length == 0)
        {
            normals = new Vector3[vertices.Length];
            for (int i = 0; i < normals.Length; i++)
                normals[i] = Vector3.up;
        }

        // Очистка старых деревьев
        ClearTrees();

        int spawnedCount = 0;
        int maxAttempts = count * 10; // Максимум попыток чтобы избежать бесконечного цикла
        int attempts = 0;

        while (spawnedCount < count && attempts < maxAttempts)
        {
            attempts++;

            // Выбираем случайный треугольник
            int randomTri = Random.Range(0, triangles.Length / 3) * 3;

            Vector3 a = vertices[triangles[randomTri]];
            Vector3 b = vertices[triangles[randomTri + 1]];
            Vector3 c = vertices[triangles[randomTri + 2]];

            Vector3 normalA = normals[triangles[randomTri]];
            Vector3 normalB = normals[triangles[randomTri + 1]];
            Vector3 normalC = normals[triangles[randomTri + 2]];

            // Случайная точка внутри треугольника
            float r1 = Random.value;
            float r2 = Random.value;
            float sqrtR1 = Mathf.Sqrt(r1);

            float x = 1 - sqrtR1;
            float y = sqrtR1 * (1 - r2);
            float z = sqrtR1 * r2;

            Vector3 localPos = (x * a) + (y * b) + (z * c);
            Vector3 localNormal = (x * normalA) + (y * normalB) + (z * normalC);
            localNormal.Normalize();

            // === ПРОВЕРКА УГЛА НАКЛОНА ===
            float angle = Vector3.Angle(Vector3.up, localNormal);
            if (angle > maxSlopeAngle)
            {
                continue; // Пропускаем этот треугольник (слишком крутой склон)
            }
            // =============================

            Vector3 worldPos = targetMesh.transform.TransformPoint(localPos);
            Quaternion worldRot = targetMesh.transform.rotation;

            // Создаём дерево
            GameObject tree = Instantiate(
                treePrefabs[Random.Range(0, treePrefabs.Length)],
                worldPos,
                Quaternion.identity,
                transform
            );

            // Масштаб
            float scale = Random.Range(minScale, maxScale);
            tree.transform.localScale = Vector3.one * scale;

            // Поворот
            Quaternion finalRotation = Quaternion.identity;

            if (alignToNormal)
            {
                finalRotation = Quaternion.FromToRotation(Vector3.up, worldRot * localNormal);
            }

            finalRotation = worldRot * finalRotation;

            if (randomRotationY)
            {
                float randomY = Random.Range(0f, rotationYVariance);
                finalRotation *= Quaternion.Euler(0, randomY, 0);
            }

            tree.transform.rotation = finalRotation;
            spawnedCount++;
        }

        Debug.Log($"Создано {spawnedCount} деревьев (попыток: {attempts})");
    }

    [ContextMenu("Clear Trees")]
    public void ClearTrees()
    {
        // Удаляем все дочерние объекты
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
}