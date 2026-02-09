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

    //private Vector2 moveInput;
    //private bool isShift;
    //private bool isAlt;
    //private bool isLKM;
    //private bool isPKM;  

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

        // Не нужно создавать новый PlayerInput каждый кадр!
        // Просто меняем поля существующего

        isMouseX = Input.GetAxis("Mouse X");
        isMouseY = Input.GetAxis("Mouse Y");

        //moveInput = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")).normalized; // GetAxisRaw() возвращает -1, 0, 1 без интерполяции (нет промежуточных значений)
        //isShift = Input.GetKey(KeyCode.LeftShift);
        //isAlt = Input.GetKey(KeyCode.LeftAlt);
        //isLKM = Input.GetMouseButtonDown(0);
        //isPKM = Input.GetMouseButtonDown(1);

    }

    //public (Vector2 _WASD, bool _Shift, bool _Alt, bool _LKM, bool _PKM) GetInput()
    //{
    //    return (moveInput, isShift, isAlt, isLKM, isPKM);
    //}

    //public Vector2 GetMoveInput() // WASD
    //{
    //    return moveInput;
    //}

    //public bool GetShiftInput() // Shift
    //{
    //    return isShift;
    //}

    // Альтернативно: метод для получения копии
    public PlayerInput GetInputCopy()
    {
        return CurrentInput; // Возвращает КОПИЮ структуры
    }

    public (float x, float y) GetMouseInput() // Mouse
    {
        return (isMouseX, isMouseY);
    }

}
