using UnityEngine;

public class MineEnemy : EnemyController
{
    public void Awake()
    {
        RangeTrigger.EnteredTrigger += OnRangeTriggerEnter;
        RangeTrigger.ExitedTrigger += OnRangeTriggerExit;

        ExplosionTrigger.EnteredTrigger += OnExplosionTriggerEnter;
       
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
        IDamage damage = GameManager.instance.player.GetComponent<IDamage>();
        damage?.TakeDamage(damageAmount);
        GameManager.instance.ToggleReticle();
        gameObject.SetActive(false);
        isDead = true;
        GameManager.instance.UpdateEnemyCounter(-1);
    }
}
