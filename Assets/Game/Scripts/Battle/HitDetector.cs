using UnityEngine;

public class HitDetector : MonoBehaviour // Универсальный (и для игрока и для врагов)
{
    [SerializeField] private GameObject character;
    [SerializeField] private int weaponDamage; // Сырой урон оружия
    private ColliderSwitch colliderSwitch;
    private Collider weaponCollider;
    private Sounds sounds;

    public void Start()
    {
        colliderSwitch = character.GetComponent<ColliderSwitch>();
        weaponCollider = GetComponent<Collider>();
        sounds = character.GetComponent<Sounds>();

        colliderSwitch.weaponColliderOn += ColliderOn; // Подписываемся на событие (Уведомление о включении коллайдера)
        colliderSwitch.weaponColliderOff += ColliderOff; // (Уведомление о выключении коллайдера)
        colliderSwitch.footColliderOn += FootColliderOn;
        colliderSwitch.footColliderOff += FootColliderOff;
    }

    public void OnDestroy()
    {
        colliderSwitch.weaponColliderOn -= ColliderOn; // Отписываемся от событий
        colliderSwitch.weaponColliderOff -= ColliderOff;
        colliderSwitch.footColliderOn -= FootColliderOn;
        colliderSwitch.footColliderOff -= FootColliderOff;
    }

    private void OnTriggerStay(Collider other) // Вызывается каждый кадр, по идеи урон должен проходить тоже каждый кадр (что является ошибкой), но этого вроде не происходит
    {
        //Debug.Log($"Объект {other.name} вошел в триггер");
        if (other.GetComponent<DamageDetector>())
        {
            other.GetComponent<DamageDetector>().GetDamage(weaponDamage);
            sounds.PlaySound(Sounds.SoundType.Hit); // Звук попадания по врагу (не нанесения урона)
        }
    }

    private void ColliderOn()
    {
        weaponCollider.enabled = true;
    }
    private void ColliderOff()
    {
        weaponCollider.enabled = false;
    }
    private void FootColliderOn()
    {

    }
    private void FootColliderOff()
    {

    }

    public void SetWeaponDamage(int _newDm) // Для изменение урона при прокачке
    {
        weaponDamage = _newDm;
    }
}
