using UnityEngine;

public class DamageDetector : MonoBehaviour // ”ниверсальный (и дл€ игрока и дл€ врагов)
{
    [SerializeField] private Animator animator;

    public void GetDamage()
    {
        animator.SetTrigger("goDamage");
    }
    
}
