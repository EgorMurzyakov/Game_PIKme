using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private CharacterController charControl;
    private Vector3 currentVelocity;

    [Header("Speed Settings")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float dodgeSpeed;

    private float speedValue = 4f;
    private Vector2 moveInput;

    private Vector3 cameraForward;
    private Vector3 cameraRight;
    private Vector3 moveDirection;
    private Quaternion targetRotation;
    private Quaternion currentRotation;
    private Vector3 crossProduct;

    private float lastChangeTurn;
    private const float ChangeTurn_WINDOW = 0.07f;
    private bool turnAllow = true;
    private bool death = false;

    [Header("Debug")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;

    // ?? ¿¬“Œ-œŒ»—   ŒÃœŒÕ≈Õ“¿ œ–» —“¿–“≈
    private void Awake()
    {
        if (charControl == null)
        {
            charControl = GetComponent<CharacterController>();
        }

        if (charControl == null)
        {
            Debug.LogError($"[MovementController] CharacterController not found on {gameObject.name}! Please add the component or assign it in Inspector.");
        }
    }

    public void Update()
    {
        UpdateSpeed();
    }

    public void FixedUpdate()
    {
        if (!death)
        {
            GetMoving();
        }
    }

    public void ChoosingAction(state _st, Vector2 _mi)
    {
        moveInput = _mi;

        switch (_st)
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
            case state.Death:
                death = true;
                break;
        }
    }

    private void GetMoving()
    {
        // ?? «‡˘ËÚ‡ ÓÚ null Camera.main
        if (Camera.main == null) return;

        cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        if (!(turnAllow == false && Time.time - ChangeTurn_WINDOW > lastChangeTurn))
        {
            moveDirection = cameraForward * moveInput.x + cameraRight * moveInput.y;
            moveDirection.Normalize();
        }

        if (moveDirection.magnitude > 0.1f)
        {
            currentVelocity = moveDirection;
        }

        if (moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
            currentRotation = transform.rotation;
            crossProduct = Vector3.Cross(currentRotation * Vector3.forward, targetRotation * Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ?? «‡˘ËÚ‡ ÓÚ null charControl
        if (charControl != null)
        {
            charControl.Move(currentVelocity * currentSpeed * Time.deltaTime);
        }
    }

    private void StopMoving()
    {
        targetSpeed = 0;
        currentSpeed = 0;
    }

    void UpdateSpeed()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedValue);
    }

    public void SetTurnAllow(bool _val)
    {
        turnAllow = _val;
        lastChangeTurn = Time.time;
    }
}