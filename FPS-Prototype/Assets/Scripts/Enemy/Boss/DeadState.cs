using UnityEngine;

public class DeadState : BaseState
{
    private BossSM bossSM;
    private float DeathCounter;
    public DeadState(StateMachine stm) : base(name: "Dead", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("Dead", 0.2f);
        DeathCounter = bossSM.deathTimer;
    }
    public override void StateLogic()
    {
        base.StateLogic();
        DeathCounter -= Time.deltaTime;

        if(DeathCounter <= 0)
        {
            BossSM.Destroy(bossSM.gameObject);
        }
    }
    public override void Action()
    {
        base.Action();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
