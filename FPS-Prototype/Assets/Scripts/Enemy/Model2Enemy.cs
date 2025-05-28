
using System.Collections;
using UnityEngine;

public class Model2Enemy : EnemyController
{
    [Header("Model 2 Setteings")]
    [SerializeField] protected Transform shootPosL;
    [SerializeField] protected Transform shootPosR;

    bool canShoot;
    bool leftShot;
    protected override void Update()
    {
        base.Update();

        shootTimer += Time.deltaTime;


        if (canShoot && shootTimer >= shootRate)
        {
            if (leftShot)
            {
                LShoot();
            }

            else
            {
                RShoot();
            }
            leftShot = !leftShot;
            shootTimer = 0f;
            SoundManager.instance.PlaySFX("enemyShot", 0.2f);

        }
    }
    protected override bool CanSeePlayer()
    {
         playerDir = (GameManager.instance.player.transform.position - transform.position);
         angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
         Debug.DrawRay(transform.position, new Vector3(playerDir.x, 0, playerDir.z));

            RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {

            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                canShoot = true;
                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }
        canShoot = false;
        agent.stoppingDistance = 0;
        return false;    
    }
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (currentHealth > 0.0f)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
    void LShoot()
    {
        if (shootPosL != null)
        {
            
            Instantiate(bullet, shootPosL.position, shootPosL.rotation);
        }
    }

    void RShoot()
    {
        if (shootPosR != null)
        {
        
        Instantiate(bullet, shootPosR.position, shootPosR.rotation);

        }

    }
}
