using UnityEngine;

public class PlayerHP : HitPoint
{
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerUI playerUI;
    protected override void Death()
    {
        stateMachine.GoDeathState();
    }

    public override void TakeDamage(int _damage) // Нанесение урона
    {
        Debug.Log("Базовый урон");
        if (currentHitPoint - _damage > 0)
        {
            Debug.Log("Было - " + currentHitPoint);
            currentHitPoint -= _damage;
            Debug.Log("Стало - " + currentHitPoint);
        }
        else
        {
            Debug.Log("Death");
            currentHitPoint = 0;
            Death();
        }

        playerUI.SetHitPointUI((float)currentHitPoint / (float)maxHitPoint);
    }
    //public override void TakeDamage(int damage)
    //{
    //    Debug.Log("Урон по ИГРОКУ с учетом брони");
    //    Debug.Log("HP player - " + currentHitPoint);
    //}

}
