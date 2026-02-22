using UnityEngine;

public class PlayerHP : HitPoint
{
    [SerializeField] private PlayerStateMachine stateMachine;
    protected override void Death()
    {
        stateMachine.GoDeathState();
    }
    //public override void TakeDamage(int damage)
    //{
    //    Debug.Log("”рон по »√–ќ ” с учетом брони");
    //    Debug.Log("HP player - " + currentHitPoint);
    //}

}
