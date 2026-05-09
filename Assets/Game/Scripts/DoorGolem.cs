using Unity.VisualScripting;
using UnityEngine;

public class DoorGolem : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private GolemStateMashine golemStateMashine;
    [SerializeField] private GameObject door_1;
    [SerializeField] private GameObject door_2;

    private BoxCollider triggerCollider;

    public void Start()
    {
        triggerCollider = GetComponent<BoxCollider>();
        door_1.SetActive(false);
        door_2.SetActive(true);

        playerStateMachine.playerDeath += RestartDoor;
    }

    public void OnDestroy()
    {
        playerStateMachine.playerDeath -= RestartDoor;
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
            golemStateMashine.SetActiv(true);
            door_1.SetActive(true);
            door_2.SetActive(false);

            triggerCollider.enabled = false; // Отключаем коллайдер после 
        }
    }

    public void RestartDoor()
    {
        triggerCollider.enabled = true;

        door_1.SetActive(false);
        door_2.SetActive(true);

        golemStateMashine.SetActiv(false);
    }
}
