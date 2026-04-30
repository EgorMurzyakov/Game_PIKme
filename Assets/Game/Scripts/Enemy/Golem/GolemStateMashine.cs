using UnityEngine;

public class GolemStateMashine : MonoBehaviour
{
    public enum golemPhase { First, Second, Third }    

    [SerializeField] private GameObject player;
    private GolemMovment movment;
    private GolemAnimation anim;
    private ColliderSwitch colliderSwitch;

    private state currentState = state.Idle;
    private state prevState = state.Empty;
    private golemPhase currentPhase = golemPhase.First;
    //public golemPhase currentPhase = golemPhase.First; // ¬ременно дл€ отладки

    [SerializeField] private bool isActiv;
    [SerializeField] private float footAttackRange; // ƒистанци€ атаки ногой
    [SerializeField] private float weaponAttackRange; // ƒистанци€ атаки оружием
    private string[] combo = { "A", "B", "AB", "AA", "AAA", "BA"};
    private string currentCombo;     

    private bool canChangeState = false;
    private bool takingDamage = false;
    private bool death = false;
    private float distanceToPlayer;

    private float takingDamageColdown = 5f;
    private float prevColdownDate;

    private bool visualLoss = false; 

    public void Start()
    {        
        movment = GetComponent<GolemMovment>();
        anim = GetComponent<GolemAnimation>();
        colliderSwitch = GetComponent<ColliderSwitch>();
        prevColdownDate = Time.time;
    }

    public void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (isActiv)
        {
            UpdateState();            
        }

        movment.GetMoving(currentState);
        if (currentState != prevState) // Ќе даст каждый кадр включать анимацию заново
        {
            anim.GetAnimation(currentState);
            colliderSwitch.ChoosingAction(currentState);
            //Debug.Log(currentState);
            prevState = currentState;
        }
    }

    private void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.T)) // ¬ременно
        {
            SetVisualLoss();
        }



        if (death)
        {
            currentState = state.Death;
        }
        else if (takingDamage) // —амое приоритетное (нет)
        {
            currentState = state.Damage;
            //prevState = state.Empty;
            takingDamage = false;
        }

        switch (currentState)
        {
            case state.Idle:
                if (distanceToPlayer < footAttackRange)
                {
                    currentCombo = "B";
                    currentState = state.AttackB;
                }
                else if (distanceToPlayer < weaponAttackRange)
                {
                    currentCombo = "A";
                    currentState = state.Attack;
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
                if (distanceToPlayer < footAttackRange)
                {
                    currentCombo = "B";
                    currentState = state.AttackB;
                }
                else if (distanceToPlayer < weaponAttackRange)
                {
                    currentCombo = "A";
                    currentState = state.Attack;
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
                    // 1 фаза: выпад или удар ногой
                    // 2 фаза: выпад + удар ногой или удар ногой
                    // 3 фаза: выпад + выпад + выпад или удар ногой + выпад
                    // сразу после атаки тер€ет игрока



                    if (currentPhase == golemPhase.First)
                    {
                        currentState = state.Action;
                        //if (visualLoss)
                        //{
                        //    visualLoss = false;
                        //    currentState = state.Action;
                        //}

                        //else if (distanceToPlayer < footAttackRange && canAttack)
                        //{
                        //    currentState = state.AttackB;
                        //    prevState = state.Empty;
                        //    startAttckColdown = Time.time;
                        //}
                        //else if (distanceToPlayer < weaponAttackRange && canAttack)
                        //{
                        //    currentState = state.Attack;
                        //    prevState = state.Empty;
                        //    startAttckColdown = Time.time;
                        //}
                        //else
                        //{
                        //    currentState = state.Walk;
                        //}
                    }
                    else if (currentPhase == golemPhase.Second)
                    {
                        currentCombo += "B";

                        if (BattleChecker())
                        {
                            currentState = state.AttackB;
                        }
                        else
                        {
                            currentState = state.Action;
                        }
                    }
                    else if (currentPhase == golemPhase.Third)
                    {
                        currentCombo += "A";

                        if (BattleChecker())
                        {
                            Debug.Log(currentCombo);

                            currentState = state.Attack;
                            prevState = state.Empty; // ѕозволит проиграть анимацию заново
                        }
                        else
                        {
                            currentState = state.Action;
                        }
                    }

                    canChangeState = false;
                }
                break;

            case state.AttackB:
                if (canChangeState)
                {
                    // 1 фаза: выпад или удар ногой
                    // 2 фаза: выпад + удар ногой или удар ногой
                    // 3 фаза: выпад + выпад + выпад или удар ногой + выпад
                    // сразу после атаки тер€ет игрока

                    if (currentPhase == golemPhase.First)
                    {
                        currentState = state.Action;
                    }
                    else if (currentPhase == golemPhase.Second)
                    {
                        currentState = state.Action; // ѕри любом исходе это последний удар (2 фаза), нет смысла делать проверку
                    }
                    else if (currentPhase == golemPhase.Third)
                    {
                        currentCombo += "A";

                        if (BattleChecker())
                        {
                            currentState = state.Attack;
                        }
                        else
                        {
                            currentState = state.Action;
                        }
                    }

                    canChangeState = false;
                }
                break;

            case state.Action:
                if (canChangeState)
                {
                    currentCombo = "";

                    if (distanceToPlayer < footAttackRange)
                    {
                        currentCombo = "B";
                        currentState = state.AttackB;
                        prevState = state.Empty;
                    }
                    else if (distanceToPlayer < weaponAttackRange)
                    {
                        currentCombo = "A";
                        currentState = state.Attack;
                        prevState = state.Empty;
                    }
                    else
                    {
                        currentState = state.Walk;
                    }

                    canChangeState = false;
                }
                break;

            case state.Damage:
                prevColdownDate = Time.time;

                if (canChangeState)
                {
                    currentCombo = "";

                    if (distanceToPlayer < footAttackRange)
                    {
                        currentCombo = "B";
                        currentState = state.AttackB;
                        prevState = state.Empty;
                    }
                    else if (distanceToPlayer < weaponAttackRange)
                    {
                        currentCombo = "A";
                        currentState = state.Attack;
                        prevState = state.Empty;
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

    private bool BattleChecker() // ¬ернет true, если такое комбо есть
    {
        foreach (string _cur in combo)
        {
            if (_cur == currentCombo) return true;
        }
        return false;
    }

    public void EndChangeState() // ¬ызываетс€ из анимационных событий, canChangeState -> true -> можем сменить состо€ние 
    {
        canChangeState = true;
        takingDamage = false;
    }

    public void GoDamageState() // ѕолучаем урон - true, не получаем - false
    {
        if (Time.time - takingDamageColdown > prevColdownDate)
        {
            takingDamage = true;
        }
    }

    public void GoDeathState()
    {
        death = true;
    }

    public void SetVisualLoss()
    {
        visualLoss = true;
    }

    public void SetGolemPhase(golemPhase _ph) // ”станавливает GolemHP
    {
        currentPhase = _ph;
    }
}
