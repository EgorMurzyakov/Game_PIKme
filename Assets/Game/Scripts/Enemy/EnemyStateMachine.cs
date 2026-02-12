using UnityEngine;

public enum enemyState { Idle, Walk, Attack, Action, Damage, Death }

public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private EnemyMovment movment;
    private EnemyAnimation anim;

    private enemyState currentState = enemyState.Idle;

    [SerializeField] private float rayRange = 10f;
    private float rayThickness = 0.1f; // Толщина луча (для визуализации)
    private Vector3 popopo;
    private RaycastHit hit;
    private bool inVisibilityArea;

    public void Start()
    {
        popopo = new Vector3 (0, 1.3f, 0);
        movment = GetComponent<EnemyMovment>();
        anim = GetComponent<EnemyAnimation>();
    }

    public void Update()
    {
        RunRayCast();
        UpdateState();
    }

    private void RunRayCast() // Запуск рейкастов для определения нахождения в зоне видимости
    {
        Ray ray = new Ray(transform.position + popopo, (player.transform.position - transform.position).normalized);

        if (Physics.Raycast(ray, out hit, rayRange) && hit.collider.CompareTag("Player"))
        {
            // Попали - рисуем жирную зеленую линию
            DrawThickRay(ray.origin, hit.point, Color.green, rayThickness);
            inVisibilityArea = true;
        }
        else
        {
            // Не попали - жирная красная
            DrawThickRay(ray.origin, ray.origin + ray.direction * rayRange, Color.red, rayThickness);
            inVisibilityArea = false;
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case enemyState.Idle:
                break;

            case enemyState.Walk:
                break;

            case enemyState.Attack:
                break;

            case enemyState.Action:
                break;

            case enemyState.Damage:
                break;

            case enemyState.Death:
                break;
        }
    }

    void DrawThickRay(Vector3 start, Vector3 end, Color color, float thickness) // Метод для рисования толстых линий (визуализация RayCast)
    {
        // Рисуем несколько линий рядом для эффекта толщины
        Vector3 up = Vector3.up * thickness;
        Vector3 right = Vector3.right * thickness;

        Debug.DrawLine(start, end, color);
        Debug.DrawLine(start + up, end + up, color);
        Debug.DrawLine(start - up, end - up, color);
        Debug.DrawLine(start + right, end + right, color);
        Debug.DrawLine(start - right, end - right, color);

        // Для совсем толстых - еще диагонали
        if (thickness > 0.2f)
        {
            Debug.DrawLine(start + up + right, end + up + right, color);
            Debug.DrawLine(start + up - right, end + up - right, color);
            Debug.DrawLine(start - up + right, end - up + right, color);
            Debug.DrawLine(start - up - right, end - up - right, color);
        }
    }
}

