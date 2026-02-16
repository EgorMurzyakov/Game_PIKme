using UnityEngine;

public class NewSceneInputHandler : MonoBehaviour
{
    public PlayerInput CurrentInput;

    public Vector2 MoveInput => CurrentInput.Move;
    public Vector2 LookInput => new Vector2(isMouseX, isMouseY);
    public bool IsRunPressed => CurrentInput.Shift;

    private float isMouseX;
    private float isMouseY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Keep the same axis layout as the old InputHandler/MovementController pair.
        CurrentInput.Move = new Vector2(
            Input.GetAxisRaw("Vertical"),
            Input.GetAxisRaw("Horizontal")
        ).normalized;

        CurrentInput.WASD = CurrentInput.Move.magnitude > 0.1f;
        CurrentInput.Shift = Input.GetKey(KeyCode.LeftShift);
        CurrentInput.Alt = Input.GetKey(KeyCode.LeftAlt);
        CurrentInput.LKM = Input.GetMouseButton(0);
        CurrentInput.PKM = Input.GetMouseButton(1);

        isMouseX = Input.GetAxis("Mouse X");
        isMouseY = Input.GetAxis("Mouse Y");
    }

    public (float x, float y) GetMouseInput()
    {
        return (isMouseX, isMouseY);
    }
}
