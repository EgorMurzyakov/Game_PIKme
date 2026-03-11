using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHP : HitPoint
{
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private InventoryManager inventoryManager;

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;
        inventoryManager.EatFood += AddHP;
    }
    public void OnDestroy()
    {
        inventoryManager.EatFood -= AddHP;
    }

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

    public override void AddHP(int _HP) // Лечение
    {
        if (currentHitPoint + _HP < maxHitPoint)
        {
            currentHitPoint += _HP;
        }
        else
        {
            currentHitPoint = maxHitPoint;
        }

        playerUI.SetHitPointUI((float)currentHitPoint / (float)maxHitPoint);
    }
}
