using UnityEngine;

public class EnemyHP : HitPoint
{
    //[SerializeField] private Animator animator;
    [SerializeField] private EnemyStateMachine stateMachine;
    [SerializeField] private EnemyUi enemyUI;

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;
    }

    protected override void Death()
    {
        stateMachine.GoDeathState();
        enemyUI.DeleteHpSlider();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");


        //Debug.Log("Враг умер, слой изменён на: " + gameObject.layer);
    }

    public override void TakeDamage(int _damage) // Нанесение урона
    {
        if (currentHitPoint - _damage > 0)
        {
            currentHitPoint -= _damage;
        }
        else
        {
            currentHitPoint = 0;
            Death();
        }

        enemyUI.SetHitPointUI((float)currentHitPoint, (float)maxHitPoint);
    }

}
