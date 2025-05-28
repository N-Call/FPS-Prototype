using UnityEngine;

public class JumpAttack : BaseState
{
    private BossSM bossSM;

    private bool isStopped;
    public JumpAttack(StateMachine stm) : base(name: "Jumping", stm)
    {

        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.animator.CrossFade("Jumping", 0.02f);
        isStopped = false;
        bossSM.agent.enabled = false;
        bossSM.transform.LookAt(new Vector3(bossSM.targetPoint.position.x, bossSM.transform.position.y, bossSM.targetPoint.position.z));

    }
    public override void StateLogic()
    {
        base.StateLogic();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("BallToNormal"))
            bossSM.ChangeState(bossSM.idle);
    }
    public override void Action()
    {
        base.Action();
        if( !isStopped && Vector3.Distance(bossSM.transform.position, bossSM.targetPoint.position) <= 2f)
        {
            bossSM.animator.CrossFade("Slaming", 0.02f);
            bossSM.rigidBody.linearVelocity = Vector3.up * bossSM.gravity;
            isStopped = true;
        }
    }

    public override void Exit()
    {
        base.Exit();

    }

    public void ResetRigid()
    {
        bossSM.rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        bossSM.rigidBody.excludeLayers = 0;
    }

    public void JumpToTarget()
    {
        bossSM.rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        bossSM.rigidBody.excludeLayers = bossSM.ignorelayer;
        Vector3 jumpVelocity = CalculateJumpVelocity(bossSM.transform.position, bossSM.targetPoint.position, bossSM.targetPoint.position.y - bossSM.transform.position.y);
        bossSM.rigidBody.AddForce(jumpVelocity, ForceMode.Impulse);
    }

    Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Physics.gravity.y;
        float verticalDistance = end.y - start.y;
        Vector3 horizontalDistance = new Vector3(end.x - start.x, 0, end.z - start.z);

        float timeToApex = Mathf.Sqrt(-2 * height / gravity);
        float totalTime = timeToApex + Mathf.Sqrt(2 * Mathf.Abs(verticalDistance - height) / -gravity);

        Vector3 horizontalVelocity = horizontalDistance / totalTime;
        float verticalVelocity = Mathf.Sqrt(-2 * gravity * height);

        return horizontalVelocity + Vector3.up * verticalVelocity;
    }
}
