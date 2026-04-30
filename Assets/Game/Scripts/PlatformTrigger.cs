using UnityEngine;
using System.Collections;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] platforms;   // Платформы
    [SerializeField] private float riseHeight = 5f;   // На сколько подняться
    [SerializeField] private float riseSpeed = 2f;    // Скорость подъёма

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            foreach (var platform in platforms)
            {
                StartCoroutine(RisePlatform(platform));
            }
        }
    }

    private IEnumerator RisePlatform(Transform platform)
    {
        Vector3 startPos = platform.position;
        Vector3 targetPos = startPos + Vector3.up * riseHeight;

        while (platform.position.y < targetPos.y - 0.01f)
        {
            platform.position = Vector3.MoveTowards(platform.position, targetPos, riseSpeed * Time.deltaTime);
            yield return null;
        }

        platform.position = targetPos;
    }
}