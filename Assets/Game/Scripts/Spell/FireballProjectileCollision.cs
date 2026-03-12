using UnityEngine;

public class FireballProjectileCollision : MonoBehaviour
{
    [Header("Что может быть поражено")] 
    public LayerMask hitMask;

    [Header("Урон фаербола")] 
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Fireball] Столкновение с {collision.gameObject.name}, слой: {collision.gameObject.layer}");

        // Проверяем, попал ли в цель
        if ((hitMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log($"[Fireball] Наносим урон {damage}");

            var detector = collision.gameObject.GetComponentInParent<DamageDetector>();
            if (detector != null)
            {
                detector.GetDamage(damage);
            }
            else
            {
                Debug.LogWarning($"[Fireball] DamageDetector НЕ найден на {collision.gameObject.name}");
            }
        }

        // Уничтожаем фаербол в любом случае
        Destroy(gameObject);
    }
}
