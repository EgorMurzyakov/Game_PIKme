using UnityEngine;

public abstract class HitPoint : MonoBehaviour 
{
    [SerializeField] protected int startHitPoint; // Начальное значение
    protected int maxHitPoint; // Максимальное значение (увеличивается при прокачке персонажа)
    protected int currentHitPoint;    

    public virtual void TakeDamage(int _damage) // Нанесение урона
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

    }

    public virtual void AddHP(FoodItem _itemSO) // Лечение
    {
        if (currentHitPoint + _itemSO.healthAmount < maxHitPoint)
        {
            currentHitPoint += _itemSO.healthAmount;
        }
        else
        {
            currentHitPoint = maxHitPoint;
        }
    }

    public int GetHP() // Проверка HP
    {
        return currentHitPoint;
    }

    protected abstract void Death(); // Должен переопределяться в PLayerHP и EnemyHP, оттуда обращается к StateMashine -> state.Death
}
