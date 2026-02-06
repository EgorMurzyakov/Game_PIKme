using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Поля остаются приватными
    private float isMouseX;
    private float isMouseY;

    private Vector2 moveInput;
    private bool isShift;

    // События
    public event Action OnButtonSpacePressed;
    public event Action OnButtonAltPressed;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Фиксируем курсор в центре экрана
        Cursor.visible = false; // Делаем курсор невидимым
    }

    private void Update()
    {
        isMouseX = Input.GetAxis("Mouse X");
        isMouseY = Input.GetAxis("Mouse Y");

        moveInput = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")).normalized; // GetAxisRaw() возвращает -1, 0, 1 без интерполяции (нет промежуточных значений)
        isShift = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKey(KeyCode.LeftAlt)) // Отдельные нажатия обрабатываем через События
        {
            OnButtonAltPressed?.Invoke();
        }
    }

    public Vector2 GetMoveInput() // WASD
    {
        return moveInput;
    }

    public bool GetShiftInput() // Shift
    {
        return isShift;
    }

    public (float x, float y) GetMouseInput() // Mouse
    {
        return (isMouseX, isMouseY);
    }

}
