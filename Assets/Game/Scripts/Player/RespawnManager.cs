using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;         // Точка спавна на сцене
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private float respawnDelay = 3f;      // Задержка перед респавном

    private bool isRespawning = false;

    private void Update()
    {
        // Следим за смертью игрока
        if (!isRespawning && stateMachine.GetPlayerState() == state.Death)
        {
            isRespawning = true;
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    private void Respawn()
    {
        // 1. Сначала перемещаем
        stateMachine.transform.position = spawnPoint.position;
        stateMachine.transform.rotation = spawnPoint.rotation;

        // 2. Потом восстанавливаем физику
        Rigidbody rb = stateMachine.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = stateMachine.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        playerHP.Respawn();
        stateMachine.GoRespawnState();

        isRespawning = false;
    }
}