using UnityEngine;

public class IdleDecide : BaseState 
{
    private BossSM bossSM;
    public int counter;
    private GameObject player;

    public IdleDecide(StateMachine stm) : base(name: "decide", stm) 
    {
        bossSM = (BossSM) this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Is Working");
        bossSM.LookAtPlayer(0);
        counter = bossSM.decideTime;
        player = GameManager.instance.player;
    }
    public override void StateLogic()
    {
        if(counter > 0 || bossSM.GetCurrentState() != bossSM.idle) { return; }

        if (bossSM.GetCurrentHealth() > bossSM.health / 2)
        {
            //Phase one State Logics
            base.StateLogic();
            if (Vector3.Distance(player.transform.position, bossSM.rigidBody.position) < bossSM.decideDis)
            {
                bossSM.ChangeState(bossSM.roll);
            }
            else if(Vector3.Distance(player.transform.position, bossSM.rigidBody.position) < bossSM.decideDis * 2)
            {
                bossSM.ChangeState(bossSM.run);
            }
            else
            {
                bossSM.ChangeState(bossSM.shoot);
            }

        }
        else
        {
            //Phase Two State Logics

            base.StateLogic();
            if (Vector3.Distance(player.transform.position, bossSM.rigidBody.position) < bossSM.decideDis)
            {
                Debug.Log("Activate: Speed");
                bossSM.ChangeState(bossSM.jump);
                //bossSM.ChangeState(bossSM.speed);
            }
            else
            {
                Debug.Log("Activate: Jump");
                bossSM.ChangeState(bossSM.jump);
            }
        }
    }
    public override void Action()
    {
        bossSM.LookAtPlayer(0);
        base.Action();
        counter--;
    }

    public override void Exit() 
    { 
        base.Exit();
        bossSM.LookAtPlayer(0);

    }





}
