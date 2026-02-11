using UnityEngine;

public class PlayerHP : HitPoint
{ 
    protected override void Death()
    {
        // 
    }
    public override void TakeDamage(int damage)
    {
        Debug.Log("”рон по »√–ќ ” с учетом брони");
    }

}
