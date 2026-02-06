using UnityEngine;

public enum state { Idle, Walk, Run, Sprint, Dodge, Attack }

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

    private state currentState = state.Idle;
    private state prevState = state.Idle;

    private bool flagBlaBlaBla = false;

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

        switch (currentState)
        {
            case state.Idle:
                if (isAlt == true) // Idle -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true) // Idle -> Attack
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
                if (isAlt == true) // Walk -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true) // Walk -> Attack
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
                if (isAlt == true) // Run -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true) // Run -> Attack
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
                if (isAlt == true) // Sprint -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true) // Sprint -> Attack
                {
                    currentState = state.Attack;
                }
                else if (isShift == false && isWASD == true) // Sprint -> Walk
                {
                    currentState = state.Walk;
                }
                else if (isWASD == false) // Spront -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Dodge:
                if (isAlt == true) // Dodge -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((isPKM || isLKM) == true) // Dodge-> Attack
                {
                    currentState = state.Attack;
                }
                if (flagBlaBlaBla)
                {
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
                if (isAlt == true) // Attack -> Dodge
                {
                    currentState = state.Dodge;
                }
                if (flagBlaBlaBla)
                {
                    if ((isPKM || isLKM) == true) // Attack -> Attack
                    {
                        currentState = state.Attack;
                    }
                    else if (isShift == true && isWASD == true) // Attack -> Run
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
                break;
        }

        movControl.ChoosingAction(currentState, isMoveInput); // Для движения сообщаем о новом состоянии всегда
        if (currentState != prevState)
        {
            Debug.Log(currentState);
            animControl.ChoosingAction(currentState, isLKM, isPKM); // Для движения сообщаем о новом состоянии только, если оно сменилось
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

    public void ChangeStateAfterAnim() // Вызывается из анимационных событий в Unity
    {
        Debug.Log("Вызван");
        flagBlaBlaBla = true;
        UpdateState();
        flagBlaBlaBla = false;
    }
}
