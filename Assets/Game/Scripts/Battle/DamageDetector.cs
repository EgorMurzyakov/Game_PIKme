using UnityEngine;

public abstract class DamageDetector : MonoBehaviour // Универсальный (и для игрока и для врагов), определяет кол-во нанесенного урона
{
    [SerializeField] protected HitPoint hitPoint;

    public abstract void GetDamage(int _weaponDamage); // Помимо сырого урона, в будущем, нужно учитывать прокачку игрока
    
    
    
}
