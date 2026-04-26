using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private GameObject enemyTemplate;  // Орк со сцены (шаблон)
    [SerializeField] private Transform playerTransform;

    [Header("Зоны (метры)")]
    [SerializeField] private float triggerRadius = 20f;  // Игрок должен подойти на это расстояние
    [SerializeField] private float spawnRadius = 10f;    // Случайная точка в этой зоне

    [Header("Настройки")]
    [SerializeField] private int maxEnemies = 5;         // Макс. количество живых врагов
    [SerializeField] private LayerMask enemyLayer;       // Слой Enemy
    [SerializeField] private float spawnDelay = 5f;      // Задержка между спавнами
    [SerializeField] private float checkInterval = 0.5f; // Частота проверок

    private bool isOnCooldown = false;
    private Collider[] overlapBuffer = new Collider[20];

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            else Debug.LogWarning("[EnemySpawner] Игрок с тегом 'Player' не найден!");
        }

        // Скрываем шаблон — он только для копирования
        if (enemyTemplate != null)
            enemyTemplate.SetActive(false);
        else
            Debug.LogWarning("[EnemySpawner] Enemy Template не назначен!");

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            bool playerNear = playerTransform != null &&
                              Vector3.Distance(transform.position, playerTransform.position) <= triggerRadius;

            int currentCount = CountLiveEnemies();
            bool canSpawn = playerNear && currentCount < maxEnemies && !isOnCooldown;

            if (canSpawn)
            {
                SpawnEnemy();
                isOnCooldown = true;
                yield return new WaitForSeconds(spawnDelay);
                isOnCooldown = false;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private int CountLiveEnemies()
    {
        // Считаем живых врагов в зоне triggerRadius по слою Enemy
        int hits = Physics.OverlapSphereNonAlloc(transform.position, triggerRadius, overlapBuffer, enemyLayer);
        return hits;
    }

    private void SpawnEnemy()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 targetPos = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

        Vector3 spawnPos;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            spawnPos = transform.position;
            Debug.LogWarning("[EnemySpawner] Точка на NavMesh не найдена, спавн в центре.");
        }

        GameObject enemy = Instantiate(enemyTemplate, spawnPos, Quaternion.identity);
        enemy.SetActive(true);
    }

    // Gizmos видны ВСЕГДА (не только при выделении)
    private void OnDrawGizmos()
    {
        // Зона активации — жёлтая
        Gizmos.color = new Color(1f, 1f, 0f, 0.08f);
        Gizmos.DrawSphere(transform.position, triggerRadius);
        Gizmos.color = new Color(1f, 1f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        // Зона спавна — синяя
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.15f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
        Gizmos.color = new Color(0f, 0.5f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}