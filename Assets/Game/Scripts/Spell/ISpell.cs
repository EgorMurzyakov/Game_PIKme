using UnityEngine;

public interface ISpell
{
    string Id { get; }
    void Cast(Transform caster, Transform cameraTransform);
}
