using System;
using UnityEngine;
using Unity.VisualScripting;

public enum damageCollider { Weapon, Foot}

public class ColliderSwitch : MonoBehaviour // Универсальный (и для игрока и для врагов)
{
    private state currentState = state.Empty;

    //public event Action weaponColliderOn;
    //public event Action weaponColliderOff;
    public event Action<damageCollider> weaponColliderOn;
    public event Action<damageCollider> weaponColliderOff;   

    public void ChoosingAction(state _st)
    {
        currentState = _st;
        if (currentState != state.Attack) // При выходе с состояния атаки Принудительно выключаем коллайдер
        {
            WeaponColliderOFF(damageCollider.Weapon);
            WeaponColliderOFF(damageCollider.Foot);
        }
    }

    public void WeaponColliderOn(damageCollider _damCol)
    {
        if (currentState == state.Attack || currentState == state.AttackB) // Защита от случайного срабатывания (на случай, если состояние сменится, а Анимационное событие будет вызвано)
        {
            weaponColliderOn?.Invoke(_damCol);
        }
    }

    public void WeaponColliderOFF(damageCollider _damCol)
    {
        weaponColliderOff?.Invoke(_damCol);
    }
}
