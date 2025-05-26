using System;
using UnityEngine;

public class MineEnemy : EnemyController
{
    public void Awake()
    {
        //Unity Events Listening for OnRangeTrigger and OnExplosionTrigger
        RangeTrigger.onTriggerEnter.AddListener(() => OnRangeTriggerEnter(GameManager.instance.player.GetComponent<Collider>()));
        RangeTrigger.onTriggerExit.AddListener(() => OnRangeTriggerExit(GameManager.instance.player.GetComponent<Collider>()));
        ExplosionTrigger.onTriggerEnter.AddListener(() => OnExplosionTriggerEnter(GameManager.instance.player.GetComponent<Collider>()));
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
                agent.stoppingDistance = stoppingDistanceOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }
 public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        agent.SetDestination(GameManager.instance.player.transform.position);  
    }

    void OnRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnExplosionTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        SoundManager.instance.PlaySFX("mineExplosion", 0.3f);
        IDamage damage = GameManager.instance.GetComponent<IDamage>();
        damage?.TakeDamage(damageAmount);
        GameManager.instance.ToggleReticle();
        Destroy(gameObject);
        GameManager.instance.UpdateEnemyCounter(-1);
    }
}
