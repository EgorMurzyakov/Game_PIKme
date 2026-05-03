[System.Serializable]
public class QuestData
{
    public string questStatus;  // "inactive", "kill_phase", "completed"
    public bool questCompleted;
    public bool rewardGiven;
    public int killCount;
    public int totalEnemies;
}