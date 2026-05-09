using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string pointID = "from_bridge";

    private void Start()
    {
        if (PlayerPrefs.GetString("SpawnPoint") == pointID)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                player.transform.position = transform.position;
        }
    }
}