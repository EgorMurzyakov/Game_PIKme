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

    [Header("Debug")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;

    public void Update()
    {
        UpdateSpeed();
    }
    public void FixedUpdate()
    {
        GetMoving();
    }

    public void ChoosingAction(state _st, Vector2 _mi)
    {
        moveInput = _mi; // Направление движения

        switch (_st)
        {
            case state.Idle:
                //targetSpeed = 0;
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
                currentSpeed = dodgeSpeed;
                break;
            case state.Attack:
                StopMoving();
                break;
        }
    }

    private void GetMoving()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * moveInput.x + cameraRight * moveInput.y;
        moveDirection.Normalize();

        if (moveDirection.magnitude > 0.1f)
        {
            currentVelocity = moveDirection; // currentVelocity сохранит направление движения когда moveDirection станет = 0
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion currentRotation = transform.rotation;
            //Определяем направление поворота
            Vector3 crossProduct = Vector3.Cross(currentRotation * Vector3.forward, targetRotation * Vector3.forward);

            //animator.SetBool("Right", crossProduct.y >= 0.05f);
            //animator.SetBool("Left", crossProduct.y <= -0.05f);
            //animator.SetBool("Forward", (-0.5f < crossProduct.y) && (crossProduct.y < 0.05f));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Иначе плавно
        }

        charControl.Move(currentVelocity * currentSpeed * Time.deltaTime);
    }

    private void StopMoving() // Мгновенная остановка
    {
        targetSpeed = 0;
        currentSpeed = 0;
    }
    void UpdateSpeed() // Плавное изменение скорости
    {
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedValue);
    }
}
