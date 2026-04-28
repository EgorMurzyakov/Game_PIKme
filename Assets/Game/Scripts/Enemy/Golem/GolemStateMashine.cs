using UnityEngine;

public class GolemStateMashine : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private GolemMovment movment;
    private GolemAnimation anim;
    private ColliderSwitch colliderSwitch;

    private state currentState = state.Idle;
    private state prevState = state.Empty;

    [SerializeField] private bool isActiv;
    [SerializeField] private float attackRange = 1f; // Дистанция атаки
    [SerializeField] private float attackCooldown = 1f; // Задержка атак
    private float startAttckColdown;
    private bool canAttack = true;    

    private bool canChangeState = false;
    private bool takingDamage = false;
    private bool death = false;
    private float distanceToPlayer;

    private bool visualLoss = false; 

    public void Start()
    {        
        movment = GetComponent<GolemMovment>();
        anim = GetComponent<GolemAnimation>();
        colliderSwitch = GetComponent<ColliderSwitch>();
    }

    public void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (isActiv)
        {
            UpdateState();            
        }

        movment.GetMoving(currentState);
        if (currentState != prevState)
        {
            anim.GetAnimation(currentState);
            colliderSwitch.ChoosingAction(currentState);
            Debug.Log(currentState);
            prevState = currentState;
        }
    }

    private void UpdateState()
    {
        canAttack = (Time.time - attackCooldown) > startAttckColdown;

        if (death)
        {
            currentState = state.Death;
        }
        else if (takingDamage) // Самое приоритетное (нет)
        {
            currentState = state.Damage;
            prevState = state.Empty;
        }

        switch (currentState)
        {
            case state.Idle:
                if (distanceToPlayer < attackRange && canAttack)
                {
                    currentState = state.Attack;
                    startAttckColdown = Time.time;
                }
                else if (visualLoss)
                {
                    visualLoss = false;
                    currentState = state.Action;
                }
                else
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (distanceToPlayer < attackRange && canAttack)
                {
                    currentState = state.Attack;
                    startAttckColdown = Time.time;
                }
                else if (visualLoss)
                {
                    visualLoss = false;
                    currentState = state.Action;
                }
                break;

            case state.Attack:
                if (canChangeState)
                {
                    if (distanceToPlayer < attackRange && canAttack)
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                        startAttckColdown = Time.time;
                    }
                    else if (visualLoss)
                    {
                        visualLoss = false;
                        currentState = state.Action;
                    }
                    else
                    {
                        currentState = state.Walk;
                    }

                    canChangeState = false;
                }
                break;

            case state.Action:
                if (canChangeState)
                {
                    if (distanceToPlayer < attackRange && canAttack)
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                        startAttckColdown = Time.time;
                    }
                    else
                    {
                        currentState = state.Walk;
                    }

                    canChangeState = false;
                }
                break;

            case state.Damage:
                if (canChangeState)
                {
                    if (distanceToPlayer < attackRange && canAttack)
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                        startAttckColdown = Time.time;
                    }
                    else
                    {
                        currentState = state.Walk;
                    }

                    canChangeState = false;
                }
                break;

            case state.Death:
                break;
        }
    }

    public void EndChangeState() // Вызывается из анимационных событий, canChangeState -> true -> можем сменить состояние 
    {
        canChangeState = true;
        takingDamage = false;
    }

    public void GoDamageState() // Получаем урон - true, не получаем - false
    {
        takingDamage = true;
    }

    public void GoDeathState()
    {
        death = true;
    }

    public void SetVisualLoss()
    {
        visualLoss = true;
    }
}
