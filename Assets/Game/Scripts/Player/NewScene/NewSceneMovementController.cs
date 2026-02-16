using UnityEngine;

public class NewSceneMovementController : MonoBehaviour
{
    [SerializeField] private CharacterController charControl;
    [SerializeField] private NewSceneInputHandler input;

    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float dodgeSpeed = 8f;
    [SerializeField] private float speedValue = 4f;

    [Header("Dodge")]
    [SerializeField] private float dodgeDuration = 0.25f;

    [Header("Debug")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private state currentState = state.Idle;

    private Vector3 currentVelocity;
    private Vector2 moveInput;

    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 moveDirection;

    private bool isDodging;
    private float dodgeEndTime;

    private void Awake()
    {
        if (charControl == null)
        {
            charControl = GetComponent<CharacterController>();
        }

        if (input == null)
        {
            input = GetComponent<NewSceneInputHandler>();
        }
    }

    private void Update()
    {
        UpdateState();
        UpdateSpeed();
    }

    private void FixedUpdate()
    {
        GetMoving();
    }

    private void UpdateState()
    {
        PlayerInput currentInput = input != null ? input.CurrentInput : PlayerInput.Default;

        if (isDodging)
        {
            if (Time.time >= dodgeEndTime)
            {
                isDodging = false;
                currentState = ResolveLocomotionState(currentInput);
            }
        }
        else
        {
            if (currentInput.Alt)
            {
                StartDodge();
            }
            else
            {
                currentState = ResolveLocomotionState(currentInput);
            }
        }

        ChoosingAction(currentState, currentInput.Move);
    }

    private state ResolveLocomotionState(in PlayerInput inputState)
    {
        if (inputState.Shift && inputState.WASD)
        {
            return state.Run;
        }

        if (inputState.WASD)
        {
            return state.Walk;
        }

        return state.Idle;
    }

    private void StartDodge()
    {
        isDodging = true;
        dodgeEndTime = Time.time + dodgeDuration;
        currentState = state.Dodge;
        currentVelocity = transform.forward;
    }

    public void ChoosingAction(state selectedState, Vector2 selectedMoveInput)
    {
        moveInput = selectedMoveInput;

        switch (selectedState)
        {
            case state.Idle:
                StopMoving();
                break;
            case state.Walk:
                targetSpeed = walkSpeed;
                break;
            case state.Run:
                targetSpeed = runSpeed;
                break;
            case state.Sprint:
                targetSpeed = sprintSpeed;
                break;
            case state.Dodge:
                targetSpeed = dodgeSpeed;
                currentVelocity = transform.forward;
                currentSpeed = dodgeSpeed;
                break;
            case state.Attack:
                StopMoving();
                break;
        }
    }

    private void GetMoving()
    {
        Transform activeCamera = Camera.main != null ? Camera.main.transform : transform;

        cameraForward = activeCamera.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        cameraRight = activeCamera.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        moveDirection = cameraForward * moveInput.x + cameraRight * moveInput.y;
        moveDirection.Normalize();

        if (moveDirection.magnitude > 0.1f)
        {
            currentVelocity = moveDirection;
        }

        charControl.Move(currentVelocity * currentSpeed * Time.deltaTime);
    }

    private void StopMoving()
    {
        targetSpeed = 0f;
        currentSpeed = 0f;
    }

    private void UpdateSpeed()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedValue);
    }
}
