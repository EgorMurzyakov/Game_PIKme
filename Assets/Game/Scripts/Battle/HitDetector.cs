using UnityEngine;

public class HitDetector : MonoBehaviour // Универсальный (и для игрока и для врагов)
{
    [SerializeField] private GameObject character;
    [SerializeField] private int weaponDamage; // Сырой урон оружия, пока так (можно оставить (хз)), позже можно брать из Item самого оружия    
    private ColliderSwitch colliderSwitch;
    private BoxCollider weaponCollider;
    private Sounds sounds;

    public void Start()
    {
        colliderSwitch = character.GetComponent<ColliderSwitch>();
        weaponCollider = GetComponent<BoxCollider>();
        sounds = character.GetComponent<Sounds>();

        colliderSwitch.weaponColliderOn += ColliderOn; // Подписываемся на событие (Уведомление о включении коллайдера)
        colliderSwitch.weaponColliderOff += ColliderOff; // (Уведомление о выключении коллайдера)
    }

    public void OnDestroy()
    {
        colliderSwitch.weaponColliderOn -= ColliderOn; // Отписываемся от событий
        colliderSwitch.weaponColliderOff -= ColliderOff;
    }

    private void OnTriggerStay(Collider other) // Вызывается каждый кадр, по идеи урон должен проходить тоже каждый кадр (что является ошибкой), но этого вроде не происходит
    {
        Debug.Log($"Объект {other.name} вошел в триггер");

        other.GetComponent<DamageDetector>().GetDamage(weaponDamage);
        sounds.PlaySound(Sounds.SoundType.Hit); // Звук попадания по врагу (не нанесения урона)
    }

    private void ColliderOn()
    {
        //Debug.Log("true");
        weaponCollider.enabled = true;
    }
    private void ColliderOff()
    {
        //Debug.Log("false");
        weaponCollider.enabled = false;
    }

}
