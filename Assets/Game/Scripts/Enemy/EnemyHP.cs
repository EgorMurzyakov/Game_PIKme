using UnityEngine;

public class EnemyHP : HitPoint
{
    //[SerializeField] private Animator animator;
    [SerializeField] private EnemyStateMachine stateMachine;

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;
    }

    protected override void Death()
    {
        stateMachine.GoDeathState();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        //animator.SetTrigger("Death");
    }

    //public override void TakeDamage(int damage)
    //{
    //    Debug.Log("”рон по ¬–ј√” без брони");
    //}

}
