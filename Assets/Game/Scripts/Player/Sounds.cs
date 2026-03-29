using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;

    private AudioSource audioSrc => GetComponent<AudioSource>();

    //public void PlaySound(int _numb, float voluem = 1f, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    //{
    //    audioSrc.pitch = Random.Range(p1, p2);
    //    audioSrc.PlayOneShot(sounds[_numb], voluem);
    //}

    public void PlaySoundLow(int _numb)
    {
        float voluem = 0.1f;
        bool destroyed = false;
        float p1 = 0.85f;
        float p2 = 1.2f;
        audioSrc.pitch = Random.Range(p1, p2);
        audioSrc.PlayOneShot(sounds[_numb], voluem);
    }

    public void PlaySoundMedium(int _numb)
    {
        float voluem = 0.5f;
        bool destroyed = false;
        float p1 = 0.85f;
        float p2 = 1.2f;
        audioSrc.pitch = Random.Range(p1, p2);
        audioSrc.PlayOneShot(sounds[_numb], voluem);
    }

    public void PlaySoundHight(int _numb)
    {
        float voluem = 1f;
        bool destroyed = false;
        float p1 = 0.85f;
        float p2 = 1.2f;
        audioSrc.pitch = Random.Range(p1, p2);
        audioSrc.PlayOneShot(sounds[_numb], voluem);
    }
}
