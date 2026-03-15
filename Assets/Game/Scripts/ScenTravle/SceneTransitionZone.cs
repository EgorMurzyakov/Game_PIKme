
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionZone : MonoBehaviour
{
    [SerializeField] private string targetScene = "Location_2";
    [SerializeField] private string spawnID = "from_gate"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("SpawnPoint", spawnID);
            SceneManager.LoadScene(targetScene);
        }
    }
}