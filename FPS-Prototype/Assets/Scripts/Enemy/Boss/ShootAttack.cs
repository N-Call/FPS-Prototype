using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootAttack : BaseState
{
    public bool isShootingOrb;

    private BossSM bossSM;
    private Vector3 playerDir;
    private Vector3 targetPos;
    private float angleToPlayer;
    private Quaternion LShoulderOrig;
    private Quaternion RShoulderOrig;
    private float lookWeight = 0.2f;

    private float pitchOrig;
    private float rpitchOrig;
    private bool reachedPitch;
    private bool isShooting;
    public ShootAttack(StateMachine stm) : base(name: "Shooting", stm)
    {
        bossSM = (BossSM)this.stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        bossSM.LookAtPlayer(0);
        bossSM.currentbullet = bossSM.homingBullet;
        bossSM.animator.CrossFade("Shoot", 0.2f);
        targetPos = GameManager.instance.player.transform.position;
        LShoulderOrig = bossSM.lShoulder.localRotation;
        RShoulderOrig = bossSM.rShoulder.localRotation;
        pitchOrig = 0;
        rpitchOrig = 0;
        isShooting = true;

        if (isShootingOrb)
        {
            ShootOrb();
        }
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
        if (isShootingOrb)
        {
            targetPos = bossSM.targetPoint.position;
        }
        else
        {
            targetPos = GameManager.instance.player.transform.position;
        }

        //bossSM.bodyParts[0].transform.LookAt(new Vector3(targetPos.x, bossSM.bodyParts[0].transform.position.y, targetPos.z));

        playerDir = (targetPos - bossSM.transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), bossSM.transform.forward);

        bossSM.transform.LookAt(targetPos);
        bossSM.transform.eulerAngles = new Vector3(0, bossSM.transform.eulerAngles.y, 0);

        Vector3 middleTargetDir = (targetPos - (Vector3.up * 0.5f)) - bossSM.lShoulder.position;
        Vector3 middleTargetDir2 = (targetPos - (Vector3.up * 0.5f)) - bossSM.rShoulder.position;

        // Calculate the vertical angle from the direction
        float pitch = Vector3.SignedAngle(middleTargetDir, new Vector3(middleTargetDir.x, 0, middleTargetDir.z), bossSM.lShoulder.right);
        pitch = Mathf.Clamp(-pitch, -90, 20);
        float pitch2 = Vector3.SignedAngle(middleTargetDir2, new Vector3(middleTargetDir2.x, 0, middleTargetDir2.z), bossSM.rShoulder.right);
        pitch2 = Mathf.Clamp(-pitch2, -90, 20);

        bossSM.lShoulder.LookAt(targetPos);
        bossSM.rShoulder.LookAt(targetPos);

        Vector3 eulerAngles = bossSM.lShoulder.rotation.eulerAngles;
        Vector3 eulerAngles2 = bossSM.rShoulder.rotation.eulerAngles;

        if (pitch2 - 90 < -120 && pitch > 0)
        {
            pitch = pitch2;
        }
        else if (pitch - 90 < -120 && pitch2 > 0)
        {
            pitch2 = pitch;
        }

        if (pitchOrig > pitch - 90 && !reachedPitch)
        {
            pitchOrig -= 2f;
            rpitchOrig -= 2f;
            eulerAngles.x = pitchOrig;
            eulerAngles2.x = rpitchOrig;
        }
        else if (isShooting)
        {
            
            reachedPitch = true;
            eulerAngles.x = pitch - 90;
            eulerAngles2.x = pitch2 - 90;
            pitchOrig = pitch - 90;
            rpitchOrig = pitch2 - 90;
        }

        if (!isShooting && pitchOrig < 0 && rpitchOrig < 0)
        { 
            pitchOrig += 3;
            rpitchOrig += 3;

            eulerAngles.x = pitchOrig;
            eulerAngles2.x = rpitchOrig;
        }

        bossSM.lShoulder.rotation = Quaternion.Euler(eulerAngles);
        bossSM.rShoulder.rotation = Quaternion.Euler(eulerAngles2);


    }

    public void ShootOrb()
    {
        targetPos = bossSM.targetPoint.position;
        bossSM.currentbullet = bossSM.regularBullet;
        
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    public override void Exit()
    {
        base.Exit();
        reachedPitch = false;
        isShootingOrb = false;
        isShooting = true;
        bossSM.LookAtPlayer(0);
    }
}
