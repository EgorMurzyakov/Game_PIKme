using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionZone : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string spawnID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("SpawnPoint", spawnID);
            PlayerPrefs.SetString("TargetScene", targetScene); // куда грузить
            
            SceneManager.LoadScene("LoadingScreen"); // сначала заглушка
        }
    }
}