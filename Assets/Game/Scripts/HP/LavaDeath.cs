using UnityEngine;

public class LavaDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок коснулся лавы!");

            // Мгновенная остановка падения
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Отключаем коллайдер, чтобы не падал дальше
            Collider col = other.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Наносим урон (вызовет смерть)
            HitPoint hp = other.GetComponent<HitPoint>();
            if (hp != null)
            {
                hp.TakeDamage(999999);
            }
        }
    }
}