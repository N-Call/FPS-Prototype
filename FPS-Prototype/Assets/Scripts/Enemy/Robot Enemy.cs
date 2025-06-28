using UnityEngine;

public class RobotEnemy : EnemyController
{
    
    protected override bool CanSeePlayer()
    {
        playerDir = (GameManager.instance.player.transform.position - transform.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV && hit.collider.CompareTag("Player"))
            {
                Vector3 shootDir = (GameManager.instance.player.transform.position - shootPos.position);

                float pitch = Vector3.SignedAngle(shootDir, new Vector3(shootDir.x, 0, shootDir.z), shootPos.right);
                pitch = Mathf.Clamp(-pitch, -45, 45);

                shootPos.LookAt(GameManager.instance.player.transform.position);

                Vector3 eulerAngles = shootPos.rotation.eulerAngles;
                eulerAngles.x = pitch;
                shootPos.rotation = Quaternion.Euler(eulerAngles);

                agent.SetDestination(GameManager.instance.player.transform.position);

                if (shootTimer >= shootRate)
                {
                    Shoot();
                }

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
        if (currentHealth > 0.0f)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }
      
    }

    protected override void Shoot()
    {
        if (shootPos != null)
        {
            shootTimer = 0;
            Instantiate(bullet, shootPos.position, shootPos.rotation);
        }
        SoundManager.instance.PlaySFX("enemyShot");
    }
}
