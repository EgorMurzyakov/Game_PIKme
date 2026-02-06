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

    private state currentState = state.Idle;

    public void Start()
    {
        input.OnButtonAltPressed += ReactToButtonAlt; // Подписка на событие
    }

    public void OnDestroy()
    {
        input.OnButtonAltPressed -= ReactToButtonAlt; // Отписка от события
    }

    public void Update()
    {
        if (input.GetMoveInput() != isMoveInput) // Проверяем WASD каждый кадр, отдельные нажатия обрабатываем через События
        {
            isMoveInput = input.GetMoveInput();

            isWASD = isMoveInput.magnitude > 0.7f; // Если есть изменения, изменяем состояние
            //if (isMoveInput.magnitude > 0.01f) { isWASD = true; } // Если есть изменения, изменяем состояние
            //else { isWASD = false; }

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
        switch (currentState)
        {
            case state.Idle:
                if (isAlt == true) // Idle -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if (isShift == true && isWASD == true) // Idle -> Run
                {
                    currentState = state.Run;
                    Debug.Log("Ты хуй 1");
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
                else if (isShift == false && isWASD == true) // Run -> Walk
                {
                    currentState = state.Walk;
                }
                else if (isWASD == false) // Run -> Idle
                {
                    currentState = state.Idle;
                    Debug.Log("Ты хуй 2");
                }
                break;
            case state.Sprint:
                if (isAlt == true) // Sprint -> Dodge
                {
                    currentState = state.Dodge;
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
                else if (isShift == true && isWASD == true) // Dodge -> Sprint
                {
                    currentState = state.Sprint;
                }
                else if (isShift == false && isWASD == true) // Dodge -> Walk
                {
                    currentState = state.Walk;
                }
                else if (isWASD == false) // Dodge -> Idle
                {
                    currentState = state.Idle;
                }
                break;
        }

        Debug.Log(currentState);
        movControl.ChoosingAction(currentState, isMoveInput); // Сообщаем о новом состоянии
        animControl.ChoosingAction(currentState);
    }

    void ReactToButtonAlt() // Реагируем на Событие
    {
        isAlt = true;
        UpdateState();
        isAlt = false; // Обязательно сразу после обновления состояния
    }

    public void ChangeStateAfterAnim() // Вызывается из анимационных событий в Unity
    {
        UpdateState();
    }
}
