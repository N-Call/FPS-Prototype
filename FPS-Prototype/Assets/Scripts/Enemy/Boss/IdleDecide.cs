using UnityEngine;

public class IdleDecide : BaseState 
{
    public bool shouldShoot;

    private BossSM bossSM;
    private float counter;
    private float spawnOrbTimer;
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

        if (shouldShoot)
        {
            bossSM.shoot.isShootingOrb = true;
            bossSM.ChangeState(bossSM.shoot);
            shouldShoot = false;
            return;
        }

        //Phase one State Logics
        base.StateLogic();
        if (Vector3.Distance(player.transform.position, bossSM.rigidBody.position) < bossSM.decideDis / 2)
        {
            bossSM.ChangeState(bossSM.roll);
        }
        else if(Vector3.Distance(player.transform.position, bossSM.rigidBody.position) < bossSM.decideDis * 2)
        {
            int range = Random.Range(0, 2);
            if (range > 0)
            {
                bossSM.ChangeState(bossSM.run);

            }
            else
            {
                bossSM.ChangeState(bossSM.roll);
            }
        }
        else
        {
            int range = Random.Range(0, 2);
            if (range > 0)
            {
                bossSM.ChangeState(bossSM.run);

            }
            else
            {
                bossSM.ChangeState(bossSM.shoot);
            }
        }

        if (bossSM.GetCurrentHealth() < bossSM.health / 2)
        {
            //Phase Two State Logics
            switch (bossSM.currentAbility)
            {
                case EAbility.speedBoost:
                    bossSM.ChangeState(bossSM.speed);
                    break;
                case EAbility.jumpBoost:
                    Debug.Log("Should Jump");
                    bossSM.ChangeState(bossSM.jump);
                    break;
                case EAbility.invensBoost:
                    bossSM.ChangeState(bossSM.beam);
                    break;
                default: break;
            }
        }

    }
    public override void Action()
    {
        bossSM.LookAtPlayer(0);
        base.Action();
        if (bossSM.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            counter -= Time.deltaTime;
        }

        if (bossSM.GetCurrentHealth() < bossSM.health / 2)
        {
            bossSM.orbSpawnCounter += Time.deltaTime;
        }
    }

    public override void Exit() 
    { 
        base.Exit();
        bossSM.LookAtPlayer(0);

    }





}
