using UnityEngine;

public class GolemHP : HitPoint
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private GolemStateMashine golemStateMachine;
    [SerializeField] private GolemUI golemUI;
    [SerializeField] private int secondPhase;
    [SerializeField] private int thirdPhase;   

    public void Start()
    {
        maxHitPoint = startHitPoint;
        currentHitPoint = startHitPoint;

        playerStateMachine.playerDeath += RestartGolem;
    }

    public void OnDestroy()
    {
        playerStateMachine.playerDeath -= RestartGolem;
    }

    protected override void Death()
    {
        golemStateMachine.GoDeathState();
        golemUI.DeleteHpSlider();
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        //if (KillCounter.Instance != null)
        //    KillCounter.Instance.ReportKill();

        //Debug.Log("Враг умер, слой изменён на: " + gameObject.layer);
    }

    public override void TakeDamage(int _damage) // Нанесение урона
    {        
        if (currentHitPoint - _damage > 0)
        {
            currentHitPoint -= _damage;

            if (currentHitPoint < thirdPhase)
            {
                golemStateMachine.SetGolemPhase(GolemStateMashine.golemPhase.Third);
            }
            else if (currentHitPoint < secondPhase)
            {
                golemStateMachine.SetGolemPhase(GolemStateMashine.golemPhase.Second);
            }
        }
        else
        {
            currentHitPoint = 0;
            Death();
        }

        golemUI.SetHitPointUI(currentHitPoint, maxHitPoint, secondPhase, thirdPhase);
    }

    public void RestartGolem()
    {
        currentHitPoint = startHitPoint;
        golemUI.SetHitPointUI(currentHitPoint, maxHitPoint, secondPhase, thirdPhase);
    }
}
