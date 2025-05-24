using UnityEngine;

public class SpeedAttack : BaseState
{
    private BossSM bossSM;
    public SpeedAttack(StateMachine stm) : base(name: "Speeding", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("Shoot", 0.2f);
    }
    public override void StateLogic()
    {
        base.StateLogic();
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
