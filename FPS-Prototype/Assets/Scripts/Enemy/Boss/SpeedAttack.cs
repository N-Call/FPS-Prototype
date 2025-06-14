using UnityEngine;

public class SpeedAttack : BaseState
{
    private BossSM bossSM;
    private float attackTimer;
    private float speedTimer;
    private bool isTransforming;
    private float origSpeed;
    public SpeedAttack(StateMachine stm) : base(name: "Speeding", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("SpeedTransform", 0.2f);
        origSpeed = bossSM.agent.acceleration;
        bossSM.agent.acceleration = bossSM.speedAmount;
        bossSM.agent.isStopped = false;
    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (speedTimer >= bossSM.speedDuration && !isTransforming)
        {
            bossSM.animator.CrossFade("SpeedToNormal", 0.2f);
            isTransforming = true;
        }
    }
    public override void Action()
    {
        base.Action();
        if(isTransforming) { return; }
        speedTimer += Time.deltaTime;
        bossSM.agent.SetDestination(GameManager.instance.player.transform.position);
        bossSM.transform.LookAt(new Vector3(GameManager.instance.player.transform.position.x, bossSM.transform.position.y, GameManager.instance.player.transform.position.z));
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("SpeedIdle"))
        {
            attackTimer += Time.deltaTime;
        }

        if (attackTimer >= bossSM.speedAttackRate && Vector3.Distance(GameManager.instance.player.transform.position, bossSM.transform.position) < bossSM.speedAttackRange)
        {
            bossSM.animator.CrossFade("SpeedAttack", 0.2f);

            attackTimer = 0;
        }
    }

    public void ChangeToIdle()
    {
        bossSM.ChangeState(bossSM.idle);
    }

    public override void Exit()
    {
        base.Exit();
        speedTimer = 0;
        isTransforming = false;
        bossSM.agent.isStopped = true;
        bossSM.agent.acceleration = origSpeed;
        bossSM.currentAbility = EAbility.None;

    }
}
