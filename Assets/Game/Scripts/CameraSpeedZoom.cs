using UnityEngine;

public class CameraSpeedZoom : MonoBehaviour
{
    [Header("Camera Targets")]
    [SerializeField] private Transform cameraNormalPos;
    [SerializeField] private Transform cameraZoomedPos;
    [SerializeField] private PlayerStateMachine playerState;

    [Header("Settings")]
    [SerializeField] private float zoomSpeed = 5f; // Скорость зума
    [SerializeField] private float returnDelay = 0.5f; // Задержка возврата

    private float currentZoom;
    private float targetZoom;
    private float delayTimer;

    void Update()
    {
        if (playerState == null) return;

        state curState = playerState.GetPlayerState();

        // Какие состояния требуют зума
        bool shouldZoom = (curState == state.Run ||
                          curState == state.Sprint ||
                          curState == state.Attack ||
                          curState == state.AttackB ||
                          curState == state.Dodge);

        if (shouldZoom)
        {
            targetZoom = 1f;
            delayTimer = 0f;
        }
        else
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= returnDelay)
                targetZoom = 0f;
        }

        // Плавный переход
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSpeed);

        // Применяем позицию
        if (cameraNormalPos != null && cameraZoomedPos != null)
        {
            transform.position = Vector3.Lerp(cameraNormalPos.position, cameraZoomedPos.position, currentZoom);
        }
    }
}
