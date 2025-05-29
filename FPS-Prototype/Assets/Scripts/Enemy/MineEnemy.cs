using System;
using UnityEngine;

public class MineEnemy : EnemyController
{
    [Header("Mine Settings")]
    [SerializeField] float explosionDistance;
    [SerializeField] int damageAmount;


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
                if (hit.distance <= explosionDistance)
                {
                    Explode();
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
        if (currentHealth > 0.0f)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
    }
          
    public void Explode()
    {
        SoundManager.instance.PlaySFX("mineExplosion", 0.3f);
        IDamage damage = GameManager.instance.player.GetComponent<IDamage>();
        damage?.TakeDamage(damageAmount);
        GameManager.instance.ToggleReticle();
        Destroy(gameObject);
        GameManager.instance.UpdateEnemyCounter(-1);
    }
}
