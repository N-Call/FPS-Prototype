using System;
using UnityEngine;

public class MineEnemy : EnemyController
{
    public void Awake()
    {
        RangeTrigger.onTriggerEnter.AddListener(() => OnRangeTriggerEnter(GameManager.instance.player.GetComponent<Collider>()));
        RangeTrigger.onTriggerExit.AddListener(() => OnRangeTriggerExit(GameManager.instance.player.GetComponent<Collider>()));

        ExplosionTrigger.onTriggerEnter.AddListener(() => OnExplosionTriggerEnter(GameManager.instance.player.GetComponent<Collider>()));

    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        SoundManager.instance.PlaySFX("turretHit", 0.2f);
        if (currentHealth <= 0)
        {
            SoundManager.instance.PlaySFX("turretDestroy", 0.2f);
        }
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
        //gameObject.SetActive(false);
        //isDead = true; 
        Destroy(gameObject);
        GameManager.instance.UpdateEnemyCounter(-1);
    }
}
