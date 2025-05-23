using UnityEngine;

public class RunAttack : BaseState
{
    private BossSM bossSM;
    public RunAttack(StateMachine stm) : base(name: "Running", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("SpinAttack", 0.2f);
    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
            !bossSM.animator.GetNextAnimatorStateInfo(0).IsName("SpinAttack"))
            bossSM.ChangeState(bossSM.idle);
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
