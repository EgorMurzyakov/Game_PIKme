using UnityEngine;

public struct PlayerInput
{
    public Vector2 Move;
    public bool WASD;
    public bool Shift;
    public bool Alt;
    public bool LKM;
    public bool PKM;

    public PlayerInput(Vector2 _Move, bool _WASD, bool _Shift, bool _Alt, bool _LKM, bool _PKM) // Конструктор
    {
        Move = _Move;
        WASD = _WASD;
        Shift = _Shift;
        Alt = _Alt;
        LKM = _LKM;
        PKM = _PKM;
    }

    // Методы (только для чтения!)
    public readonly bool IsMoving => Move.magnitude > 0.1f;

    public readonly bool WantsAttack => LKM || PKM;

    public readonly bool HasAnyInput => IsMoving || Shift || Alt || WantsAttack;

    // Статический метод для создания "по умолчанию"
    public static PlayerInput Default => new PlayerInput(
        _Move: Vector2.zero,
        _WASD: false,
        _Shift: false,
        _Alt: false,
        _LKM: false,
        _PKM: false
    );

}
