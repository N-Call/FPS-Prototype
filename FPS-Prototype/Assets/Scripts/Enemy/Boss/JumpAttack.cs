using UnityEngine;

public class JumpAttack : BaseState
{
    private BossSM bossSM;
    public JumpAttack(StateMachine stm) : base(name: "Jumping", stm)
    {

        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Is jumping");
        bossSM.rigidBody.AddForce(Vector3.up * bossSM.jumpForce, ForceMode.Impulse);
    }
    public override void StateLogic()
    {
        base.StateLogic();
        RaycastHit hit;
        if (Physics.Raycast(bossSM.transform.position, -bossSM.transform.up, out hit, 2))
        {
            bossSM.ChangeState(bossSM.idle);
        }
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
