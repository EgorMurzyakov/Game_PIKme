using UnityEngine;

// === ENUM ВЫНЕСЕН ЗА ПРЕДЕЛЫ КЛАССА ===
public class FenceBrushData : MonoBehaviour
{
    public enum HeightMode { Auto, Terrain, Mesh }
    // ====================================

    [Header("🚧 Настройки Забора")]
    public GameObject[] fencePrefabs;
    public float spacing = 2f;
    public bool autoRotate = true;
    public bool alignToSurface = true;
    
    [Header("🏔️ Режим определения высоты")]
    [Tooltip("Terrain = для Unity Terrain, Mesh = для обычных 3D моделей")]
    public HeightMode heightMode = HeightMode.Auto;
    
    public Terrain targetTerrain;
    
    [Header("⚙️ Дополнительные настройки")]
    [Tooltip("Поворот префаба вокруг Y (например, 90 если забор смотрит боком)")]
    public float rotationOffset = 90f;
    
    [Tooltip("Смещение по высоте (если забор летает - ставь отрицательное)")]
    public float heightOffset = 0f;
    
    public float randomRotationVariance = 0f;
    public Vector2 randomScale = new Vector2(1f, 1f);
    
    [Header("⚠️ Ограничения")]
    [Range(0, 90)]
    public float maxSlopeAngle = 60f;
    
    [Header("🎨 Отображение")]
    public Color previewColor = new Color(1, 0.5f, 0, 0.5f);
    public bool showPreview = true;
    
    [Header("🔍 Слои")]
    public LayerMask surfaceLayers = ~0;
    public string[] ignoreLayerNames = new string[] { "UI", "Player" };
    
    [Header("🧹 Очистка")]
    public bool clearOnNewStroke = false;
    
    [System.NonSerialized]
    public Vector3 lastPlacePos;
    [System.NonSerialized]
    public bool canPlace = true;
    
    public void ClearAllFences()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(transform.GetChild(i).gameObject);
            else
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}