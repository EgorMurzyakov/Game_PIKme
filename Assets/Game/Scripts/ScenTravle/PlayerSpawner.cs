using UnityEngine;
using System.Collections;

// Вешается на объект Player в каждой сцене
public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SpawnAtPoint());
    }

    private IEnumerator SpawnAtPoint()
    {
        yield return null; // ждём один кадр

        string savedID = PlayerPrefs.GetString("SpawnPoint", "");
        if (string.IsNullOrEmpty(savedID)) yield break;

        SpawnPoint[] allPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint point in allPoints)
        {
            if (point.pointID == savedID)
            {
                CharacterController cc = GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }

                transform.position = point.transform.position;

                yield return null;

                if (rb != null) rb.isKinematic = false;
                if (cc != null) cc.enabled = true;

                Debug.Log("Заспавнился на: " + point.pointID);
                break;
            }
        }
    }
}
