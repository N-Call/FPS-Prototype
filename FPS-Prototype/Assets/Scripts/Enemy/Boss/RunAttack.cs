using Unity.VisualScripting;
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
        bossSM.agent.isStopped = false;
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

        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("SpinAttack"))
        {
            bossSM.agent.SetDestination(GameManager.instance.player.transform.position);
            bossSM.transform.LookAt(GameManager.instance.player.transform.position);
        }
    }

    public override void Exit()
    {
        bossSM.agent.isStopped = true;
        base.Exit();
    }
}
