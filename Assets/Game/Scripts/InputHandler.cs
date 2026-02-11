using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Публичное поле, а не свойство (для скорости)
    public PlayerInput CurrentInput;

    // Поля остаются приватными
    private float isMouseX;
    private float isMouseY;

    // События
    public event Action OnButtonSpacePressed;
    public event Action OnButtonAltPressed;
    public event Action OnButtonLeftMousePressed;
    public event Action OnButtonRightMousePressed;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Фиксируем курсор в центре экрана
        Cursor.visible = false; // Делаем курсор невидимым
    }

    private void Update()
    {
        // Обновляем поля структуры напрямую
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

    public (float x, float y) GetMouseInput() // Mouse
    {
        return (isMouseX, isMouseY);
    }

}
