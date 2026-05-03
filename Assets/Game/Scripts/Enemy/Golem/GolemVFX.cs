using UnityEngine;
using System.Collections;

public class GolemVFX : MonoBehaviour
{
    [Header("Тряска камеры (опционально)")]
    [SerializeField] private CameraShake cameraShake;

    [Header("VFX (основа)")]
    [SerializeField] private ParticleSystem particles_1;
    [SerializeField] private ParticleSystem particles_2;
    [SerializeField] private GameObject eyasA;
    [SerializeField] private GameObject eyasB;

    public void StartParticle(int _a)
    {
        switch (_a)
        {
            case 0:
                SpawnVFX(particles_1);
                if (cameraShake != null)
                    cameraShake.Shake(0.5f, 0.2f, 0.2f, 0.4f, 0f);
                break;
            case 1:
                SpawnVFX(particles_2);
                if (cameraShake != null)
                    cameraShake.Shake(0.4f, 0.15f, 0.15f, 0.4f, 0f);
                break;
        }
    }

    public void BlinkEyas()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        eyasA.SetActive(true);
        eyasB.SetActive(true);
        yield return new WaitForSeconds(0.4f); // 0.2 секунды
        eyasA.SetActive(false);
        eyasB.SetActive(false);
    }

    //private IEnumerator PlayAndStop(ParticleSystem ps)
    //{
    //    ps.Play();
    //    yield return new WaitForSeconds(ps.main.duration);
    //    ps.Stop();
    //}
    private void SpawnVFX(ParticleSystem original)
    {
        if (original == null) return;

        // Создаём копию в мире на позиции и вращении оригинала
        Vector3 spawnPosition = original.transform.position;
        Quaternion spawnRotation = original.transform.rotation;

        ParticleSystem vfxCopy = Instantiate(original, spawnPosition, spawnRotation);

        // Отключаем от любого родителя (чтобы жил своей жизнью)
        vfxCopy.transform.SetParent(null);

        // Запускаем и удаляем
        StartCoroutine(PlayAndDestroy(vfxCopy));
    }

    private IEnumerator PlayAndDestroy(ParticleSystem ps)
    {
        // Отключаем зацикливание на всякий случай
        var main = ps.main;
        main.loop = false;

        // Запускаем
        ps.Play();

        // Ждём окончания
        yield return new WaitForSeconds(main.duration);

        // Останавливаем и удаляем
        ps.Stop();
        Destroy(ps.gameObject);
    }


}
