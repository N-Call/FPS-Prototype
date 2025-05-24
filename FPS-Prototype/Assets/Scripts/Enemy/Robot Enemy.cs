using UnityEngine;

public class RobotEnemy : EnemyController
{
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        SoundManager.instance.PlaySFX("turretHit", 0.2f);
        if (currentHealth <= 0)
        {
            SoundManager.instance.PlaySFX("turretDestroy", 0.2f);
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        SoundManager.instance.PlaySFX("enemyShot", 0.2f);
    }
}
