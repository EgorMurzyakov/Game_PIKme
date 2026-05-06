using UnityEngine;

public class QuestZoneTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        
        Debug.Log("Триггер сработал, объект: " + other.name + ", тег: " + other.tag);
        
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (QuestManager_Starosta.Instance == null)
        {
            Debug.LogWarning("QuestManager_Starosta не найден!");
            return;
        }

        Debug.Log("Статус квеста до: " + QuestManager_Starosta.Instance.Data.questStatus);
        QuestManager_Starosta.Instance.CompleteQuest();
        Debug.Log("Статус квеста после: " + QuestManager_Starosta.Instance.Data.questStatus);
    }
}