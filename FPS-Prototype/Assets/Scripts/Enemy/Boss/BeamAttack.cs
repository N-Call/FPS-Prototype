using UnityEngine;

public class BeamAttack : BaseState
{
    private BossSM bossSM;
    public BeamAttack(StateMachine stm) : base(name: "Beaming", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.LookAtPlayer(0);
        bossSM.animator.CrossFade("LaserBeam", 0.2f);
        bossSM.SetInvincible(true);
    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
            !bossSM.animator.GetNextAnimatorStateInfo(0).IsName("LaserBeam"))
            bossSM.ChangeState(bossSM.idle);

    }
    public override void Action()
    {
        base.Action();
        bossSM.bodyParts[1].transform.LookAt(GameManager.instance.player.transform.position + Vector3.down);
    }

    public override void Exit()
    {
        base.Exit();
        bossSM.LookAtPlayer(0);
        bossSM.SetInvincible(false);
    }
}
