// PlayerSpawner.cs — вешается на Player
using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SpawnAtPoint());
    }

    private IEnumerator SpawnAtPoint()
    {
        yield return null;

        string savedID = PlayerPrefs.GetString("SpawnPoint", "");
        if (string.IsNullOrEmpty(savedID)) yield break;

        // Ищем все точки спавна на сцене
        SpawnPoint[] allPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint point in allPoints)
        {
            if (point.pointID == savedID)
            {
                // Отключаем CharacterController на время перемещения
                CharacterController cc = GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                transform.position = point.transform.position;

                if (cc != null) cc.enabled = true;

                Debug.Log("Заспавнился на: " + point.pointID);
                break;
            }
        }
    }
}