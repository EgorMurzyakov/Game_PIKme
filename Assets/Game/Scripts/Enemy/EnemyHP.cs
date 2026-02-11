using UnityEngine;

public class EnemyHP : HitPoint
{
    [SerializeField] private Animator animator;

    protected override void Death()
    {
        animator.SetTrigger("Death");
    }

    //public override void TakeDamage(int damage)
    //{
    //    Debug.Log("”рон по ¬–ј√” без брони");
    //}

}
