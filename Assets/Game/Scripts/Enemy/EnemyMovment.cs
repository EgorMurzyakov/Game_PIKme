using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float attackDistance;
    [SerializeField] private float speed = 3;
    private float currentSpeed;
    private CharacterController charControl;
    private NavMeshAgent agent;   

    public void Start()
    {
        charControl = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();        

        currentSpeed = speed;
    }

    public void GetMoving(state _st)
    {

        switch (_st)
        {
            case state.Idle:
                currentSpeed = 0f;
                break;

            case state.Walk:
                currentSpeed = speed;
                break;

            case state.Attack:
                break;

            case state.Action:
                break;

            case state.Damage:
                break;

            case state.Death:
                break;
        }


        // 1. Обновляем цель агента
        agent.SetDestination(target.position);

        // 2. Получаем направление от агента
        Vector3 direction = agent.desiredVelocity;

        // 3. Нормализуем и применяем через CharacterController
        if (direction.magnitude > 0.1f)
        {
            direction.Normalize();

            // Движение
            charControl.Move(direction * currentSpeed * Time.deltaTime);
        }

    }
}