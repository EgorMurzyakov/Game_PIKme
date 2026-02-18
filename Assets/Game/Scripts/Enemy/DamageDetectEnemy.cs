using UnityEngine;

public class DamageDetectEnemy : DamageDetector
{
    [SerializeField] private EnemyStateMachine stateMachine;

    public override void GetDamage(int _weaponDamage) // ѕомимо сырого урона, в будущем, нужно учитывать прокачку игрока
    {
        stateMachine.TakingDamage(true); // √оворим машине состо€ний что получили урон -> она включает анимацию получени€ урона
        hitPoint.TakeDamage(_weaponDamage); // Ќаносим урон
    }
}
