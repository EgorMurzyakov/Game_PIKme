using UnityEngine;

public enum state { Idle, Walk, Run, Sprint, Dodge, Attack, Empty, Action, Damage, Death }

public class PlayerStateMachine : MonoBehaviour
{
    private InputHandler inputHandler;
    private MovementController movControl;
    private AnimationController animControl;
    private ColliderSwitch colliderSwitch;

    private state currentState = state.Idle;
    private state prevState = state.Empty;
    private bool flagAttack = false;
    private bool flagMovment = false;
    private bool blockFlagAttack = false;
    private bool blockFlagMovment = false;

    private float lastAttackTime = 0f;
    private float lastDodgeTime = 0f;
    private const float COMBO_WINDOW = 1f;
    private const float DODGE_WINDOW = 0.25f;
    private bool canChangeStateAttack = true;
    private bool canChangeStateDodge = true;
    private bool weaponInHand = false;
    private bool death = false;

    private string[] combo = { "L ", "LL ", "LLL ", "R ", "RR ", "RRR ", "LLR ", "LLRR ", "RRL ", "RRLL " };
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
        ref PlayerInput currentInput = ref inputHandler.CurrentInput;
        UpdateState(in currentInput);
        flagMovment = false;
    }

    private void UpdateState(in PlayerInput input)
    {
        canChangeStateAttack = (lastAttackTime < Time.time - COMBO_WINDOW);
        canChangeStateDodge = (lastDodgeTime < Time.time - DODGE_WINDOW);

        if (death)
        {
            currentState = state.Death;
        }

        switch (currentState)
        {
            case state.Idle:
                if (input.Alt && canChangeStateDodge)
                {
                    currentState = state.Dodge;
                    movControl.SetTurnAllow(false);
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack && weaponInHand)
                {
                    currentCombo = input.PKM ? "R " : "L ";
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                }
                else if (input.Shift && input.WASD)
                {
                    currentState = state.Run;
                }
                else if (input.WASD)
                {
                    currentState = state.Walk;
                }
                break;

            case state.Walk:
                if (input.Alt && canChangeStateDodge)
                {
                    currentState = state.Dodge;
                    movControl.SetTurnAllow(false);
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack && weaponInHand)
                {
                    currentCombo = input.PKM ? "R " : "L ";
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                }
                else if (input.Shift && input.WASD)
                {
                    currentState = state.Run;
                }
                else if (!input.WASD)
                {
                    currentState = state.Idle;
                }
                break;

            case state.Run:
                if (input.Alt && canChangeStateDodge)
                {
                    currentState = state.Dodge;
                    movControl.SetTurnAllow(false);
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack && weaponInHand)
                {
                    currentCombo = input.PKM ? "R " : "L ";
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                }
                else if (!input.Shift && input.WASD)
                {
                    currentState = state.Walk;
                }
                else if (!input.WASD)
                {
                    currentState = state.Idle;
                }
                break;

            case state.Sprint:
                if (input.Alt && canChangeStateDodge)
                {
                    currentState = state.Dodge;
                    movControl.SetTurnAllow(false);
                }
                else if ((input.PKM || input.LKM) && canChangeStateAttack && weaponInHand)
                {
                    currentCombo = input.PKM ? "R " : "L ";
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                }
                else if (!input.Shift && input.WASD)
                {
                    currentState = state.Walk;
                }
                else if (!input.WASD)
                {
                    currentState = state.Idle;
                }
                break;

            case state.Dodge:
                if ((input.PKM || input.LKM) && flagAttack && canChangeStateAttack && weaponInHand)
                {
                    currentCombo = input.PKM ? "R " : "L ";
                    blockFlagMovment = true;
                    flagAttack = false;
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                }
                if (flagMovment)
                {
                    lastDodgeTime = Time.time;
                    currentState = (input.Shift && input.WASD) ? state.Sprint :
                                   (input.WASD) ? state.Walk : state.Idle;
                }
                break;

            case state.Attack:
                if (input.Alt && flagMovment)
                {
                    blockFlagAttack = true;
                    currentState = state.Dodge;
                    movControl.SetTurnAllow(false);
                }
                else if (flagAttack && (input.PKM || input.LKM))
                {
                    currentCombo += input.PKM ? "R " : "L ";
                    blockFlagMovment = BattleChecker();
                    flagAttack = false;
                    currentState = state.Attack;
                    movControl.SetTurnAllow(false);
                    prevState = state.Empty;
                }
                else if (flagMovment)
                {
                    lastAttackTime = Time.time;
                    currentState = (input.Shift && input.WASD) ? state.Run :
                                   (input.WASD) ? state.Walk : state.Idle;
                }
                break;

            case state.Death:
                break;
        }

        movControl.ChoosingAction(currentState, input.Move);
        if (currentState != prevState)
        {
            animControl.ChoosingAction(currentState, input.LKM, input.PKM);
            colliderSwitch.ChoosingAction(currentState);
            prevState = currentState;
        }
    }

    public void StartChangeStateDodge()
    {
        blockFlagAttack = false;
        flagAttack = true;
        blockFlagMovment = false;
        movControl.SetTurnAllow(true);
    }

    public void StartChangeState()
    {
        flagAttack = !blockFlagAttack;
        blockFlagMovment = false;
    }

    public void EndChangeState()
    {
        flagAttack = false;
        flagMovment = !blockFlagMovment;
        movControl.SetTurnAllow(true);
    }

    private bool BattleChecker()
    {
        foreach (var c in combo)
            if (currentCombo == c) return true;
        return false;
    }

    public void GoDeathState()
    {
        death = true;
    }

    public state GetPlayerState() => currentState;
    public void SetWeaponInHand(bool _val) => weaponInHand = _val;

    public void GoRespawnState()
    {
        death = false;
        currentState = state.Idle;
        prevState = state.Empty;

        if (movControl != null)
        {
            movControl.Respawn(); // вместо старых двух строчек с movControl
        }

        if (animControl != null)
            animControl.ResetToIdle();
    }
}