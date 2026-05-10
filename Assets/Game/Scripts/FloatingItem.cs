using UnityEngine;

/// <summary>
/// Плавное парение и вращение предмета.
/// Работает корректно как с Rigidbody, так и без него.
/// </summary>
public class FloatingItem : MonoBehaviour
{
    [Header("Вращение")]
    [Tooltip("Скорость вращения в градусах в секунду")]
    public float rotationSpeed = 90f;

    [Tooltip("Ось вращения (по умолчанию — вокруг Y)")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Парение")]
    [Tooltip("Высота подъёма и опускания")]
    public float bobHeight = 0.15f;

    [Tooltip("Скорость парения (циклов в секунду)")]
    public float bobSpeed = 1.2f;

    [Tooltip("Случайный сдвиг фазы — чтобы несколько предметов не двигались синхронно")]
    public bool randomPhaseOffset = true;

    // ── приватные поля ──────────────────────────────────────────────────────
    private Vector3 _startPosition;
    private float _phaseOffset;
    private Rigidbody _rb;

    void Start()
    {
        _startPosition = transform.position;
        _phaseOffset = randomPhaseOffset ? Random.Range(0f, Mathf.PI * 2f) : 0f;

        _rb = GetComponent<Rigidbody>();

        if (_rb != null)
        {
            // Отключаем физику — управляем позицией вручную
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.interpolation = RigidbodyInterpolation.Interpolate; // убирает дёрганье
        }
    }

    void FixedUpdate()
    {
        if (_rb != null)
            ApplyFloating();
    }

    void Update()
    {
        if (_rb == null)
            ApplyFloating();
    }

    void ApplyFloating()
    {
        // Вращение через Quaternion — без накопления ошибок и скачков
        Quaternion deltaRotation = Quaternion.AngleAxis(
            rotationSpeed * Time.deltaTime,
            rotationAxis.normalized
        );

        // Парение по синусоиде
        float newY = _startPosition.y
                   + Mathf.Sin(Time.time * bobSpeed * Mathf.PI * 2f + _phaseOffset)
                   * bobHeight;

        Vector3 newPosition = new Vector3(_startPosition.x, newY, _startPosition.z);

        if (_rb != null)
        {
            _rb.MoveRotation(_rb.rotation * deltaRotation);
            _rb.MovePosition(newPosition);
        }
        else
        {
            transform.rotation *= deltaRotation;
            transform.position = newPosition;
        }
    }
}