using UnityEngine;

public class NewSceneCameraLook : MonoBehaviour
{
    [SerializeField] private NewSceneInputHandler input;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 150f;
    [SerializeField] private float minPitch = -45f;
    [SerializeField] private float maxPitch = 80f;

    private float pitch;
    private float yaw;

    private void Start()
    {
        if (input == null)
        {
            input = GetComponentInParent<NewSceneInputHandler>();
        }

        if (playerBody != null)
        {
            yaw = playerBody.eulerAngles.y;
        }
    }

    private void LateUpdate()
    {
        Vector2 lookInput = input != null
            ? input.LookInput
            : new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
