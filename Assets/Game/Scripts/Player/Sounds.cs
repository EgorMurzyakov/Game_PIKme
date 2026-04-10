using UnityEngine;
using System;
using UnityRandom = UnityEngine.Random; // Псевдоним для Random

public class Sounds : MonoBehaviour
{    
    private AudioSource audioSrc => GetComponent<AudioSource>();

    [SerializeField] private SoundList[] sounds;
    private GroundType ground = GroundType.Ground;    
    private float rayDistance = 2f; // Длина луча
    [SerializeField] private LayerMask groundLayers; // Какие слои считаются землей
    private Vector3 offset = new Vector3(0f, 1f, 0f);
    private string currentGroundTag = "";

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
        Ground,
        Tree,
        Stone,
        Empty
    }

    public enum SoundMagic
    {
        FireballStart,
        FireballEnd,
        TornadoStart,
        TornadoEnd
    }

    public void Update()
    {
        CheckGround();        
    }

    public void PlaySound(SoundType type) // Базовые звуки
    {        
        if (type == SoundType.Footstep && sounds[(int)type].audioClip.Length != 0)
        {
            if (ground == GroundType.Empty) return;
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

    private void CheckGround()
    {
        // Создаем луч из позиции персонажа вниз
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        RaycastHit hit;

        // Пускаем луч
        if (Physics.Raycast(ray, out hit, rayDistance, groundLayers))
        {
            // Получаем тег объекта под ногами
            currentGroundTag = hit.collider.tag;
        }
        else
        {          
            // Если ничего не нашли (персонаж в воздухе)
            currentGroundTag = "";
        }

        switch (currentGroundTag)
        {
            case "Ground":
                ground = GroundType.Ground;
                break;
            case "Tree":
                ground = GroundType.Tree;
                break;
            case "Stone":
                ground = GroundType.Stone;
                break;
            default:
                ground = GroundType.Empty; 
                break;
        }        
    }
}

[Serializable]
public struct SoundList // Для удобной работы в инспекторе
{
    public string name;
    public float voluem;
    public AudioClip[] audioClip;
}