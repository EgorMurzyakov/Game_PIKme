using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    // Публичное поле, а не свойство (для скорости)
    public PlayerInput CurrentInput;

    // Буффер ввода
    private float inputBuffer = 0.2f;
    private float startLKM;
    private float startPKM;

    // Поля остаются приватными
    private float isMouseX;
    private float isMouseY;

    // События
    //public event Action OnButtonSpacePressed;
    //public event Action OnButtonAltPressed;
    //public event Action OnButtonLeftMousePressed;
    //public event Action OnButtonRightMousePressed;

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

        // ---------------- Буфер ввода для атаки ------------ Начало 
        CurrentInput.LKM = Input.GetMouseButton(0);
        if (CurrentInput.LKM)
        {
            startLKM = Time.time;
        }
        else if (Time.time < startLKM + inputBuffer)
        {
            CurrentInput.LKM = true;
        }
        CurrentInput.PKM = Input.GetMouseButton(1);
        if (CurrentInput.PKM) 
        {
            startPKM = Time.time;            
        }
        else if (Time.time < startPKM + inputBuffer)
        {
            CurrentInput.PKM = true;
        }
        // ---------------- Буфер ввода для атаки ------------ Конец



        isMouseX = Input.GetAxis("Mouse X");
        isMouseY = Input.GetAxis("Mouse Y");
    }

    public (float x, float y) GetMouseInput() // Mouse
    {
        return (isMouseX, isMouseY);
    }


}
