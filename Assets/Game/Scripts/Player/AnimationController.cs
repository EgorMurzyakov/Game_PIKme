using UnityEngine;
using UnityEngine.Pool;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;   

    public void ChoosingAction(state _st, char _KM)
    {
        ResetAllTrigger(); // —брасываем все триггеры, активным всегда может быть только 1

        switch (_st)
        {
            case state.Idle:
                animator.SetTrigger("goIdle");
                break;
            case state.Walk:
                animator.SetTrigger("goWalk");
                break;
            case state.Run:
                animator.SetTrigger("goRun");
                break;
            case state.Sprint:
                animator.SetTrigger("goSprint");
                break;
            case state.Dodge:
                animator.SetTrigger("goDodge");
                break;
            case state.Attack:
                animator.SetTrigger(_KM == 'L' ? "LKM" : "PKM");
                break;
            case state.Death:
                animator.SetTrigger("Death");
                break;
        }
    }

    private void ResetAllTrigger()
    {
        animator.ResetTrigger("goIdle");
        animator.ResetTrigger("goWalk");
        animator.ResetTrigger("goRun");
        animator.ResetTrigger("goSprint");
        animator.ResetTrigger("goDodge");
        animator.ResetTrigger("LKM");
        animator.ResetTrigger("PKM");

    }

    public void ResetToIdle()
    {
        ResetAllTrigger();
        animator.Rebind();
        animator.Update(0f);
        animator.SetTrigger("goIdle");
    }

    //public void ProbAOA()
    //{
    //    animator.applyRootMotion = false;
    //}
}
