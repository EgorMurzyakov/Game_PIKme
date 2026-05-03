using UnityEngine;
using System.Collections;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private Transform[] platforms;
    [SerializeField] private float riseHeight = 5f;
    [SerializeField] private float riseSpeed = 2f;

    [Header("Вращение (опционально)")]
    [SerializeField] private bool changeRotation = false;
    [SerializeField] private Vector3 targetRotation = Vector3.zero;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Тряска камеры (опционально)")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeDelay = 0f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private float shakeFadeIn = 0.2f;
    [SerializeField] private float shakeFadeOut = 0.4f;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other.CompareTag("Player"))
        {
            activated = true;

            foreach (var platform in platforms)
                StartCoroutine(RisePlatform(platform));

            if (cameraShake != null)
                cameraShake.Shake(shakeDuration, shakeStrength, shakeFadeIn, shakeFadeOut, shakeDelay);
        }
    }

    private IEnumerator RisePlatform(Transform platform)
    {
        Vector3 targetPos = platform.position + Vector3.up * riseHeight;
        Quaternion targetRot = platform.rotation * Quaternion.Euler(targetRotation);

        bool positionDone = false;
        bool rotationDone = !changeRotation;

        while (!positionDone || !rotationDone)
        {
            if (!positionDone)
            {
                platform.position = Vector3.MoveTowards(platform.position, targetPos, riseSpeed * Time.deltaTime);
                if (Vector3.Distance(platform.position, targetPos) < 0.01f)
                {
                    platform.position = targetPos;
                    positionDone = true;
                }
            }

            if (!rotationDone)
            {
                platform.rotation = Quaternion.RotateTowards(platform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                if (Quaternion.Angle(platform.rotation, targetRot) < 0.1f)
                {
                    platform.rotation = targetRot;
                    rotationDone = true;
                }
            }

            yield return null;
        }
    }
}