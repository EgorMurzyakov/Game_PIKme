using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float mouseSen = 150f;
    //[SerializeField] private InputHandler input;
    private InputHandler input;

    private Vector3 emptyPosition; // Смещение пустышки
    private (float x, float y) mouseInput;
    private float xRotation = 0f;
    private float yRotation = 0f;


    void Start()
    {
        input = GetComponent<InputHandler>();
        emptyPosition = new Vector3(0f, transform.position.y, 0f); // Смещение пустышки
    }

    void LateUpdate()
    {
        transform.position = targetPlayer.position; // Следуем за моделью персонажа
        transform.position += emptyPosition; // Смещение пустышки

        mouseInput = input.GetMouseInput(); // Получаем ввод мыши
        float mouseX = mouseInput.x * mouseSen * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSen * Time.deltaTime;

        xRotation -= mouseY;  // Наклон камеры вверх/вниз  
        xRotation = Mathf.Clamp(xRotation, -45f, 90f);  // Ограничение, чтобы не перевернуть голову  
        yRotation += mouseX;  // Поворот камеры влево/вправо  
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

}
