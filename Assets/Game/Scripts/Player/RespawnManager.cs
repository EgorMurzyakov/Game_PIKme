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
        // 1. Отключаем CharacterController чтобы можно было телепортировать
        CharacterController cc = stateMachine.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 2. Перемещаем на точку спавна
        stateMachine.transform.position = spawnPoint.position;
        stateMachine.transform.rotation = spawnPoint.rotation;

        // 3. Включаем обратно
        if (cc != null) cc.enabled = true;

        // 4. Восстанавливаем физику (на случай смерти от лавы)
        Rigidbody rb = stateMachine.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 5. Восстанавливаем коллайдер (на случай смерти от лавы)
        Collider col = stateMachine.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // 6. Сбрасываем HP, движение, стейт
        playerHP.Respawn();
        stateMachine.GoRespawnState();

        isRespawning = false;
    }
}