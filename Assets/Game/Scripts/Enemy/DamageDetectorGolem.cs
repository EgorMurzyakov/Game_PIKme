using UnityEngine;

public class DamageDetectorGolem : DamageDetector
{
    [SerializeField] private GolemStateMashine stateMachine;
    public override void GetDamage(int _weaponDamage) // Помимо сырого урона, в будущем, нужно учитывать прокачку игрока
    {
        Debug.Log("Получаем урон - " + _weaponDamage);
        stateMachine.GoDamageState(); // Говорим машине состояний что получили урон -> включает анимацию получения урона
        hitPoint.TakeDamage(_weaponDamage); // Наносим урон
    }
}
