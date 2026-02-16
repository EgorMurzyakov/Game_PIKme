using Unity.VisualScripting;
using UnityEngine;


public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private GameObject player;   
    private EnemyMovment movment;
    private EnemyAnimation anim;
    private ColliderSwitch colliderSwitch;

    private state currentState = state.Idle;
    private state prevState = state.Idle;

    [SerializeField] private float rayRange = 10f;
    [SerializeField] private float attackRange = 1f;
    private float rayThickness = 0.1f; // Толщина луча (для визуализации)
    private Vector3 popopo;
    private RaycastHit hit;
    private bool inVisibilityArea;
    private bool canChangeState = false;
    private float distanceToPlayer;

    public void Start()
    {
        popopo = new Vector3 (0, 1.3f, 0);
        movment = GetComponent<EnemyMovment>();
        anim = GetComponent<EnemyAnimation>();
        colliderSwitch = GetComponent<ColliderSwitch>();
    }

    public void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        RunRayCast(); // Рейкасты, меняет inVisibilityArea

        UpdateState();

        movment.GetMoving(currentState);
        if (currentState != prevState)
        {
            anim.GetAnimation(currentState);
            colliderSwitch.ChoosingAction(currentState);
            Debug.Log(currentState);
            prevState = currentState;
        }
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
            case state.Idle:
                if (inVisibilityArea && distanceToPlayer < attackRange)
                {
                    currentState = state.Attack;
                }
                else if (inVisibilityArea)
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (inVisibilityArea && distanceToPlayer < attackRange)
                {
                    currentState = state.Attack;
                }
                else if (inVisibilityArea == false)
                {
                    currentState = state.Idle;
                }
                break;

            case state.Attack:
                if (canChangeState)
                {
                    if (inVisibilityArea && distanceToPlayer < attackRange)
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                    }
                    else if (inVisibilityArea)
                    {
                        currentState = state.Walk;
                    }
                    else
                    {
                        currentState = state.Idle;
                    }

                    canChangeState = false;
                }
                break;

            case state.Action:
                break;

            case state.Damage:
                break;

            case state.Death:
                break;
        }
    }

    public void EndChangeState()
    {
        canChangeState = true;
        Debug.Log("Вызвалось - " + canChangeState);
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

