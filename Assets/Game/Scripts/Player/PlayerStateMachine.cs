using UnityEngine;
using UnityEngine.Windows;

public enum state { Idle, Walk, Run, Sprint, Dodge, Attack, Empty }

public class PlayerStateMachine : MonoBehaviour
{
    private InputHandler inputHandler;
    private MovementController movControl;
    private AnimationController animControl;
    private ColliderSwitch colliderSwitch;

    // Очень важно!!! (Костыли)
    private state currentState = state.Idle;
    private state prevState = state.Empty;
    private bool flagAttack = false; // <- Анимационное событие
    private bool flagMovment = false; // <- Анимационное событие
    private bool blockFlagAttack = false;
    private bool blockFlagMovment = false; // <- Костыль, блокирует flagMovment, если была атака, но EndChangeState() успел вызваться   

    // Настройки для Кулдауна
    private float lastAttackTime = 0f;
    private float lastDodgeTime = 0f;
    private const float COMBO_WINDOW = 1f; // Окно (Кулдаун) комбо 
    private const float DODGE_WINDOW = 0.25f; // Окно (Кулдаун) уклонения
    private bool canChangeStateAttack = true;
    private bool canChangeStateDodge = true;

    // Комбо 
    private string[] combo = { "L", "LL", "LLL", "R", "RR", "RRR", "LLR", "LLRR", "RRL", "RRLL" };
    private string currentCombo; 

    public void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        movControl = GetComponent<MovementController>();
        animControl = GetComponent<AnimationController>();
        colliderSwitch = GetComponent<ColliderSwitch>();
    }

    public void Update()
    {
        // === ШАГ 1: СБОР ВСЕГО ВВОДА ЗА КАДР ===
        ref PlayerInput currentInput = ref inputHandler.CurrentInput; // Берем ССЫЛКУ на оригинал, не копируем, работаем напрямую

        // === ШАГ 2: ОБРАБОТКА СОСТОЯНИЯ ===
        UpdateState(in currentInput); // 'in' = только для чтения

        // === ШАГ 3: ОЧИСТКА КАДРА ===
        flagMovment = false;
}

    private void UpdateState(in PlayerInput input)
    {
        // Приоритеты -> 1) Уклонение 2) Атака 3) Движения
        
        canChangeStateAttack = (lastAttackTime < Time.time - COMBO_WINDOW);
        canChangeStateDodge = (lastDodgeTime < Time.time - DODGE_WINDOW);

        switch (currentState)
        {
            case state.Idle:
                if (input.Alt && canChangeStateDodge) // Idle -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack) // Idle -> Attack
                {
                    if (input.PKM)
                    {
                        currentCombo = "R";
                    }
                    else
                    {
                        currentCombo = "L";
                    }


                    currentState = state.Attack;
                }
                else if (input.Shift && input.WASD) // Idle -> Run
                {
                    currentState = state.Run;
                }
                else if (input.WASD) // Idle -> Walk
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (input.Alt && canChangeStateDodge) // Walk -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack) // Walk -> Attack
                {
                    if (input.PKM)
                    {
                        currentCombo = "R";
                    }
                    else
                    {
                        currentCombo = "L";
                    }


                    currentState = state.Attack;
                }
                else if (input.Shift && input.WASD) // Walk -> Run
                {
                    currentState = state.Run;
                }
                else if (input.WASD == false) // Walk -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Run:
                if (input.Alt && canChangeStateDodge) // Run -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack) // Run -> Attack
                {
                    if (input.PKM)
                    {
                        currentCombo = "R";
                    }
                    else
                    {
                        currentCombo = "L";
                    }


                    currentState = state.Attack;
                }
                else if (input.Shift == false && input.WASD) // Run -> Walk
                {
                    currentState = state.Walk;
                }
                else if (input.WASD == false) // Run -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Sprint:
                if (input.Alt && canChangeStateDodge) // Sprint -> Dodge
                {
                    currentState = state.Dodge;
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack) // Sprint -> Attack
                {
                    if (input.PKM)
                    {
                        currentCombo = "R";
                    }
                    else
                    {
                        currentCombo = "L";
                    }


                    currentState = state.Attack;
                }
                else if (input.Shift == false && input.WASD) // Sprint -> Walk
                {
                    currentState = state.Walk;
                }
                else if (input.WASD == false) // Sprint -> Idle
                {
                    currentState = state.Idle;
                }
                break;

            case state.Dodge:
                if ((input.PKM || input.LKM) && flagAttack && canChangeStateAttack) // Dodge -> Attack
                {
                    if (input.PKM)
                    {
                        currentCombo = "R";
                    }
                    else
                    {
                        currentCombo = "L";
                    }


                    blockFlagMovment = true;
                    flagAttack = false; // <- Очищаем после использования
                    currentState = state.Attack;
                }
                else if (flagMovment)
                {
                    lastDodgeTime = Time.time;

                    if (input.Shift && input.WASD) // Dodge -> Sprint
                    {
                        currentState = state.Sprint;
                    }
                    else if (input.Shift == false && input.WASD) // Dodge -> Walk
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

                if (input.Alt) // 1. Всегда можем уклониться  
                {
                    blockFlagAttack = true;
                    currentState = state.Dodge; // Attack -> Dodge                    
                }
                else if (flagAttack && ((input.PKM || input.LKM))) // 2. Можем атаковать только если flagAttack == true 
                {
                    if (input.PKM)
                    {
                        currentCombo += "R";
                    }
                    else
                    {
                        currentCombo += "L";
                    }

                    blockFlagMovment = BattleChecker(); 

                    flagAttack = false; // <- Очищаем после использования
                    currentState = state.Attack; // Attack -> Attack
                    prevState = state.Empty; // Чтобы перейти в новую анимацию
                }
                else if (flagMovment) // 3. Можем ходить только если flagMovment == true
                {
                    lastAttackTime = Time.time;

                    if (input.Shift && input.WASD) // Attack -> Run
                    {
                        currentState = state.Run;
                    }
                    else if (input.WASD) // Attack -> Walk
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

        movControl.ChoosingAction(currentState, input.Move); // Для движения сообщаем о новом состоянии всегда
        if (currentState != prevState)
        {
            animControl.ChoosingAction(currentState, input.LKM, input.PKM); // Для аттаки сообщаем о новом состоянии только, если оно сменилось
            colliderSwitch.ChoosingAction(currentState); // Коллайдер меча
            prevState = currentState;
            Debug.Log(currentState);
        }
    }

    public void StartChangeStateDodge()
    {
        blockFlagAttack = false;
        flagAttack = true;
        blockFlagMovment = false;
    }

    public void StartChangeState() // Начало промежутка в котором можно сменить состояние 
    {
        flagAttack = !blockFlagAttack; // Очищаестся сразу после использования или в EndChangeState() 
        blockFlagMovment = false;
    }

    public void EndChangeState() // Конец промежутка, гарантируем что состояние измениться
    {

        flagAttack = false;
        flagMovment = !blockFlagMovment; // Очищается сразу после использования

    }

    private bool BattleChecker()
    {
        for (int i = 0; i < combo.Length; i++)
        {
            if (currentCombo == combo[i])
            {
                return true;
            }
        }
        return false;
    }
}
