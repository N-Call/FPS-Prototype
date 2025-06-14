using UnityEngine;
using UnityEngine.AI;

public class RollAttack : BaseState
{

    private BossSM bossSM;
    private float currentSpeed;
    public RollAttack(StateMachine stm) : base(name: "Rolling", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.currentbullet = bossSM.homingBullet;
        currentSpeed = bossSM.agent.speed;
        bossSM.agent.speed = bossSM.rollSpeed;
        bossSM.currentDecideDis = 0;
        bossSM.animator.SetFloat("DecideDis", bossSM.currentDecideDis);
        bossSM.animator.CrossFade("RollTransform", 0.2f);
        bossSM.agent.isStopped =false;
    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("RollTransform"))
            bossSM.currentDecideDis = Vector3.Distance(GameManager.instance.player.transform.position, bossSM.rigidBody.position) - bossSM.rollDecideDis;

        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("BallToNormal"))
            bossSM.ChangeState(bossSM.idle);
    }
    public override void Action()
    {
        base.Action();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("SpinBallAttack"))
        {
            bossSM.agent.destination = GameManager.instance.player.transform.position;
        }
    }

    public override void Exit()
    {
        base.Exit();
        bossSM.agent.isStopped = true;
    }
}
