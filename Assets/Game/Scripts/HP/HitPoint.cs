using UnityEngine;

public abstract class HitPoint : MonoBehaviour 
{
    [SerializeField] protected int startHitPoint; // Начальное значение
    protected int maxHitPoint; // Максимальное значение (увеличивается при прокачке персонажа)
    protected int currentHitPoint;

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;
    }

    public virtual void TakeDamage(int _damage) // Нанесение урона
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

    }

    public virtual void AddHP(int _HP) // Лечение
    {
        if (currentHitPoint + _HP < maxHitPoint)
        {
            currentHitPoint += _HP;
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
