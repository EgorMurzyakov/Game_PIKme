using System;
using UnityEngine;
using Unity.VisualScripting;

public class ColliderSwitch : MonoBehaviour // Универсальный (и для игрока и для врагов)
{
    private state currentState = state.Empty;

    public event Action weaponColliderOn;
    public event Action weaponColliderOff;

    public void ChoosingAction(state _st)
    {
        currentState = _st;
        if (currentState != state.Attack) // При выходе с состояния атаки Принудительно выключаем коллайдер
        {
            WeaponColliderOFF();
        }
    }

    public void WeaponColliderOn()
    {
        if (currentState == state.Attack) // Защита от случайного срабатывания (на случай, если состояние сменится, а Анимационное событие будет вызвано)
        {
            weaponColliderOn?.Invoke();
        }
    }

    public void WeaponColliderOFF()
    {
        weaponColliderOff?.Invoke();
    }

}
