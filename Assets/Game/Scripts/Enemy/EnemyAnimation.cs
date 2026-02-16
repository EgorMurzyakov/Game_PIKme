using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void GetAnimation(state _st)
    {
        ResetAllTrigger();

        switch (_st)
        {
            case state.Idle:
                anim.SetTrigger("goIdle");
                break;

            case state.Walk:
                anim.SetTrigger("goWalk");
                break;

            case state.Attack:
                anim.SetTrigger("goAttack");
                break;

            case state.Action:
                anim.SetTrigger("goAction");
                break;

            case state.Damage:
                anim.SetTrigger("goDamage");
                break;

            case state.Death:
                anim.SetTrigger("Death");
                break;
        }
    }

    private void ResetAllTrigger()
    {
        anim.ResetTrigger("goIdle");
        anim.ResetTrigger("goWalk");
        anim.ResetTrigger("goAttack");
        anim.ResetTrigger("goAction");
        anim.ResetTrigger("goDamage");
    }

}
