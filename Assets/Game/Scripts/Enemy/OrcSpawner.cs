using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class OrcSpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private GameObject orcPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("Зоны (метры)")]
    [SerializeField] private float triggerRadius = 20f;   // Игрок должен подойти на это расстояние
    [SerializeField] private float spawnRadius = 3f;      // Случайная точка в этой зоне
    [SerializeField] private float checkRadius = 10f;     // Не спавнить, если в этой зоне уже есть живой враг

    [Header("Настройки")]
    [SerializeField] private LayerMask enemyLayer;        // Выберите слой Enemy (Layer 7)
    [SerializeField] private float spawnDelay = 5f;       // Задержка перед следующим спавном
    [SerializeField] private float checkInterval = 0.5f;  // Частота проверок (сек)

    private bool isOnCooldown = false;
    private Collider[] overlapBuffer = new Collider[20];  // Буфер для оптимизации

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else Debug.LogWarning("[OrcSpawner] Игрок с тегом 'Player' не найден!");
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            bool playerNear = playerTransform != null &&
                              Vector3.Distance(transform.position, playerTransform.position) <= triggerRadius;

            bool areaClear = !IsLiveEnemyNearby();

            if (playerNear && areaClear && !isOnCooldown)
            {
                SpawnOrc();
                isOnCooldown = true;
                yield return new WaitForSeconds(spawnDelay);
                isOnCooldown = false;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private bool IsLiveEnemyNearby()
    {
        // Проверяем ТОЛЬКО слой Enemy. Трупы (слой DeadEnemy) автоматически игнорируются!
        int hits = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, overlapBuffer, enemyLayer);
        return hits > 0;
    }

    private void SpawnOrc()
    {
        // 1. Случайная точка в круге (высота не меняется)
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 targetPos = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

        // 2. Ищем ближайшую проходимую точку на NavMesh
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            Instantiate(orcPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Instantiate(orcPrefab, transform.position, Quaternion.identity);
            Debug.LogWarning("[OrcSpawner] Валидная точка на NavMesh не найдена. Спавн в центре.");
        }
    }

    //  Визуализация зон в Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.2f); // Жёлтая: зона активации игрока
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = new Color(0, 1, 0, 0.3f); // Зелёная: зона случайного спавна
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = new Color(1, 0, 0, 0.2f); // Красная: проверка живых врагов
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}