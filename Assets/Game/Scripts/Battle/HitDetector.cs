using UnityEngine;

public class HitDetector : MonoBehaviour // Универсальный (и для игрока и для врагов)
{
    [SerializeField] private GameObject character;
    private ColliderSwitch colliderSwitch;
    private BoxCollider weaponCollider;

    public void Start()
    {
        colliderSwitch = character.GetComponent<ColliderSwitch>();
        weaponCollider = GetComponent<BoxCollider>();

        colliderSwitch.weaponColliderOn += ColliderOn; // Подписываемся на событие (Уведомление о включении коллайдера)
        colliderSwitch.weaponColliderOff += ColliderOff; // (Уведомление о выключении коллайдера)
    }

    public void OnDestroy()
    {
        colliderSwitch.weaponColliderOn -= ColliderOn; // Отписываемся от событий
        colliderSwitch.weaponColliderOff -= ColliderOff;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Объект {other.name} вошел в триггер");

        other.GetComponent<DamageDetector>().GetDamage();
        ColliderOff();
    }

    private void ColliderOn()
    {
        Debug.Log("true");
        weaponCollider.enabled = true;
    }
    private void ColliderOff()
    {
        Debug.Log("false");
        weaponCollider.enabled = false;
    }

}
