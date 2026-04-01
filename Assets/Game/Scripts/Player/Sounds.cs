using UnityEngine;
using System;
using UnityRandom = UnityEngine.Random; // Псевдоним для Random

public class Sounds : MonoBehaviour
{    
    private AudioSource audioSrc => GetComponent<AudioSource>();

    [SerializeField] private SoundList[] sounds;
    public GroundType ground = GroundType.Soil;

    public enum SoundType // !!! ВАЖНО !!! чтобы в инспекторе был тот же порядок
    {
        Footstep,
        Dodge,
        Attack,
        Hit,
        Damage,
        Death,
        Magic
    }

    public enum GroundType
    {
        Soil,
        Dirt,
        Tree,
        Stone
    }

    public enum SoundMagic
    {
        FireballStart,
        FireballEnd,
        TornadoStart,
        TornadoEnd
    }

    public void PlaySound(SoundType type) // Базовые звуки
    {
        if (type == SoundType.Footstep && sounds[(int)type].audioClip.Length != 0)
        {
            // Берем вариацию звука в зависимости от типа земли под ногами
            audioSrc.PlayOneShot(sounds[(int)type].audioClip[(int)ground], sounds[(int)type].voluem); 
        }
        else if (sounds[(int)type].audioClip.Length != 0)
        {
            // Берем случайную вариацию звука
            audioSrc.PlayOneShot(sounds[(int)type].audioClip[UnityRandom.Range(0, sounds[(int)type].audioClip.Length)], sounds[(int)type].voluem);
        }
    }

    public void PlayMagicSound(SoundMagic magicType) // Магия
    {
        if (sounds[(int)SoundType.Magic].audioClip.Length != 0)
        {
            audioSrc.PlayOneShot(sounds[(int)SoundType.Magic].audioClip[(int)magicType], sounds[(int)magicType].voluem);
        }
    }

    public void SetGroundType(GroundType _tp)
    {
        ground = _tp;
    }
}

[Serializable]
public struct SoundList // Для удобной работы в инспекторе
{
    public string name;
    public float voluem;
    public AudioClip[] audioClip;
}