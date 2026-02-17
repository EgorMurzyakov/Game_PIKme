using UnityEngine;

public class FireballProjectileCollision : MonoBehaviour
{
    public LayerMask hitMask;

    private void OnCollisionEnter(Collision collision)
    {
        if ((hitMask.value & (1 << collision.gameObject.layer)) == 0)
            return;

        Destroy(gameObject);
    }
}
