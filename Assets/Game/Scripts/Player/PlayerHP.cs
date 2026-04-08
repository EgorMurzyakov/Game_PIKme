using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHP : HitPoint
{
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private InventoryManager inventoryManager;

    private int baceMaxHitPoint; // На это значение не влияет прокачка (сохраняет исходное MaxHP)
    private bool timerOperation = false;
    private float startTime = 0f; // Время начала
    private int durationEffect = 0; // Длительность

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;
        inventoryManager.EatFood += AddHP;
        baceMaxHitPoint = maxHitPoint;
    }

    public void Update()
    {
        if (timerOperation)
        {
            if (startTime + durationEffect < Time.time)
            {
                TemporaryEffectEnd();
                timerOperation = false;
            }
        }
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

    public override void AddHP(FoodItem _item) // Лечение
    {
        TemporaryEffectSrart(_item);

        if (currentHitPoint + _item.healthAmount < maxHitPoint)
        {
            currentHitPoint += _item.healthAmount;
        }
        else
        {
            currentHitPoint = maxHitPoint;
        }

        playerUI.SetHitPointUI((float)currentHitPoint / (float)maxHitPoint);
    }

    public void TemporaryEffectSrart(FoodItem _itemSO)
    {
        if (_itemSO.GetDurationEffect() > 0)
        {
            if (timerOperation) // Перед новым эффектом очищаем старый
            {
                TemporaryEffectEnd();
            }
            timerOperation = true;
            playerUI.BoostIconOn();
            maxHitPoint = (int)(maxHitPoint * _itemSO.GetUpCoeff());
            durationEffect = _itemSO.GetDurationEffect();
            startTime = Time.time;
            playerUI.SetHitPointUI((float)currentHitPoint / (float)maxHitPoint); // Обновляем полоску HP
        }
    }

    private void TemporaryEffectEnd()
    {
        timerOperation = false;
        playerUI.BoostIconOff();
        maxHitPoint = baceMaxHitPoint;
        if (currentHitPoint > maxHitPoint) 
        {
            currentHitPoint = maxHitPoint;
        }
        playerUI.SetHitPointUI((float)currentHitPoint / (float)maxHitPoint); // Обновляем полоску HP
    }
}
