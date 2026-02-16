using System.Collections;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem trailParticles_1;
    [SerializeField] private ParticleSystem trailParticles_2;
    [SerializeField] private ParticleSystem trailParticles_3;
    [SerializeField] private ParticleSystem trailParticles_4;
    [SerializeField] private ParticleSystem trailParticles_5;
    [SerializeField] private ParticleSystem trailParticles_6;

    public void StartTrail(int a)
    {
        switch (a) {
            case 1:
                trailParticles_1.Play();
                break;
            case 2:
                trailParticles_2.Play();
                break;
            case 3:
                trailParticles_3.Play();
                break;
            case 4:
                trailParticles_4.Play();
                break;
            case 5:
                trailParticles_5.Play();
                break;
            case 6:
                trailParticles_6.Play();
                break;
        }                   
    } 

    public void StopTrail(int a)
    {
        switch (a)
        {
            case 1:
                trailParticles_1.Stop();
                break;
            case 2:
                trailParticles_2.Stop();
                break;
            case 3:
                trailParticles_3.Stop();
                break;
            case 4:
                trailParticles_4.Stop();
                break;
            case 5:
                trailParticles_5.Stop();
                break;
            case 6:
                trailParticles_6.Stop();
                break;
        }
    }
}
