using UnityEngine;

public class FireballSpell : MonoBehaviour, ISpell
{
    public string Id => "FIREBALL";

    [Header("Настройки фаербола")]
    public GameObject prefab;
    public float speed = 34.5f;
    public float lifeTimeSeconds = 5f;

    private GameObject activeFireball;
    private Vector3 direction;
    private float spawnTime;

    public void Cast(Transform caster, Transform cameraTransform)
    {
        if (prefab == null || cameraTransform == null) return;


        if (activeFireball != null)
            Destroy(activeFireball);

        direction = cameraTransform.forward.normalized;

        Vector3 pos = cameraTransform.position + direction * 5f;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        activeFireball = Instantiate(prefab, pos, rot);
        spawnTime = Time.time;
    }

    void Update()
    {
        if (activeFireball == null) return;

        // Движение каждый кадр
        activeFireball.transform.position += direction * speed * Time.deltaTime;

        // Уничтожение через время
        if (Time.time - spawnTime >= lifeTimeSeconds)
        {
            Destroy(activeFireball);
            activeFireball = null;
        }
    }
}
