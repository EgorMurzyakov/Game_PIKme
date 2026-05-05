using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private GameObject player;   
    private EnemyMovment movment;
    private EnemyAnimation anim;
    private ColliderSwitch colliderSwitch;

    private state currentState = state.Idle;
    private state prevState = state.Idle;

    [SerializeField] private float rayRange = 10f; // Дистанция, с которой враг видит игрока
    [SerializeField] private float attackRange = 1f; // Дистанция атаки
    [SerializeField] private float attackCooldown = 1f; // Задержка атак
    private float startAttckColdown;
    private bool canAttack = true;

    private bool inVisibilityArea;

    private bool canChangeState = true;
    private bool takingDamage = false;
    private bool death = false;
    private float distanceToPlayer;

    public void Start()
    {
        movment = GetComponent<EnemyMovment>();
        anim = GetComponent<EnemyAnimation>();
        colliderSwitch = GetComponent<ColliderSwitch>();        
    }

    public void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        inVisibilityArea = distanceToPlayer < rayRange; // Упрощение заместо рейкастов

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

    private void UpdateState()
    {
        canAttack = (Time.time - attackCooldown) > startAttckColdown;

        if (death)
        {
            currentState = state.Death;
        }
        else if (takingDamage) 
        {
            currentState = state.Damage;
        }

        switch (currentState)
        {
            case state.Idle:
                if (inVisibilityArea && distanceToPlayer < attackRange && canAttack) // -> Attack
                {
                    currentState = state.Attack;
                    startAttckColdown = Time.time;
                }
                else if (inVisibilityArea) // -> Walk
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (inVisibilityArea && distanceToPlayer < attackRange && canAttack) // -> Attack
                {
                    currentState = state.Attack;
                    startAttckColdown = Time.time;
                }
                else if (inVisibilityArea == false) // -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Attack:
                if (canChangeState)
                {
                    if (inVisibilityArea && distanceToPlayer < attackRange && canAttack) // -> Attack
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                        startAttckColdown = Time.time;
                    }
                    else if (inVisibilityArea)
                    {
                        currentState = state.Walk; // -> Walk
                    }
                    else
                    {
                        currentState = state.Idle; // -> Idle
                    }

                    canChangeState = false;
                }
                break;

            case state.Action:
                break;

            case state.Damage:
                if (canChangeState)
                {
                    if (inVisibilityArea && distanceToPlayer < attackRange && canAttack) // -> Attack
                    {
                        currentState = state.Attack;
                        prevState = state.Empty;
                        startAttckColdown = Time.time;
                    }
                    else if (inVisibilityArea) // -> Walk
                    {
                        currentState = state.Walk;
                    }
                    else
                    {
                        currentState = state.Idle; // -> Idle
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
        StartCoroutine(DieSequence());
    }

    private IEnumerator DieSequence()
    {
        // Останавливаем навигацию
        movment.StopNavAgent();

        // Ждём 
        yield return new WaitForSeconds(30f);

        Destroy(gameObject);
    }
}

