using UnityEngine;

public class DamageDetector : MonoBehaviour // Универсальный (и для игрока и для врагов), определяет кол-во нанесенного урона
{
    [SerializeField] private Animator animator;
    [SerializeField] private HitPoint hitPoint;

    public void GetDamage(int _weaponDamage) // Помимо сырого урона, в будущем, нужно учитывать прокачку игрока
    {
        animator.SetTrigger("goDamage"); // Нужно изменить и добавить классы наследники
        hitPoint.TakeDamage(_weaponDamage); 
    }
    
}
