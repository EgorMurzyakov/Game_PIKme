using UnityEngine;

public class FireballProjectileCollision : MonoBehaviour
{
    [Header("Что может быть поражено")] 
    public LayerMask hitMask;

    [Header("Урон фаербола")] 
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        // проверяем, что слой попадает в маску
        if ((hitMask.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // попытка нанести урон через существующую систему
        var detector = collision.gameObject.GetComponent<DamageDetector>();
        if (detector != null)
        {
            detector.GetDamage(damage);
        }

        Destroy(gameObject);
    }
}
