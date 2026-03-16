using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Автоматически размещает объект на запечённом NavMesh при старте.
/// Полезно, если карта на нестандартной высоте.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAutoPlace : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Максимальная дистанция поиска земли вниз")]
    public float maxDropDistance = 50f;
    
    [Tooltip("Смещение по Y после размещения (чтобы не проваливался)")]
    public float heightOffset = 0.1f;

    [Header("Статус")]
    public bool isPlaced = false;

    void Start()
    {
        PlaceOnNavMesh();
    }

    void PlaceOnNavMesh()
    {
        var agent = GetComponent<NavMeshAgent>();
        
        // Если агент уже на навмеше — ничего не делаем
        if (agent != null && agent.isOnNavMesh)
        {
            isPlaced = true;
            return;
        }

        Debug.Log($"[{name}] 🔍 Ищу NavMesh для размещения...");

        // Пускаем луч вниз, чтобы найти поверхность
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDropDistance))
        {
            Debug.Log($"[{name}] 🎯 Поверхность найдена: {hit.point}");
            
            // Проверяем, есть ли тут запечённый NavMesh
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                // Перемещаем объект на навмеш
                transform.position = navHit.position + Vector3.up * heightOffset;
                
                Debug.Log($"[{name}] ✅ Размещён на NavMesh: {transform.position}");
                isPlaced = true;
                
                // Если есть PatrolNPC — даём ему знать, что можно запускаться
                var patrol = GetComponent<PatrolNPC>();
                if (patrol != null)
                {
                    // Можно добавить публичный метод Restart() в PatrolNPC при необходимости
                }
            }
            else
            {
                Debug.LogWarning($"[{name}] ⚠️ Поверхность найдена, но NavMesh рядом нет!");
                Debug.LogWarning($"  💡 Проверьте: 1) запечён ли навмеш, 2) совпадает ли Agent Type");
            }
        }
        else
        {
            Debug.LogError($"[{name}] ❌ Земля не найдена в радиусе {maxDropDistance}м!");
            Debug.LogError($"  💡 Убедитесь, что под объектом есть меш с коллайдером");
        }
    }

    // Отладка: рисуем луч в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * maxDropDistance);
        Gizmos.DrawSphere(transform.position + Vector3.down * maxDropDistance, 0.2f);
    }
}