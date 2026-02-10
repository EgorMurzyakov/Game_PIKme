using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float mouseSen = 150f;
    //[SerializeField] private InputHandler input;
    private InputHandler input;

    private Vector3 emptyPosition; // �������� ��������
    private (float x, float y) mouseInput;
    private float xRotation = 0f;
    private float yRotation = 0f;


    void Start()
    {
        input = GetComponent<InputHandler>();
        emptyPosition = new Vector3(0f, transform.position.y, 0f); // �������� ��������
    }

    void LateUpdate()
    {
        transform.position = targetPlayer.position; // ������� �� ������� ���������
        transform.position += emptyPosition; // �������� ��������

        mouseInput = input.GetMouseInput(); // �������� ���� ����
        float mouseX = mouseInput.x * mouseSen * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSen * Time.deltaTime;

        xRotation -= mouseY;  // ������ ������ �����/����  
        xRotation = Mathf.Clamp(xRotation, -45f, 90f);  // �����������, ����� �� ����������� ������  
        yRotation += mouseX;  // ������� ������ �����/������  
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

}
