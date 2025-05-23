using UnityEngine;

public class ShootAttack : BaseState
{
    private BossSM bossSM;
    public ShootAttack(StateMachine stm) : base(name: "Shooting", stm)
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
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
            !bossSM.animator.GetNextAnimatorStateInfo(0).IsName("Shoot"))
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
