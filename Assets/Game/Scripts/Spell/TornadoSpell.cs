using UnityEngine;

public class TornadoSpell : MonoBehaviour, ISpell
{
    public string Id => "TORNADO";

    [Header("Настройки Торнадо")]
    public GameObject prefab;
    public float speed = 34.5f;
    public float lifeTimeSeconds = 0.25f;

    private GameObject activeTornado;
    private Vector3 direction;
    private float spawnTime;

    public void Cast(Transform caster, Transform playerTransform)
    {
        if (prefab == null || playerTransform == null) return;


        if (activeTornado != null)
            Destroy(activeTornado);

        direction = playerTransform.forward.normalized;

        // Получаем нижнюю границу модели игрока
        float bottomY = 0f;
        Renderer renderer = playerTransform.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            bottomY = renderer.bounds.min.y;
        }
        else
        {
            Collider collider = playerTransform.GetComponent<Collider>();
            if (collider != null)
            {
                bottomY = collider.bounds.min.y;
            }
        }

        Vector3 pos = new Vector3(playerTransform.position.x, bottomY, playerTransform.position.z);
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);

        activeTornado = Instantiate(prefab, pos, rot);
        spawnTime = Time.time;



        Rigidbody rb = activeTornado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction;
        }
    }

    void Update()
    {
        if (activeTornado == null) return;

        // Движение каждый кадр
        // activeTornado.transform.position += direction * speed * Time.deltaTime;

        // Уничтожение через время
        if (Time.time - spawnTime >= lifeTimeSeconds)
        {
            Destroy(activeTornado);
            activeTornado = null;
        }
    }
}
