using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Скрипт патрулирования для NPC.
/// Заставляет персонажа ходить по кругу в заданном радиусе от стартовой точки.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PatrolNPC : MonoBehaviour
{
    [Header("⚙️ Настройки патрулирования")]
    [Tooltip("Скорость передвижения")]
    public float moveSpeed = 1.5f;
    
    [Tooltip("Радиус зоны патрулирования от стартовой точки")]
    public float patrolRadius = 10f;
    
    [Tooltip("Время паузы в точке (секунды)")]
    public float waitTime = 2f;
    
    [Tooltip("Дистанция, на которой считается что НПЦ дошёл")]
    public float stopDistance = 0.5f;
    
    [Tooltip("Минимальная дистанция между точками (чтобы не ходил на месте)")]
    public float minDistanceBetweenPoints = 2f;

    [Header("🔗 Ссылки")]
    [SerializeField] private NavMeshAgent agent;
    
    // Приватные переменные
    private Vector3 startPosition;
    private Vector3 targetPoint;
    private float waitTimer;
    private bool isWaiting;
    private bool isInitialized;

    // ============================================================
    // UNITY METHODS
    // ============================================================
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!isInitialized) return;
        
        if (!agent.isOnNavMesh || !agent.enabled)
        {
            Debug.LogWarning($"[{name}] Потерян NavMesh! Остановка патрулирования.");
            isInitialized = false;
            Invoke(nameof(Initialize), 1f);
            return;
        }

        PatrolLogic();
    }

    // Отладка: рисуем зону патруля в редакторе
    void OnDrawGizmosSelected()
    {
        // Стартовая точка (зелёная)
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);
        
        // Радиус патрулирования (жёлтый круг)
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        // Целевая точка (красная)
        if (isInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPoint, 0.2f);
        }
    }

    // ============================================================
    // MAIN LOGIC
    // ============================================================

    void Initialize()
    {
        if (agent == null)
        {
            Debug.LogError($"[{name}] ❌ NavMeshAgent не найден! Добавьте компонент.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"[{name}] ⚠️ Агент ещё не на NavMesh. Повторная попытка через 0.5с...");
            Invoke(nameof(Initialize), 0.5f);
            return;
        }

        // Настройки агента
        agent.speed = moveSpeed;  // ← ИСПОЛЬЗУЕМ НАШУ ПЕРЕМЕННУЮ!
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;

        startPosition = transform.position;
        isInitialized = true;
        
        Debug.Log($"[{name}] ✅ Патрулирование запущено! Скорость: {moveSpeed}");
        
        SetNewDestination();
    }

    void PatrolLogic()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewDestination();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance <= stopDistance)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }
        }
    }

    void SetNewDestination()
    {
        if (!isInitialized) return;

        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        targetPoint = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        if (NavMesh.SamplePosition(targetPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            targetPoint = hit.position;
            agent.SetDestination(targetPoint);
        }
        else
        {
            Debug.LogWarning($"[{name}] ⚠️ Не удалось найти точку на NavMesh!");
            Invoke(nameof(SetNewDestination), 1f);
        }
    }

    // ============================================================
    // PUBLIC METHODS
    // ============================================================

    public void StopPatrol()
    {
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        isInitialized = false;
    }

    public void ResumePatrol()
    {
        if (agent != null)
        {
            agent.isStopped = false;
            if (!isInitialized)
            {
                Initialize();
            }
        }
    }

    public void SetStartPosition(Vector3 newPosition)
    {
        startPosition = newPosition;
        startPosition.y = transform.position.y;
        if (isInitialized)
        {
            SetNewDestination();
        }
    }
}