using UnityEngine;

public enum state { Idle, Walk, Run, Sprint, Dodge, Attack, Empty }

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private InputHandler input;
    [SerializeField] private MovementController movControl;
    [SerializeField] private AnimationController animControl;

    private Vector2 isMoveInput;

    private bool isWASD = false;
    private bool isShift = false;
    private bool isSpace = false;
    private bool isAlt = false;
    private bool isLKM = false;
    private bool isPKM = false;

    // Очень важно!!! (Костыли)
    private state currentState = state.Idle;
    private state prevState = state.Empty;
    private bool flagAttack = false;
    private bool flagMovment = false;

    // Настройки для Кулдауна
    private float lastAttackTime = 0f;
    private float lastDodgeTime = 0f;
    private const float COMBO_WINDOW = 1f; // Окно (Кулдаун) комбо 
    private const float DODGE_WINDOW = 0.25f; // Окно (Кулдаун) уклонения
    private bool canChangeState = true;
    private bool canChangeStateDodge = true; // (!!! Нужно придумать нормальные названия !!!)

    public void Start()
    {
        input.OnButtonAltPressed += ReactToButtonAlt; // Подписка на событие
        input.OnButtonLeftMousePressed += ReactToButtonLeftMouse; 
        input.OnButtonRightMousePressed += ReactToButtonRightMouse; 
    }

    public void OnDestroy()
    {
        input.OnButtonAltPressed -= ReactToButtonAlt; // Отписка от события
        input.OnButtonLeftMousePressed -= ReactToButtonLeftMouse;
        input.OnButtonRightMousePressed -= ReactToButtonRightMouse;
    }

    public void Update()
    {
        if (input.GetMoveInput() != isMoveInput) // Проверяем WASD каждый кадр, отдельные нажатия обрабатываем через События
        {
            isMoveInput = input.GetMoveInput();

            isWASD = isMoveInput.magnitude > 0.7f; // Если есть изменения, изменяем состояние
            UpdateState();
        }

        if (input.GetShiftInput() != isShift) // Проверяем Shift каждый кадр, отдельные нажатия обрабатываем через События
        {
            isShift = input.GetShiftInput();
            UpdateState();
        }
    }

    private void UpdateState()
    {
        // Приоритеты -> 1) Уклонение 2) Атака 3) Движения

        canChangeState = (lastAttackTime < Time.time - COMBO_WINDOW);
        canChangeStateDodge = (lastDodgeTime < Time.time - DODGE_WINDOW);

        switch (currentState)
        {
            case state.Idle:
                if (isAlt == true && canChangeStateDodge) // Idle -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true && canChangeState) // Idle -> Attack
                {
                    currentState = state.Attack;
                }
                else if (isShift == true && isWASD == true) // Idle -> Run
                {
                    currentState = state.Run;
                }
                else if (isWASD == true) // Idle -> Walk
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (isAlt == true && canChangeStateDodge) // Walk -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true && canChangeState) // Walk -> Attack
                {
                    currentState = state.Attack;
                }
                else if (isShift == true && isWASD == true) // Walk -> Run
                {
                    currentState = state.Run;
                }
                else if (isWASD == false) // Walk -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Run:
                if (isAlt == true && canChangeStateDodge) // Run -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true && canChangeState) // Run -> Attack
                {
                    currentState = state.Attack;
                }
                else if (isShift == false && isWASD == true) // Run -> Walk
                {
                    currentState = state.Walk;
                }
                else if (isWASD == false) // Run -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Sprint:
                if (isAlt == true && canChangeStateDodge) // Sprint -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true && canChangeState) // Sprint -> Attack
                {
                    currentState = state.Attack;
                }
                else if (isShift == false && isWASD == true) // Sprint -> Walk
                {
                    currentState = state.Walk;
                }
                else if (isWASD == false) // Sprint -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Dodge:
                if ((isPKM || isLKM) == true && canChangeState) // Dodge-> Attack
                {
                    currentState = state.Attack;
                }
                if (flagMovment)
                {
                    lastDodgeTime = Time.time;

                    if (isShift == true && isWASD == true) // Dodge -> Sprint
                    {
                        currentState = state.Sprint;
                    }
                    else if (isShift == false && isWASD == true) // Dodge -> Walk
                    {
                        currentState = state.Walk;
                    }
                    else // Dodge -> Idle
                    {
                        currentState = state.Idle;
                    }
                }
                break;

            case state.Attack:
                // |------------------{-------------}-----| <- Анимация Атаки
                //                    ^             ^
                //                    |             |
                // 1.Запрет на любой   2.Запрет на    3.Запрет на атаки, гарантирует переход
                //    Переход           движения           на движение (Это кто-то читает?)
                //  (Кроме уклонения)

                if (isAlt == true) // 1. Всегда можем уклониться 
                {
                    currentState = state.Dodge; // Attack -> Dodge
                }
                else if (flagAttack && ((isPKM || isLKM) == true)) // 2. Можем атаковать только если flagAttack == true 
                {
                    currentState = state.Attack; // Attack -> Attack
                    prevState = state.Empty; // Чтобы перейти в новую анимацию
                }
                else if (flagMovment) // 3. Можем ходить только если flagMovment == true
                {
                    lastAttackTime = Time.time;

                    if (isShift == true && isWASD == true) // Attack -> Run
                    {
                        currentState = state.Run;
                    }
                    else if (isWASD == true) // Attack -> Walk
                    {
                        currentState = state.Walk;
                    }
                    else // Attack -> Idle
                    {
                        currentState = state.Idle;
                    }
                }

                flagAttack = false; // Нет гарантий что EndChangeState() будет вызван
                break;
        }

        movControl.ChoosingAction(currentState, isMoveInput); // Для движения сообщаем о новом состоянии всегда
        if (currentState != prevState)
        {
            Debug.Log(currentState);
            animControl.ChoosingAction(currentState, isLKM, isPKM); // Для аттаки сообщаем о новом состоянии только, если оно сменилось
            prevState = currentState;
        }
    }
    

    void ReactToButtonAlt() // Реагируем на Событие
    {
        isAlt = true;
        UpdateState();
        isAlt = false;
    }
    void ReactToButtonLeftMouse()
    {
        isLKM = true;
        UpdateState();
        isLKM = false; 
    }    
    void ReactToButtonRightMouse()
    {
        isPKM = true;
        UpdateState();
        isPKM = false;
    }

    public void StartChangeState() // Начало промежутка в котором можно сменить состояние 
    {
        flagAttack = true;
    }

    public void EndChangeState() // Конец промежутка, гарантируем что состояние измениться
    {
        flagAttack = false;
        flagMovment = true;
        UpdateState();
        flagMovment = false;
    }
}
