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
        bossSM.LookAtPlayer(0);
        counter = bossSM.decideTime;
        player = GameManager.instance.player;
    }
    public override void StateLogic()
    {
        if(counter > 0 || bossSM.GetCurrentState() != bossSM.idle) { return; }

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

        if (bossSM.GetCurrentHealth() < bossSM.health / 2)
        {
            //Phase Two State Logics
            switch (bossSM.currentAbility)
            {
                case BossSM.Ability.speedBoost:
                    bossSM.ChangeState(bossSM.speed);
                    break;
                case BossSM.Ability.jumpBoost:
                    Debug.Log("Should Jump");
                    bossSM.ChangeState(bossSM.jump);
                    break;
                case BossSM.Ability.invensBoost:
                    break;
                default: break;
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
