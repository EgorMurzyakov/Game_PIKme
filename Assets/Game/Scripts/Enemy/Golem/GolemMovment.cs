using UnityEngine;
using UnityEngine.AI;

public class GolemMovment : MonoBehaviour
{
    [SerializeField] private Transform target;   
    [SerializeField] private float speed = 3;
    private float currentSpeed;
    private bool canGo = true;
    private bool canRotate = true;
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
        if (_st != state.Death)
        {

            switch (_st)
            {
                case state.Idle:
                    //currentSpeed = 0f;
                    canGo = false;
                    canRotate = false;
                    break;

                case state.Walk:
                    canGo = true;
                    canRotate = true;
                    currentSpeed = speed;
                    break;

                case state.Attack:
                    canGo = false;
                    canRotate = true;                    
                    break;

                case state.AttackB:
                    canGo = false;
                    canRotate = true;
                    break;

                case state.Action:
                    canGo = false;
                    canRotate = false;
                    break;

                case state.Damage:
                    canGo = false;
                    canRotate = false;
                    break;

                case state.Death:
                    canGo = false;
                    canRotate = false;
                    break;
            }

            if (canRotate)
            {
                // 1. Обновляем цель агента
                agent.SetDestination(target.position);
            }

            // 2. Получаем направление от агента
            Vector3 direction = agent.desiredVelocity;

            // 3. Нормализуем и применяем через CharacterController
            if (direction.magnitude > 0.1f && canGo)
            {
                direction.Normalize();

                // Движение
                charControl.Move(direction * currentSpeed * Time.deltaTime);
            }

        }
    }
}
