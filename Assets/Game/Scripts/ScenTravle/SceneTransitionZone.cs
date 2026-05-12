using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionZone : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string spawnID;
    [SerializeField] private SaveManager saveManager;

    private void Awake()
    {
        if (saveManager == null)
        {
            saveManager = FindObjectOfType<SaveManager>();
            if (saveManager == null)
            {
                Debug.LogWarning("SaveManager не найден в сцене. Назначьте SaveManager в инспекторе или добавьте объект с этим компонентом.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name == "ValeraAD_old_from_commit")
            {
                other.gameObject.GetComponent<InventoryManager>().RemoveItemByID("book_tornado", 26);
            }

            SaveGame();
            PlayerPrefs.SetString("SpawnPoint", spawnID);
            PlayerPrefs.SetString("TargetScene", targetScene); // куда грузить
            SceneManager.LoadScene("LoadingScreen"); // сначала заглушка
        }
    }

    public void SaveGame()
    {
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
        else
        {
            Debug.LogWarning("SaveManager не назначен в SceneTransitionZone.");
        }
    }
}