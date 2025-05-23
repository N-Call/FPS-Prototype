using UnityEngine;

public class RollAttack : BaseState
{

    private BossSM bossSM;
    public RollAttack(StateMachine stm) : base(name: "Rolling", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("RollTransform", 0.2f);
    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("RollTransform"))
            bossSM.currentDecideDis = (int)Vector3.Distance(GameManager.instance.player.transform.position, bossSM.rigidBody.position) - bossSM.rollDecideDis;

        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("BallToNormal"))
            bossSM.ChangeState(bossSM.idle);
    }
    public override void Action()
    {
        base.Action();
    }

    public override void Exit()
    {
        base.Exit();
        bossSM.currentDecideDis = 0;
        bossSM.animator.SetInteger("DecideDis", bossSM.currentDecideDis);
    }
}
