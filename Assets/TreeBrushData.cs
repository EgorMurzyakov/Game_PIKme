using UnityEngine;

public class TreeBrushData : MonoBehaviour
{
    [Header("Настройки Меши")]
    public MeshFilter targetMesh;

    [Header("Настройки Кисти")]
    public GameObject[] treePrefabs;
    public float brushSize = 5f;
    public int density = 5;

    [Header("Случайный масштаб")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Случайный поворот")]
    public bool randomRotationY = true;
    public float rotationYVariance = 360f;

    [Header("Ограничения")]
    [Range(0, 90)]
    public float maxSlopeAngle = 30f;

    [Header("Отображение")]
    public Color brushColor = new Color(0, 1, 0, 0.3f);
}