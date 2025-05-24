using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootAttack : BaseState
{
    private BossSM bossSM;
    private Vector3 playerDir;
    private float angleToPlayer;
    private Quaternion LShoulderOrig;
    private Quaternion RShoulderOrig;
    private float lookWeight = 0.2f;

    public ShootAttack(StateMachine stm) : base(name: "Shooting", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("Shoot", 0.2f);
        LShoulderOrig = bossSM.lShoulder.localRotation;
        RShoulderOrig = bossSM.rShoulder.localRotation;
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
        playerDir = (GameManager.instance.player.transform.position - bossSM.transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), bossSM.transform.forward);

        bossSM.transform.LookAt(GameManager.instance.player.transform);
        bossSM.transform.eulerAngles = new Vector3(0, bossSM.transform.eulerAngles.y, 0);

        Quaternion targetRotation = Quaternion.LookRotation(GameManager.instance.player.transform.position - bossSM.lShoulder.position);
        bossSM.lShoulder.rotation = Quaternion.Slerp(bossSM.lShoulder.rotation, targetRotation, lookWeight);

        targetRotation = Quaternion.LookRotation(GameManager.instance.player.transform.position - bossSM.rShoulder.position);
        bossSM.rShoulder.rotation = Quaternion.Slerp(bossSM.rShoulder.rotation, targetRotation, lookWeight);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
